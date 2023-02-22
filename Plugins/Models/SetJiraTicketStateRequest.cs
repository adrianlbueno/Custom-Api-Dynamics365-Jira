using System.Runtime.Serialization;

namespace Plugins.Models
{
    [DataContract]
    public class SetJiraTicketStateRequest
    {
        [DataMember(Name = "jiraurl")]
        public string JiraUrl { get; set; }

        [DataMember(Name = "ticketkey")]
        public string TicketKey { get; set; }

        [DataMember(Name ="status")]
        public int Status { get; set; }

        public SetJiraTicketStateRequest(string jiraUrl, string ticketKey, int status)
        {
            JiraUrl = jiraUrl;
            TicketKey = ticketKey;
            Status = status;
        }
    }
}
