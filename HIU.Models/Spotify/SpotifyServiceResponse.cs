using System;
using System.Runtime.Serialization;

namespace HIU.Models.Spotify
{
    [DataContract]
    public class SpotifyServiceResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresInSeconds { get; set; }

        [DataMember(Name = "fetched_on")]
        public DateTime FetechedOn { get; set; }

        [DataMember(Name = "u_id")]
        public string UserId { get; set; }

        [DataMember(Name = "auth_code")]
        public string AuthCode { get; set; }
    }
}
