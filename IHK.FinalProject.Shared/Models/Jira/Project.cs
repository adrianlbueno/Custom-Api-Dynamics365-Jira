using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Project
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("projectTypeKey")]
        public string ProjectTypeKey { get; set; }
        
        [JsonPropertyName("avatarUrls")]
        public Dictionary<string, string> AvatarUrls { get; set; }
    }
}
