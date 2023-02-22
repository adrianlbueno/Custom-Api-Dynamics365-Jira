using System.Text.Json.Serialization;


namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Person
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("name")]
        public string Name{ get; set; }

        [JsonPropertyName("key")]
        public string Key{ get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress{ get; set; }

        [JsonPropertyName("avartUrls")]
        public string AvartarUrls{ get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName{ get; set; }

        [JsonPropertyName("active")]
        public bool Active{ get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone{ get; set; }
    }
}
