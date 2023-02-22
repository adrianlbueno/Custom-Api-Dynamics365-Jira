using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class OutwardIssueFields
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
        
        [JsonPropertyName("status")]
        public Status Status { get; set; }
        
        [JsonPropertyName("priority")]
        public Priority Priority { get; set; }
        
        [JsonPropertyName("issuetype")]
        public IssueType Issuetype { get; set; }
    }
}
