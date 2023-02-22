using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class ProgressContainer
    {
        [JsonPropertyName("progress")]
        public int Progress { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
