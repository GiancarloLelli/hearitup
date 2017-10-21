using HIU.Core.ViewModel;

namespace HIU.App.Views
{
    public sealed partial class HistoryPage : ViewBase
    {
        public HistoryPageViewModel ViewModel { get; set; }

        public HistoryPage()
        {
            InitializeComponent();
            ViewModel = DataContext as HistoryPageViewModel;
        }
    }
}
