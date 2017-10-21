using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using HIU.App.Views;
using HIU.Core.ViewModel;
using HIU.Data.Service;
using Microsoft.Practices.ServiceLocation;

namespace HIU.App.Bootstrap
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<INavigationService>()) SimpleIoc.Default.Register<INavigationService>(() => BuildNavigationService());
            if (!SimpleIoc.Default.IsRegistered<IMusicRecordRepository>()) SimpleIoc.Default.Register<IMusicRecordRepository>(() => new MusicRecordRepository());

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<HistoryPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<ContactPageViewModel>();
            SimpleIoc.Default.Register<ListenPageViewModel>();
        }

        private NavigationService BuildNavigationService()
        {
            var nav = new NavigationService();
            nav.Configure(Pages.Main, typeof(MainPage));
            nav.Configure(Pages.History, typeof(HistoryPage));
            nav.Configure(Pages.Settings, typeof(SettingsPage));
            nav.Configure(Pages.Contact, typeof(ContactPage));
            nav.Configure(Pages.Listen, typeof(ListenPage));
            return nav;
        }

        public MainPageViewModel MainPage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        public HistoryPageViewModel HistoryPage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistoryPageViewModel>();
            }
        }

        public SettingsPageViewModel SettingsPage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
            }
        }

        public ContactPageViewModel ContactPage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ContactPageViewModel>();
            }
        }

        public ListenPageViewModel ListenPage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListenPageViewModel>();
            }
        }
    }

    public class Pages
    {
        public const string Main = nameof(MainPage);
        public const string History = nameof(HistoryPage);
        public const string Settings = nameof(SettingsPage);
        public const string Contact = nameof(ContactPage);
        public const string Listen = nameof(ListenPage);
    }
}
