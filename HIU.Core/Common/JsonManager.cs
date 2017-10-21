using BingAudio;
using HIU.Models.Cortana;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace HIU.Core.Common
{
    public class JsonManager
    {
        public static T Deserialize<T>(string json) where T : class
        {
            try
            {
                var types = new List<Type> { typeof(Match), typeof(ScalingTransform), typeof(Song) };
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T), types);
                using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    T result = (T)deserializer.ReadObject(stream);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Serialize<T>(T settingsObject) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, settingsObject);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
