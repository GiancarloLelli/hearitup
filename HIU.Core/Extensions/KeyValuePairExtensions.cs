using System.Collections.Generic;
using System.Text;

namespace HIU.Core.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static string ToQueryString(this KeyValuePair<string, string>[] query)
        {
            var builder = new StringBuilder();

            foreach (var item in query)
            {
                builder.Append($"{item.Key}={item.Value}&");
            }

            return builder.ToString();
        }
    }
}
