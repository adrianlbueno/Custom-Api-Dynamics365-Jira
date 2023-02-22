using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Transition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}