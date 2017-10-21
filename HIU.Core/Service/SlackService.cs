using HIU.Core.Extensions;
using HIU.Models.Slack;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIU.Core.Service
{
    public class SlackService
    {
        readonly string m_webhook;

        public SlackService(string url)
        {
            m_webhook = url;
        }

        public async Task<bool> SendMessage(SlackPayload payload)
        {
            var serializedPayload = payload.Serialize();
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(m_webhook, new StringContent(serializedPayload, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }
        }
    }
}
