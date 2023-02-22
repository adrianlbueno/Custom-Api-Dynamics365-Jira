using Plugins.Helper;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plugins.Models
{
    [DataContract]

    public class ApiResponse  {

        [DataMember]
        public bool Successful { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
