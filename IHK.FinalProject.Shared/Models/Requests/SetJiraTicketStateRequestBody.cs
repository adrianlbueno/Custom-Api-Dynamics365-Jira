using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira.Requests
{
    public class SetJiraTicketStateRequestBody
    {
        [JsonPropertyName("jiraurl")]
        public string JiraUrl { get; set; }

        [JsonPropertyName("ticketkey")]
        public string TicketKey { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
