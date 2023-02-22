using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class TransitionContainer
    {
        [JsonPropertyName("transition")]
        public Transition Transition { get; set; }
    }
}
