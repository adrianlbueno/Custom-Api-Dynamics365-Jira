using IHK.FinalProject.Shared.Models.Jira;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jira.Api
{
    public interface IJiraApi
    {
        [Get("/search?jql=issuetype=Request+And+cf[10904]~\"{ticketNumber}\"")]
        public Task<IssueResult> GetTicketsAsync(string ticketNumber);

        [Post("/issue/{ticketKey}/transitions")]
        public Task<object> SetTicketsState(string ticketKey, [Body(serializationMethod: BodySerializationMethod.Serialized)]TransitionContainer transitionContainer);

    }
}
