using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Plugins.Helper
{
    public static class JSONSerializer
    {
        public static string Serialize<T>(T instance) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using(MemoryStream mmStream = new MemoryStream())
            {
                serializer.WriteObject(mmStream, instance);
                return Encoding.UTF8.GetString(mmStream.ToArray());
            }
        }

        public static T Deserialize<T>(string json) where T : class 
        {
            using (MemoryStream memory = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return serializer.ReadObject(memory) as T;
            }
        }
    }
}
