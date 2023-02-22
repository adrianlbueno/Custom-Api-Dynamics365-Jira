using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Result
    {
        [JsonPropertyName("expand")]
        public string Expand { get; set; }
        
        [JsonPropertyName("startAt")]
        public int StartAt { get; set; }
        
        [JsonPropertyName("maxResults")]
        public int MaxResults { get; set; }
        
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
