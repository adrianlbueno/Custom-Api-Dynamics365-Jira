using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class StatusCategory
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }
        
        [JsonPropertyName("colorName")]
        public string ColorName { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}