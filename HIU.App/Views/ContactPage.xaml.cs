using HIU.Core.ViewModel;

namespace HIU.App.Views
{
    public sealed partial class ContactPage : ViewBase
    {
        public ContactPageViewModel ViewModel { get; set; }

        public ContactPage()
        {
            InitializeComponent();
            ViewModel = DataContext as ContactPageViewModel;
        }
    }
}
