using CortanaMusicSearch;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using HIU.Core.Messages;
using HIU.Core.ViewModel;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using Windows.UI.Xaml.Navigation;

namespace HIU.App.Views
{
    public sealed partial class ListenPage : ViewBase
    {
        public ListenPageViewModel ViewModel { get; set; }

        public ListenPage()
        {
            InitializeComponent();
            ViewModel = DataContext as ListenPageViewModel;
            Loaded += ListenPageLoaded;
        }

        private void Animate()
        {
            foreach (var children in DiscoveryGrid.Children)
            {
                var lastElement = DiscoveryGrid.Children.IndexOf(children) == (DiscoveryGrid.Children.Count - 1);
                var animation = children.Scale(1.25f, 1.25f, 0.5f, 0.5f, 1000, 0).Then()
                                        .Scale(1, 1, 0.5f, 0.5f, 1000, 0);

                if (lastElement && ViewModel.ContinueAnimate)
                    animation.Completed += Animation_Completed;

                animation.Start();
            }
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            var animationSender = (sender as AnimationSet);
            animationSender.Completed -= Animation_Completed;
            Animate();
        }

        private async void ListenPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.TryAgainVisible = false;
            ViewModel.ContinueAnimate = true;

            var audioRecognitionClient = AudioRecognitionClient.CreateInstance();
            var sessionFactory = await audioRecognitionClient.CreateRecoSessionFactoryAsync(ViewModel.ClientInfo);

            ViewModel.RecognitionSession = await sessionFactory.CreateMicrophoneInputRecognitionSession();
            ViewModel.StreamRecognitionSession = sessionFactory.CreateContinuousStreamRecognitionSession(ViewModel.Audio);
            ViewModel.StreamRecognitionSession.Start();

            ViewModel.RecognitionSession.StatusChangedEvent += RecognitionSession_StatusChangedEvent;
            ViewModel.RecognitionSession.Start();

            Animate();
        }

        private void RecognitionSession_StatusChangedEvent(RecognitionStatus status)
        {
            if (status == RecognitionStatus.Match)
            {
                var resultJson = ViewModel.RecognitionSession.GetQueryResult();
                if (ViewModel.TryParseResult(resultJson))
                {
                    ViewModel.ContinueAnimate = false;
                    ViewModel.StreamRecognitionSession.Abort();
                    ViewModel.RecognitionSession.StatusChangedEvent -= RecognitionSession_StatusChangedEvent;
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ResultGrid.Offset((float)-MainGridControl.ActualWidth, 0, 1000, 0).Start());
                }
            }
            else if (status == RecognitionStatus.NoMatch)
            {
                ViewModel.ContinueAnimate = false;
                ViewModel.RecognitionSession.StatusChangedEvent -= RecognitionSession_StatusChangedEvent;
                Messenger.Default.Send(new ViewModelMessage { Text = "Sorry, We were unable to recognise this song!" });
                DispatcherHelper.CheckBeginInvokeOnUI(() => ViewModel.TryAgainVisible = true);
            }
            else if (status == RecognitionStatus.FailedDueToError || status == RecognitionStatus.AbortedByUser)
            {
                // TODO: Add telemetry
                return;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= ListenPageLoaded;
            base.OnNavigatedFrom(e);
        }

        private void TryAgaianClickEventHandler(object sender, Windows.UI.Xaml.RoutedEventArgs e) => ListenPageLoaded(sender, e);
    }
}
