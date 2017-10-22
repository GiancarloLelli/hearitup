using HIU.Core.Contract;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HIU.App.Views
{
	public partial class ViewBase : Page
	{
		readonly Frame m_rootFrame;

		public ViewBase()
		{
			InitializeComponent();
			HideApplicationBar().Wait();
			m_rootFrame = Window.Current.Content as Frame;
			Loaded += ViewBase_Loaded;
		}

		private void ViewBase_Loaded(object sender, RoutedEventArgs e)
		{
			var navigableViewModel = DataContext as INavigable;
			navigableViewModel?.Loaded(e);
		}

		private async Task HideApplicationBar()
		{
			if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
			{
				var statusBar = StatusBar.GetForCurrentView();
				await statusBar.HideAsync();
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var navigableViewModel = DataContext as INavigable;
			navigableViewModel?.OnNavigateTo(e.Parameter);

			if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
			{
				var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
				systemNavigationManager.BackRequested += BackRequested;
				if (m_rootFrame.CanGoBack) systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

			var navigableViewModel = DataContext as INavigable;
			navigableViewModel?.OnNavigateFrom(e.Parameter);

			if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
			{
				var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
				systemNavigationManager.BackRequested -= BackRequested;
			}
		}
		private void BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (m_rootFrame.CanGoBack)
			{
				m_rootFrame.GoBack();
				e.Handled = true;
			}
		}
	}
}
