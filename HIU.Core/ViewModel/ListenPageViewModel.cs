using CortanaMusicSearch;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using HIU.Core.Common;
using HIU.Core.Contract;
using HIU.Core.Messages;
using HIU.Core.Service;
using HIU.Data.Service;
using HIU.Models.Cortana;
using HIU.Models.Spotify;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HIU.Core.ViewModel
{
	public class ListenPageViewModel : ViewModelBase, INavigable
	{
		readonly IMusicRecordRepository m_music;
		readonly INavigationService m_navigation;

		private CortanaServiceResponse m_response;
		private bool m_covers;
		private bool m_tracks;
		private bool m_loading;
		private bool m_tryagain;
		private Track m_selected;

		public ClientInfo ClientInfo { get; set; }

		public AudioFormat Audio { get; set; }

		public bool ContinueAnimate { get; set; }

		public Match Discovery
		{
			get
			{
				return m_response?.Items?.FirstOrDefault() ?? new Match();
			}
		}

		public IContinuousStreamRecognitionSession StreamRecognitionSession { get; set; }

		public IRecognitionSession RecognitionSession { get; set; }

		public ObservableCollection<Track> SpotifyTracks { get; set; }

		public RelayCommand SaveCommand { get; set; }

		public Track SelectedTrack
		{
			get { return m_selected; }
			set
			{
				Set(() => SelectedTrack, ref m_selected, value);
				SaveCommand.RaiseCanExecuteChanged();
			}
		}

		public bool AreCoversAvailable
		{
			get { return m_covers; }
			set { Set(() => AreCoversAvailable, ref m_covers, value); }
		}

		public bool AreTracksAvailable
		{
			get { return m_tracks; }
			set { Set(() => AreTracksAvailable, ref m_tracks, value); }
		}

		public bool IsLoading
		{
			get { return m_loading; }
			set { Set(() => IsLoading, ref m_loading, value); }
		}

		public bool TryAgainVisible
		{
			get { return m_tryagain; }
			set { Set(() => TryAgainVisible, ref m_tryagain, value); }
		}

		public ListenPageViewModel(INavigationService nav, IMusicRecordRepository music)
		{
			m_music = music;
			m_navigation = nav;

			ContinueAnimate = true;
			AreTracksAvailable = true;
			AreCoversAvailable = true;
			SpotifyTracks = new ObservableCollection<Track>();
			SaveCommand = new RelayCommand(async () => await SaveSongToDatabase(), () => SelectedTrack != null);

			ClientInfo = new ClientInfo
			{
				AppId = "cortana",
				AppVersion = "0",
				DeviceId = "4006787494004f8cb150a7c0d6ab1d23",
				ConfigId = "0000",
				MarketId = "US",
				DesiredResultFormat = QueryResultFormat.Json
			};

			Audio = new AudioFormat(SampleType.Float32, Endianess.LittleEndian, 48000, 1);
			m_response = new CortanaServiceResponse();
		}

		public bool TryParseResult(string json)
		{
			var response = false;

			if (!string.IsNullOrEmpty(json))
			{
				m_response = JsonManager.Deserialize<CortanaServiceResponse>(json);
				response = m_response.Result.ToLower().Equals("match");
				DispatcherHelper.CheckBeginInvokeOnUI(async () =>
				{
					IsLoading = true;
					RaisePropertyChanged(() => Discovery);
					await LoadSpotifyTracks();
					IsLoading = false;
				});
			}

			return response;
		}

		public Task OnNavigateTo(object parameter) => Task.FromResult<object>(null);

		public Task OnNavigateFrom(object parameter)
		{
			ContinueAnimate = true;
			TryAgainVisible = false;
			SpotifyTracks.Clear();
			return Task.FromResult<object>(null);
		}

		private async Task LoadSpotifyTracks()
		{
			if (string.IsNullOrEmpty(Discovery.Properties.SongTitle))
			{
				AreTracksAvailable = false;
				return;
			}

			var spotifyService = new SpotifyService();
			var songs = await spotifyService.SearchTracks(Discovery.Properties.SongTitle);

			songs.ForEach((s) => SpotifyTracks.Add(s));
			if (SpotifyTracks.Count <= 0) AreTracksAvailable = false;
		}

		private async Task SaveSongToDatabase()
		{
			if (Discovery == null || SelectedTrack == null)
			{
				return;
			}

			IsLoading = true;

			var spotifyService = new SpotifyService();
			var musicRecordItem = Discovery.ToMusicRecordItem();

			musicRecordItem.SpotifyId = SelectedTrack.TrackId;
			musicRecordItem.ImageUrl = SelectedTrack.AlbumArt;
			musicRecordItem.AudioPreview = SelectedTrack.StreamingPreview;
			musicRecordItem.AddedToSpotify = await spotifyService.AddTrackToPlaylist(SelectedTrack);

			if (!musicRecordItem.AddedToSpotify)
				MessengerInstance.Send<ViewModelMessage>(new ViewModelMessage { Text = "Sorry, I wasn't able to add this track to your Spotify playlist" });

			await m_music.AddRecord(musicRecordItem);
			IsLoading = false;
			m_navigation.GoBack();
		}

		public Task Loaded(object parameter) => Task.FromResult<object>(null);
	}
}
