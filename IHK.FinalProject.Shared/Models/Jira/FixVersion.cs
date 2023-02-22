using System;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class FixVersion
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("name")]
        public string Name{ get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("released")]
        public bool Released { get; set; }

        [JsonPropertyName("releasedDate")]
        public DateTimeOffset ReleaseDate{ get; set; }
    }
}
