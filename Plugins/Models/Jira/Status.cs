using System.Runtime.Serialization;

namespace Plugins.Models.Jira
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "self")]
        public string Self { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}