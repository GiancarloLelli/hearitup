using HIU.Core.Common;
using HIU.Core.Extensions;
using HIU.Models.Bing;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Web.Http;
using System.Threading.Tasks;

namespace HIU.Core.Service
{
    public class BingService
    {
        private const string API_KEY = "b3cc2186e24e4861a1456a39074f9522";

        public async Task<List<BingImage>> SearchCoverArts(string album, string artist)
        {
            var arts = new List<BingImage>();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", API_KEY);

            var queryDictionary = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("q", $"{artist} {album}"),
                new KeyValuePair<string, string>("count", "12"),
                new KeyValuePair<string, string>("mkt", CultureInfo.CurrentCulture.Name.ToLower()),
                new KeyValuePair<string, string>("safeSearch", "Moderate")
            };

            var searchUrl = string.Concat("https://api.cognitive.microsoft.com/bing/v5.0/images/search?", queryDictionary.ToQueryString());
            var response = await client.GetAsync(new Uri(searchUrl, UriKind.Absolute));
            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                var pocoResponse = JsonManager.Deserialize<BingImageResult>(responseText);
                arts = pocoResponse.Images ?? new List<BingImage>();
            }

            return arts;
        }
    }
}
