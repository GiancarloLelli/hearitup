using HIU.Data.Service;
using HIU.Models.Cortana;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace HIU.Core.Common
{
    public class DebugSeeder
    {
        public static async Task Seed(IMusicRecordRepository svc)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///HIU.Data/SampleData/Result.json", UriKind.Absolute));
            using (var stream = await file.OpenStreamForReadAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    var debugRecords = JsonManager.Deserialize<CortanaServiceResponse>(json);
                    foreach (var item in debugRecords.Items)
                    {
                        svc?.AddRecord(item.ToMusicRecordItem());
                    }
                }
            }
        }
    }
}
