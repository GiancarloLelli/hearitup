using HIU.Models.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIU.Data.Service
{
    public interface IMusicRecordRepository
    {
        Task AddRecord(MusicRecordItem item);
        Task<bool> DeleteRecord(MusicRecordItem item);
        MusicRecordItem FindRecord(string key);
        List<MusicRecordItem> GetMusicRecords(int take = 0);
    }
}