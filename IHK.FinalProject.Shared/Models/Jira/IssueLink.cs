using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class IssueLink
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("type")]
        public IssueLinkType Type { get; set; }

        [JsonPropertyName("outwardIssue")]
        public OutwardIssue OutwardIssue { get; set; }
    }
}
