using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HIU.Models.Spotify
{
    [DataContract]
    public class SpotifySearchResponse
    {
        [DataMember(Name = "tracks")]
        public TrackList Items { get; set; }
    }

    [DataContract]
    public class TrackList
    {
        [DataMember(Name = "items")]
        public List<Track> Tracks { get; set; }
    }

    [DataContract]
    public class Artist
    {
        [DataMember(Name = "id")]
        public string ArtistId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Image
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class Album
    {
        [DataMember(Name = "id")]
        public string AlbumId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "artists")]
        public List<Artist> Artists { get; set; }

        [DataMember(Name = "images")]
        public List<Image> Images { get; set; }
    }

    [DataContract]
    public class Track
    {
        [DataMember(Name = "id")]
        public string TrackId { get; set; }

        [DataMember(Name = "name")]
        public string TrackName { get; set; }

        [DataMember(Name = "album")]
        public Album Album { get; set; }

        [DataMember(Name = "preview_url")]
        public string StreamingPreview { get; set; }

        [IgnoreDataMember]
        public string AlbumArtist
        {
            get
            {
                return Album?.Artists?.FirstOrDefault()?.Name ?? string.Empty;
            }
        }

        [IgnoreDataMember]
        public string AlbumArt
        {
            get
            {
                return Album?.Images?.FirstOrDefault()?.Url ?? string.Empty;
            }
        }

        [IgnoreDataMember]
        public string AlbumName
        {
            get
            {
                return Album?.Name ?? string.Empty;
            }
        }
    }
}
