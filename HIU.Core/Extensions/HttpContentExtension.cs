using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace HIU.Core.Extensions
{
	public static class HttpContentExtension
	{
		public static async Task<T> ReadAs<T>(this IHttpContent Context)
		{
			var content = await Context.ReadAsStringAsync();
			DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
			using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(content)))
			{
				T result = (T)deserializer.ReadObject(stream);
				return result;
			}
		}
	}
}
