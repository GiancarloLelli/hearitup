using HIU.Core.ViewModel;

namespace HIU.App.Views
{
    public sealed partial class MainPage : ViewBase
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = DataContext as MainPageViewModel;
        }
    }
}
