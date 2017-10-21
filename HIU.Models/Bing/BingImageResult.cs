using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HIU.Models.Bing
{
    [DataContract]
    public class BingImageResult
    {
        [DataMember(Name = "value")]
        public List<BingImage> Images { get; set; }
    }
}