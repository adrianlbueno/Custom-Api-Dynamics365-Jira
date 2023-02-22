using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class IssueLinkType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("inward")]
        public string Inward { get; set; }
        
        [JsonPropertyName("outward")]
        public string Outward { get; set; }
        
        [JsonPropertyName("self")]
        public string Self { get; set; }
    }
}
