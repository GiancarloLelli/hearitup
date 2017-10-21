using System;
using System.Runtime.Serialization;

namespace HIU.Models.Repository
{
    [DataContract]
    public class MusicRecordItem
    {
        public MusicRecordItem()
        {
            AddedToSpotify = true;
        }

        public MusicRecordItem(bool nullable)
        {
            IsNull = nullable;
            AddedToSpotify = true;
        }

        // Plumbing data member
        public int Id { get; set; }
        public DateTime DiscoveredOn { get; set; }
        public bool IsNull { get; set; }
        public bool AddedToSpotify { get; set; }

        public string MicrosoftId { get; set; }
        public string Domain { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string SecondsOffset { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyId { get; set; }

        // Properties type wrapper
        public string SongType { get; set; }
        public string SongTitle { get; set; }
        public string ArtistName { get; set; }
        public string ReleaseTitle { get; set; }
        public string AudioPreview { get; set; }
    }
}
