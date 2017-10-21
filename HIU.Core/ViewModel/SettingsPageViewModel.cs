using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using HIU.Core.Common;
using HIU.Core.Contract;
using HIU.Core.Messages;
using HIU.Core.Service;
using HIU.Models.Spotify;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Authentication.Web;

namespace HIU.Core.ViewModel
{
    public class SettingsPageViewModel : ViewModelBase, INavigable
    {
        readonly INavigationService m_navigation;

        private bool m_shouldDisconnect;
        private string m_label;

        public string ButtonLabel
        {
            get { return m_label; }
            set
            {
                Set(() => ButtonLabel, ref m_label, value, true);
            }
        }

        public string Version
        {
            get
            {
                return $"Version: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Revision}";
            }
        }

        public string Author
        {
            get
            {
                return $"Author: {Package.Current.PublisherDisplayName}";
            }
        }

        public string App
        {
            get
            {
                return $"Name: {Package.Current.DisplayName}";
            }
        }

        public Uri About
        {
            get
            {
                return new Uri("https://it.linkedin.com/in/giancarlolelli/en", UriKind.Absolute);
            }
        }

        public Uri Privacy
        {
            get
            {
                return new Uri("http://www.iubenda.com/privacy-policy/558569", UriKind.Absolute);
            }
        }

        public SettingsPageViewModel(INavigationService navigation)
        {
            m_navigation = navigation;
        }

        public Task OnNavigateFrom(object parameter)
        {
            return Task.FromResult<object>(null);
        }

        public Task OnNavigateTo(object parameter)
        {
            var checkSpotifySettings = HasValidSpotifySettingsStored();
            ButtonLabel = checkSpotifySettings ? "Disconnect" : "Connect";
            m_shouldDisconnect = checkSpotifySettings;
            return Task.FromResult<object>(null);
        }

        public async void HandleAuth()
        {
            if (m_shouldDisconnect)
            {
                Logout();
            }
            else
            {
                await Login();
            }
        }

        private async Task Login()
        {
            bool showError = true;
            var service = new SpotifyService();
            var spotifyAuthSettings = service.GetAuthSettings();
            var authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, spotifyAuthSettings.Url, service.GetCallbackUri());

            if (authResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var authResponse = await service.FetchTokens(authResult.ResponseData, spotifyAuthSettings.Check);
                if (authResponse != null)
                {
                    SettingsManager.SetSerialized<SpotifyServiceResponse>(authResponse, "Spotify-Data");
                    ButtonLabel = "Disconnect";
                    showError = false;
                    m_shouldDisconnect = true;
                }
            }

            if (showError)
            {
                MessengerInstance.Send(new ViewModelMessage { Text = "Oops, something went wrong in the authentication!" });
            }
        }

        private void Logout()
        {
            SettingsManager.Remove("Spotify-Data");
            ButtonLabel = "Connect";
            m_shouldDisconnect = false;
        }

        private bool HasValidSpotifySettingsStored() => SettingsManager.Exist("Spotify-Data");
    }
}
