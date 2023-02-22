using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Models
{
    public static class Constants
    {
        public const string OpportunityIdRequestParameter = "con_opportunityid";

        private const string JiraApiUrlConfigName = "Sales.OpportunityProcess.JiraUrl";

        private const string JiraFunctionUrlConfigName = "Sales.OpportunityProcess.JiraFunctionUrl";

        private const string EnvironmentJiraFuntionKeyName = "con_JiraFunction";

        private const string JiraGetTicketsFunctionName = "SetJiraTicketState";

        private const string ResponseParameter = "con_response";
    }
}
