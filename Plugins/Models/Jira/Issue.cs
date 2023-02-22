using System.Runtime.Serialization;

namespace Plugins.Models.Jira
{
    [DataContract]
    public class Issue
    {
        [DataMember(Name = "expand")]
        public string Expand { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "self")]
        public string Self { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "fields")]
        public Fields Fields { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
