using HIU.Data.Context;
using HIU.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIU.Data.Service
{
    public class MusicRecordRepository : IMusicRecordRepository
    {
        public async Task AddRecord(MusicRecordItem item)
        {
            try
            {
                using (var db = new MusicRecordContext())
                {
                    db.Records.Add(item);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteRecord(MusicRecordItem item)
        {
            bool success = true;

            try
            {
                using (var db = new MusicRecordContext())
                {
                    success = db.Records.Remove(item) != null;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return success;
        }

        public MusicRecordItem FindRecord(string key)
        {
            MusicRecordItem record = null;

            try
            {
                using (var db = new MusicRecordContext())
                {
                    record = db.Records.FirstOrDefault(s => s.MicrosoftId == key);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return record;
        }

        public List<MusicRecordItem> GetMusicRecords(int take = 0)
        {
            var list = new List<MusicRecordItem>();

            try
            {
                using (var db = new MusicRecordContext())
                {
                    if (take == 0)
                    {
                        list = db.Records.ToList();
                    }
                    else
                    {
                        list = db.Records.Take(take).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }
    }
}
