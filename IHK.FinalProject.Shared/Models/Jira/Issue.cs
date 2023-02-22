using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Issue
    {
        [JsonPropertyName("expand")]
        public string Expand { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("fields")]
        public Fields Fields { get; set; }

        [JsonPropertyName("name")]
        public string Name{ get; set; }
    }
}
