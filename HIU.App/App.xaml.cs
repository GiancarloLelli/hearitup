using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using HIU.App.Views;
using HIU.Core.Messages;
using HIU.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HIU.App
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            using (var db = new MusicRecordContext())
            {
                db.Database.Migrate();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached) DebugSettings.EnableFrameRateCounter = false;
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }

            Messenger.Default.Unregister<ViewModelMessage>(this);
            Messenger.Default.Register<ViewModelMessage>(this, async (message) => await (new MessageDialog(message.Text, "The app says...")).ShowAsync());

            DispatcherHelper.Initialize();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
