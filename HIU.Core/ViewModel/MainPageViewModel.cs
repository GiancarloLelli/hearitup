using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using HIU.Core.Contract;
using HIU.Core.Messages;
using HIU.Data.Service;
using HIU.Models.Repository;
using HIU.Models.UI;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Xaml.Controls;

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

            // Menu items
            MenuItems.Add(new HamburgerMenuItem { Label = "Settings", Gliph = new SymbolIcon(Symbol.Setting) });
            MenuItems.Add(new HamburgerMenuItem { Label = "Feedback", Gliph = new SymbolIcon(Symbol.Emoji2) });
            MenuItems.Add(new HamburgerMenuItem { Label = "Contact Us", Gliph = new SymbolIcon(Symbol.Mail) });

            // Internet connection check
            var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (!isNetworkAvailable) MessengerInstance.Send<ViewModelMessage>(new ViewModelMessage { Text = "No internet connection detected, some featured may be disabled." });

            return Task.FromResult<object>(null);
        }

        public async void Navigation(object sender, ItemClickEventArgs e)
        {
            var hamburgerMenuItem = e.ClickedItem as HamburgerMenuItem;
            if (hamburgerMenuItem == null) return;
            var isFeedback = hamburgerMenuItem.Label.Equals("Feedback") ? true : false;
            var isContactUs = hamburgerMenuItem.Label.Equals("Contact Us") ? true : false;

            if (isFeedback)
            {
                var feedbackSupported = StoreServicesFeedbackLauncher.IsSupported();
                if (feedbackSupported) await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
                else
                {
                    await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Package.Current.Id.ProductId}", UriKind.Absolute));
                }
            }
            else if (isContactUs)
            {
                m_navigation.NavigateTo("ContactPage");
            }
            else
            {
                m_navigation.NavigateTo("SettingsPage");
            }
        }

        public void LoadMoreFromHistory() => m_navigation.NavigateTo("HistoryPage");

        public void GoToListenPage() => m_navigation.NavigateTo("ListenPage");

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
    }
}
