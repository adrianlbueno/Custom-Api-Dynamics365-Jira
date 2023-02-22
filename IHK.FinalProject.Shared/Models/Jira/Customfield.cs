using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jiras
{
    public class Customfield
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
    }
}