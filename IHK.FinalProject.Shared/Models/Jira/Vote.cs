using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Vote
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("votes")]
        public int Votes { get; set; }
        
        [JsonPropertyName("hasVoted")]
        public bool HasVoted { get; set; }
    }
}