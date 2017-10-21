using HIU.Core.ViewModel;

namespace HIU.App.Views
{
    public sealed partial class SettingsPage : ViewBase
    {
        public SettingsPageViewModel ViewModel { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = DataContext as SettingsPageViewModel;
        }
    }
}
