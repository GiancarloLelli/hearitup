using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HIU.Models.Spotify
{
    [DataContract]
    public class SpotifyProfileResponse
    {
        [DataMember(Name = "id")]
        public string UserId { get; set; }
    }
}
