using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class IssueResult : Result
    {
        [JsonPropertyName("issues")]
        public List<Issue> Issues { get; set; }
    }
}
