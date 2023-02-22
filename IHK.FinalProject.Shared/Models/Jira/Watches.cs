using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Watches
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("watchCount")]
        public int WatchCount { get; set; }
        
        [JsonPropertyName("isWatching")]
        public bool IsWatching { get; set; }
    }
}
