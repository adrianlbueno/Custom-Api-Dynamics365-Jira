using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Status
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("iconUrl")]
        public string IconUrl { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("statusCategory")]
        public StatusCategory StatusCategory { get; set; }
    }
}
