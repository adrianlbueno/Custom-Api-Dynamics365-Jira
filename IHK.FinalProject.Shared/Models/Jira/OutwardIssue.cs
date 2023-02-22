using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class OutwardIssue
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }
        
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("fields")]
        public OutwardIssueFields Fields { get; set; }
    }
}
