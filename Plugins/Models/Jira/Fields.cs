using System.Runtime.Serialization;

namespace Plugins.Models.Jira
{
    [DataContract]
    public class Fields
    {
        [DataMember(Name = "customFields")]
        public CustomFields CustomFields{ get; set; }
        
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "status")]
        public Status Status{ get; set; }
    }
}
