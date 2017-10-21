using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HIU.Core.Messages;
using HIU.Core.Service;
using HIU.Models.Slack;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Windows.ApplicationModel;

namespace HIU.Core.ViewModel
{
    public class ContactPageViewModel : ViewModelBase
    {
        private string m_body;
        private string m_subject;
        private string m_email;
        private bool m_enabled;

        public string Body
        {
            get { return m_body; }
            set
            {
                Set(() => Body, ref m_body, value);
                Send.RaiseCanExecuteChanged();
            }
        }

        public string Subject
        {
            get { return m_subject; }
            set { Set(() => Subject, ref m_subject, value); }
        }

        public string Email
        {
            get { return m_email; }
            set
            {
                Set(() => Email, ref m_email, value);
                Send.RaiseCanExecuteChanged();
            }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set { Set(() => Enabled, ref m_enabled, value); }
        }

        public RelayCommand Send { get; set; }

        public ContactPageViewModel()
        {
            Send = new RelayCommand(() => SendFeedback(), () =>
            {
                bool isInternetConnected = NetworkInterface.GetIsNetworkAvailable();
                return !string.IsNullOrEmpty(Body) && !string.IsNullOrEmpty(Subject) && !string.IsNullOrEmpty(Email) && isInternetConnected;
            });
        }

        private async void SendFeedback()
        {
            Enabled = true;
            var showErrorDialog = true;

            var validEmail = new EmailAddressAttribute().IsValid(Email);
            if (validEmail)
            {
                var service = new SlackService("https://hooks.slack.com/services/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");
                var version = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Revision}";

                SlackPayload slackPayload = new SlackPayload();
                var attachment = slackPayload.CreateAttachment(Body, Subject);
                attachment.AddField("App Version", version, true);
                attachment.AddField("From", Email, true);
                attachment.AddField("Telemetry ID", Guid.NewGuid().ToString(), false); // TODO Set Telemetry ID
                slackPayload.Attachments.Add(attachment);

                var sendResult = await service.SendMessage(slackPayload);
                if (sendResult) showErrorDialog = false;
            }

            if (showErrorDialog) MessengerInstance.Send(new ViewModelMessage { Text = "Oops, something went wrong! Make sure your internet connection is active." });
            else MessengerInstance.Send(new ViewModelMessage { Text = "Thanks for reaching out to us! We've recorded your feedback." });

            Enabled = false;
        }
    }
}
