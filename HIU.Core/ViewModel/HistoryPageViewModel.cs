using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using HIU.Core.Contract;
using HIU.Data.Service;
using HIU.Models.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace HIU.Core.ViewModel
{
    public class HistoryPageViewModel : ViewModelBase, INavigable
    {
        readonly INavigationService m_navigation;
        readonly IMusicRecordRepository m_service;

        public MusicRecordItem SelectedRecord { get; set; }

        public ObservableCollection<MusicRecordItem> Records { get; set; }

        public RelayCommand<string> LeftSwipe { get; set; }


        public HistoryPageViewModel(INavigationService navigation, IMusicRecordRepository service)
        {
            m_navigation = navigation;
            m_service = service;
            Records = new ObservableCollection<MusicRecordItem>();
            LeftSwipe = new RelayCommand<string>((id) => LeftSwipeAction(id));
        }

        public Task OnNavigateFrom(object parameter)
        {
            Records.Clear();
            return Task.FromResult<object>(null);
        }

        public Task OnNavigateTo(object parameter)
        {
            var records = m_service?.GetMusicRecords(0).OrderByDescending(m => m.DiscoveredOn);
            foreach (var record in records.Skip(1)) Records.Add(record);
            return Task.FromResult<object>(null);
        }

        public void LeftSwipeAction(string id)
        {
            var item = m_service.FindRecord(id);
            var inMemory = Records.Where(x => x.MicrosoftId == id).FirstOrDefault();
            if (item != null) m_service.DeleteRecord(item);
            if (inMemory != null) Records.Remove(inMemory);
        }

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

        public void OpenListRecordDetail() => OpenRecordDetail(SelectedRecord, true);
    }
}
