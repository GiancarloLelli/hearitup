using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using HIU.Core.Common;
using HIU.Core.Contract;
using HIU.Core.Messages;
using HIU.Core.Service;
using HIU.Data.Service;
using HIU.Models.Repository;
using HIU.Models.Spotify;
using HIU.Models.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Security.Authentication.Web;
using Windows.System;

namespace HIU.Core.ViewModel
{
	public class MainPageViewModel : ViewModelBase, INavigable
	{
		private MusicRecordItem m_last;
		private bool m_hasHistory;

		readonly INavigationService m_navigation;
		readonly IMusicRecordRepository m_service;

		public MusicRecordItem LastDiscovery
		{
			get { return m_last; }
			private set
			{
				Set("LastDiscovery", ref m_last, value);
			}
		}

		public MusicRecordItem SelectedRecord { get; set; }

		public bool HasHistory
		{
			get { return m_hasHistory; }
			private set
			{
				Set("HasHistory", ref m_hasHistory, value);
			}
		}

		public ObservableCollection<MusicRecordItem> Records { get; set; }

		public ObservableCollection<HamburgerMenuItem> MenuItems { get; set; }

		public MainPageViewModel(INavigationService nav, IMusicRecordRepository service)
		{
			m_navigation = nav;
			m_service = service;
			Records = new ObservableCollection<MusicRecordItem>();
			MenuItems = new ObservableCollection<HamburgerMenuItem>();
		}

		public Task OnNavigateFrom(object parameter)
		{
			Records.Clear();
			MenuItems.Clear();
			return Task.FromResult<object>(null);
		}

		public Task OnNavigateTo(object parameter)
		{
			// Max query take
			var display = DisplayInformation.GetForCurrentView().DiagonalSizeInInches;
			var take = display > 6 ? 6 : (display > 5 && display < 6 ? 4 : 3);

			// Records
			var records = m_service?.GetMusicRecords(take).OrderByDescending(m => m.DiscoveredOn);
			LastDiscovery = records.FirstOrDefault() ?? new MusicRecordItem(true);
			HasHistory = !LastDiscovery.IsNull && records.Count() > 1;
			foreach (var record in records.Skip(1)) Records.Add(record);

			// Internet connection check
			var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
			if (!isNetworkAvailable)
			{
				MessengerInstance.Send<ViewModelMessage>(new ViewModelMessage { Text = "No internet connection detected, some featured may be disabled." });
			}

			return Task.FromResult<object>(null);
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
					showError = false;
				}
			}

			if (showError)
			{
				MessengerInstance.Send(new ViewModelMessage { Text = "Oops, something went wrong in the authentication!" });
			}
		}

		public async void GoToListenPage()
		{
			var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
			if (isNetworkAvailable && !HasValidSpotifySettingsStored())
			{
				await Login();
			}

			if (isNetworkAvailable && HasValidSpotifySettingsStored())
			{
				m_navigation.NavigateTo("ListenPage");
			}
			else if (!isNetworkAvailable && HasValidSpotifySettingsStored())
			{
				MessengerInstance.Send(new ViewModelMessage { Text = "No Internet connection detected!" });
			}
		}

		public Task Loaded(object parameter) => Task.FromResult<object>(null);

		public void OpenRecordDetail(MusicRecordItem record, bool reset)
		{
			if (record == null) return;
			Task.Run(async () => await Launcher.LaunchUriAsync(new Uri(record.Url, UriKind.Absolute)));

			if (reset)
			{
				SelectedRecord = null;
				RaisePropertyChanged(() => SelectedRecord);
			}
		}

		public void OpenLastDiscoveryDetail() => OpenRecordDetail(LastDiscovery, false);

		public void OpenListRecordDetail() => OpenRecordDetail(SelectedRecord, true);

		private void Logout() => SettingsManager.Remove("Spotify-Data");

		private bool HasValidSpotifySettingsStored() => SettingsManager.Exist("Spotify-Data");

		public void LoadMoreFromHistory() => m_navigation.NavigateTo("HistoryPage");
	}
}
