using HIU.Core.Common;
using HIU.Core.Extensions;
using HIU.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace HIU.Core.Service
{
    public class SpotifyService
    {
        const string APP_ID = "5644dafa98a9462d913e3981c6958608";
        const string APP_SC = "4ec64f21b3204ed1a13eba7764665401";
        const string CALLBACK = "http://hearitupapp.com/callback";
        const string TOKEN_URL = "https://accounts.spotify.com/api/token";
        const string AUTHORIZE_URL = "https://accounts.spotify.com/authorize/?";
        const string MY_PLAYLIST = "https://api.spotify.com/v1/me/playlists";
        const string CREATE_URL = "https://api.spotify.com/v1/users/{0}/playlists";
        const string ME = "https://api.spotify.com/v1/me";
        const string PLAYLIST_ADD = "https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks";

        public Uri GetCallbackUri() => new Uri(CALLBACK);

        public SpotifyAuthSettings GetAuthSettings()
        {
            var clientId = $"client_id={APP_ID}&";
            var responseType = "response_type=code&";
            var redirect = $"redirect_uri={Uri.EscapeDataString(CALLBACK)}&";
            var scopes = "scope=playlist-modify-public%20playlist-modify-private%20playlist-read-private%20user-read-email%20user-read-birthdate%20user-read-private&";
            var check_value = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            var correlationState = $"state={check_value}";

            var settingsConcat = string.Concat(clientId, responseType, redirect, scopes, correlationState);
            var requestUrl = string.Concat(AUTHORIZE_URL, settingsConcat);

            return new SpotifyAuthSettings { Url = new Uri(requestUrl), Check = check_value };
        }

        public async Task<SpotifyServiceResponse> FetchTokens(string authResult, string digest)
        {
            SpotifyServiceResponse response = null;
            var authCode = ParseAuthCode(authResult, digest);

            if (!string.IsNullOrEmpty(authCode))
            {
                using (var client = new HttpClient())
                {
                    var stringArray = Encoding.UTF8.GetBytes($"{APP_ID}:{APP_SC}");
                    var header = $"Basic {Convert.ToBase64String(stringArray)}";

                    var postEncodedParam = new HttpFormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string,string>("grant_type", "authorization_code"),
                        new KeyValuePair<string,string>("code", authCode),
                        new KeyValuePair<string,string>("redirect_uri", Uri.EscapeDataString(CALLBACK))
                    });

                    client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", header);
                    var request = await client.PostAsync(new Uri(TOKEN_URL), postEncodedParam);
                    if (request.IsSuccessStatusCode)
                    {
                        response = await request.Content.ReadAs<SpotifyServiceResponse>();
                        response.FetechedOn = DateTime.UtcNow;
                        response.AuthCode = authCode;
                        response.UserId = await GetUserId(response);
                    }
                }
            }

            return response;
        }

        public async Task<List<Track>> SearchTracks(string songTitle)
        {
            var songs = new List<Track>();

            using (var client = new HttpClient())
            {
                var queryDictionary = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("q", Uri.EscapeDataString(songTitle)),
                    new KeyValuePair<string, string>("limit", "12"),
                    new KeyValuePair<string, string>("type", "track")
                };

                var response = await client.GetAsync(new Uri(string.Concat("https://api.spotify.com/v1/search?", queryDictionary.ToQueryString())));
                if (response.IsSuccessStatusCode)
                {
                    var pocoResponse = await response.Content.ReadAs<SpotifySearchResponse>();
                    songs = pocoResponse?.Items?.Tracks ?? new List<Track>();
                }
            }

            return songs;
        }

        public async Task<bool> AddTrackToPlaylist(Track selectedTrack)
        {
            var token = SettingsManager.GetSerialized<SpotifyServiceResponse>("Spotify-Data");
            var result = false;
            var id = await GetPlaylistId(token);

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(token.UserId))
            {
                using (var client = new HttpClient())
                {
                    token = await RefreshToken(token);
                    client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", $"Bearer {token.AccessToken}");

                    var url = string.Format(PLAYLIST_ADD, token.UserId, id);
                    url = string.Concat(url, $"?uris=spotify:track:{selectedTrack.TrackId}");

                    var response = await client.PostAsync(new Uri(url, UriKind.Absolute), new HttpStringContent(string.Empty));
                    result = response.IsSuccessStatusCode;
                }
            }

            return result;
        }

        private string ParseAuthCode(string authStringResult, string checkString)
        {
            var authCode = string.Empty;

            var resultAuthUri = new Uri(authStringResult);
            var queryDictionary = from q in resultAuthUri.Query.Substring(1).Split('&').ToList()
                                  select new KeyValuePair<string, string>(q.Split('=')[0], q.Split('=')[1]);

            var serviceCheckString = queryDictionary.FirstOrDefault(x => x.Key.ToLower().Equals("state"));
            var queryToken = queryDictionary.FirstOrDefault(x => x.Key.ToLower().Equals("code"));

            if (serviceCheckString.Value.ToLower().Equals(checkString))
            {
                authCode = queryToken.Value;
            }

            return authCode;
        }

        private async Task<string> GetUserId(SpotifyServiceResponse token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", $"{token.TokenType} {token.AccessToken}");
                var profile = await client.GetAsync(new Uri(ME, UriKind.Absolute));
                return (await profile.Content.ReadAs<SpotifyProfileResponse>())?.UserId;
            }
        }

        private async Task<SpotifyServiceResponse> RefreshToken(SpotifyServiceResponse oldToken)
        {
            SpotifyServiceResponse response = oldToken;
            var renew = oldToken.FetechedOn.AddSeconds(oldToken.ExpiresInSeconds).ToUniversalTime() < DateTime.UtcNow;

            if (!string.IsNullOrEmpty(oldToken.RefreshToken) && renew)
            {
                using (var client = new HttpClient())
                {
                    var stringArray = Encoding.UTF8.GetBytes($"{APP_ID}:{APP_SC}");
                    var header = $"Basic {Convert.ToBase64String(stringArray)}";

                    var postEncodedParam = new HttpFormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string,string>("grant_type", "refresh_token"),
                        new KeyValuePair<string,string>("refresh_token", oldToken.RefreshToken)
                    });

                    client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", header);
                    var request = await client.PostAsync(new Uri(TOKEN_URL), postEncodedParam);
                    if (request.IsSuccessStatusCode)
                    {
                        var newToken = await request.Content.ReadAs<SpotifyServiceResponse>();
                        response.FetechedOn = DateTime.UtcNow;
                        response.AccessToken = newToken.AccessToken;
                        response.RefreshToken = newToken.RefreshToken;
                        response.ExpiresInSeconds = newToken.ExpiresInSeconds;
                        SettingsManager.SetSerialized<SpotifyServiceResponse>(response, "Spotify-Data");
                    }
                }
            }

            return response;
        }

        private async Task<string> GetPlaylistId(SpotifyServiceResponse token)
        {
            var id = SettingsManager.Get<string>("AppPlaylist");
            var playlists = new List<Playlist>();

            if (token != null && id == null)
            {
                using (var client = new HttpClient())
                {
                    token = await RefreshToken(token);
                    client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", $"Bearer {token.AccessToken}");

                    var response = await client.GetAsync(new Uri(MY_PLAYLIST, UriKind.Absolute));
                    if (response.IsSuccessStatusCode)
                    {
                        var pocoResponse = await response.Content.ReadAs<SpotifyPlaylistResponse>();
                        if (pocoResponse != null && pocoResponse.Playlists != null) playlists.AddRange(pocoResponse.Playlists);

                        while (!string.IsNullOrEmpty(pocoResponse.NextUrl))
                        {
                            response = await client.GetAsync(new Uri(pocoResponse.NextUrl, UriKind.Absolute));
                            if (response.IsSuccessStatusCode)
                            {
                                pocoResponse = await response.Content.ReadAs<SpotifyPlaylistResponse>();
                                if (pocoResponse != null && pocoResponse.Playlists != null) playlists.AddRange(pocoResponse.Playlists);
                            }
                        }
                    }
                }

                id = playlists.FirstOrDefault(p => p.Name.ToLower().Equals("hearitup - playlist"))?.PlaylistId;
                if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(token.UserId))
                {
                    var createPlaylistUrl = string.Format(CREATE_URL, token.UserId);
                    var body = "{\"name\": \"hearItUp - Playlist\", \"public\" : false}";
                    using (var client = new HttpClient())
                    {
                        token = await RefreshToken(token);
                        client.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", $"Bearer {token.AccessToken}");

                        var response = await client.PostAsync(new Uri(createPlaylistUrl, UriKind.Absolute), new HttpStringContent(body));
                        if (response.IsSuccessStatusCode)
                        {
                            var pocoResponse = await response.Content.ReadAs<Playlist>();
                            id = pocoResponse?.PlaylistId;
                        }
                    }
                }

                SettingsManager.Set(id, "AppPlaylist");
            }

            return id;
        }
    }
}
