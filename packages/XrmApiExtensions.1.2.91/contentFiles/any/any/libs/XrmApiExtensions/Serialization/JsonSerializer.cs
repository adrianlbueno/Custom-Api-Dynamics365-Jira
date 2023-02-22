using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon.Core.Serialization
{
    public class JsonSerializer<T> where T : class
    {
        DataContractJsonSerializer serializer = null;

        public JsonSerializer()
        {
            serializer = new DataContractJsonSerializer(typeof(T));
        }

        public string Serialize(T item)
        {
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, item);
                stream.Position = 0;
                return new StreamReader(stream).ReadToEnd();
            }
        }

        public T Deserialize(String json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return serializer.ReadObject(stream) as T;
            }
        }
    }
}
