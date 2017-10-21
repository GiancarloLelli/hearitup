using HIU.Core.Common;
using HIU.Models.Slack;

namespace HIU.Core.Extensions
{
    public static class SlackPayloadExtensions
    {

        public static string Serialize(this SlackPayload target)
        {
            return JsonManager.Serialize(target);
        }
    }
}
