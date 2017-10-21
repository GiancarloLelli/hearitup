using BingAudio;
using HIU.Models.Repository;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HIU.Models.Cortana
{
    [DataContract]
    public class CortanaServiceResponse
    {
        public CortanaServiceResponse()
        {
            Items = new List<Match>();
        }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }

        [DataMember(Name = "matches")]
        public IEnumerable<Match> Items { get; set; }
    }

    [DataContract]
    public class Match
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "secondsOffset")]
        public float SecondsOffset { get; set; }

        [DataMember(Name = "properties")]
        public Song Properties { get; set; }

        [DataMember(Name = "transform")]
        public ScalingTransform Transform { get; set; }

        public MusicRecordItem ToMusicRecordItem()
        {
            return new MusicRecordItem
            {
                ArtistName = Properties.ArtistName,
                ReleaseTitle = Properties.ReleaseTitle,
                SongTitle = Properties.SongTitle,
                SongType = Properties.Type,
                DiscoveredOn = DateTime.Now,
                Domain = Domain,
                MicrosoftId = Id,
                SecondsOffset = SecondsOffset.ToString(),
                IsNull = false,
                Type = Type,
                Url = Url
            };
        }
    }
}

namespace BingAudio
{
    [DataContract(Namespace = "BingAudio", Name = "ScalingTransform")]
    public class ScalingTransform
    {
        [DataMember(Name = "__type")]
        public string Type { get; set; }

        [DataMember(Name = "inversePitchSpeedupPercentage")]
        public string InversePitchSpeedupPercentage { get; set; }

        [DataMember(Name = "inverseTempoSpeedupPercentage")]
        public string InverseTempoSpeedupPercentage { get; set; }
    }

    [DataContract(Namespace = "BingAudio", Name = "Song")]
    public class Song
    {
        [DataMember(Name = "__type")]
        public string Type { get; set; }

        [DataMember(Name = "songTitle")]
        public string SongTitle { get; set; }

        [DataMember(Name = "artistName")]
        public string ArtistName { get; set; }

        [DataMember(Name = "releaseTitle")]
        public string ReleaseTitle { get; set; }
    }
}