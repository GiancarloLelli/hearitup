using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HIU.Models.Spotify
{
    [DataContract]
    public class SpotifyPlaylistResponse
    {
        [DataMember(Name = "items")]
        public List<Playlist> Playlists { get; set; }

        [DataMember(Name = "next")]
        public string NextUrl { get; set; }
    }

    [DataContract]
    public class Playlist
    {
        [DataMember(Name = "id")]
        public string PlaylistId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
