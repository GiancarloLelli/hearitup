using System.Runtime.Serialization;

namespace HIU.Models.Bing
{
    [DataContract]
    public class BingImage
    {
        [DataMember(Name = "thumbnailUrl")]
        public string Url { get; set; }
    }
}
