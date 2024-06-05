using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Plugins.Helper;
using Plugins.Models;
using Plugins.Models.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Plugins.CustomAPIs
{
    public class GetJiraTickets : IPlugin
    {
        private const string OpportunityIDRequestParameter = "con_opportunityid";

        private const string ResponseParameter = "con_response";

        private const string JiraApiUrlConfigName = "Sales.OpportunityProcess.JiraUrl";

        private const string EnvironmentJiraFunctionKeyName = "con_JiraFunction";

        private const string JiraFunctionUrlConfigName = "Sales.OpportunityProcess.JiraFunctionUrl";

        private const string JiraGetTicketsFunctionName = "GetJiraTickets";

        #region Execute Method
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;

            var executionContext = serviceProvider.Get<IPluginExecutionContext>();
            
            if (executionContext.Stage == 30)
            {
                return;
            }

            trace.Trace($"Method start '{nameof(Execute)}'");

            ApiResponse apiResponse = new ApiResponse();

            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            IOrganizationServiceFactory serviceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            bool areParametersValid = ValidatesParameters(apiResponse, context, out Guid sourceOpportunityId);

            if (areParametersValid == false)
            {
                return;
            }

            CrmServiceContext crmContext = GetCrmContext(service);

            Opportunity oppty = GetOpportunity(crmContext, sourceOpportunityId);

            if (!TryGetAzureFunctionCode(crmContext, out string errorMessage, out string azureFunctionCode))
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            if (!TryGetConfigurationValue(crmContext, JiraFunctionUrlConfigName, out errorMessage, out string jiraFunctionUrl))
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            if (!TryGetConfigurationValue(crmContext, JiraApiUrlConfigName, out errorMessage, out string jiraUrl))
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            string azureFunctionUrl = GetUrl(oppty, azureFunctionCode, jiraUrl);

            HttpResponseMessage responseMessage = MakeResponseRequestToAzureFunctions(jiraFunctionUrl, azureFunctionUrl);

            string json = ReadDataFromResponse(responseMessage);

            List<Issue> issues = GetIssues(json);

            IssuesHandler(crmContext, service, context, issues, oppty, apiResponse, trace);
        }
        #endregion

        #region methods
        private bool ValidatesParameters(ApiResponse apiResponse, IPluginExecutionContext context, out Guid opportunityid)
        {
            if (!context.InputParameters.ContainsKey(OpportunityIDRequestParameter) ||
                !(context.InputParameters[OpportunityIDRequestParameter] is Guid sourceOpportunityId))
            {
                string invalidParametersMessage = "Parameters are invalid";

                apiResponse.Message = invalidParametersMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                opportunityid = Guid.Empty;

                return false;
            }

            apiResponse.Message = "The response is successful";
            opportunityid = sourceOpportunityId;

            return true;
        }

        private Opportunity GetOpportunity(CrmServiceContext crmServiceContext, Guid sourceOpportunityId)
        {
            return crmServiceContext.OpportunitySet.FirstOrDefault(x => x.Id == sourceOpportunityId);
        }

        private CrmServiceContext GetCrmContext(IOrganizationService service) 
        {
            return new CrmServiceContext(service);
        }

        private string GetUrl(Opportunity opportunity, string azureFunctionCode, string jiraUrl)
        {

            return $"{JiraGetTicketsFunctionName}?code={azureFunctionCode}&" +
            $"opportunitynumber={opportunity.con_OpportunityNumber}&jiraurl={jiraUrl}";

        }

        private HttpResponseMessage MakeResponseRequestToAzureFunctions(string jiraFunctionUrl, string getUrl)
        {
            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(jiraFunctionUrl) })
            {
                try
                {
                    return client.GetAsync(getUrl).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.ToString());
                }
            }
        }

        private string ReadDataFromResponse(HttpResponseMessage response)
        { 
            return response.Content.ReadAsStringAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private List<Issue> GetIssues(string json)
        {
            try
            {
                return JSONSerializer.Deserialize<List<Issue>>(json);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to deserialize Issues.", ex);
            }
        }

        private void IssuesHandler(CrmServiceContext crmContext, IOrganizationService service, IExecutionContext context, List<Issue> issues,
            Opportunity opportunity, ApiResponse apiResponse, ITracingService trace)
        {
            foreach (Issue issue in issues)
            {
                if (string.IsNullOrWhiteSpace(issue.Name))
                {
                    continue;
                }

                con_JiraTicket jiraTicket = GetJiraTicket(crmContext, service, issue);

                jiraTicket.con_OpportunityId = opportunity.ToEntityReference();
                jiraTicket.con_TicketNumber = issue.Key;
                jiraTicket.con_Description = issue.Fields.Summary;
                jiraTicket.con_TicketStatusCode = (con_JiraTicket_con_TicketStatusCode)Convert.ToInt32(issue.Fields?.Status?.Id);

                try
                {
                    crmContext.UpdateObject(jiraTicket);
                    crmContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    trace.Trace($"Failed to update jira ticket {ex.ToString()}");
                }
            }

            apiResponse.Successful = true;
            context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
        }

        private con_JiraTicket GetJiraTicket(CrmServiceContext crmContext, IOrganizationService service, Issue issue)
        {
            con_JiraTicket jiraTicket = crmContext.con_JiraTicketSet.FirstOrDefault(x => x.con_Name == issue.Name);

            if (jiraTicket is null)
            {
                jiraTicket = new con_JiraTicket
                {
                    con_Name = issue.Name
                };

                jiraTicket.Id = service.Create(jiraTicket);
            }

            return jiraTicket;
        }

        private bool TryGetAzureFunctionCode(CrmServiceContext crmContext, out string errorMessage, out string azureFuntionCode)
        {
            azureFuntionCode = null;
            errorMessage = null;

            EnvironmentVariableDefinition jiraFunctionEnviromentDefinition = crmContext.EnvironmentVariableDefinitionSet
                .FirstOrDefault(x => x.SchemaName == EnvironmentJiraFunctionKeyName);

            if (jiraFunctionEnviromentDefinition is null)
            {
                return false;
            }

            EnvironmentVariableValue jiraFunctionEnvironmentValue = crmContext.EnvironmentVariableValueSet
                .FirstOrDefault(x => x.EnvironmentVariableDefinitionId.Id == jiraFunctionEnviromentDefinition.Id);

            if (jiraFunctionEnvironmentValue is null)
            {
                return false;
            }

            azureFuntionCode = jiraFunctionEnvironmentValue.Value;
            return true;
        }

        private bool TryGetConfigurationValue(CrmServiceContext crmContext, string configName,
            out string errorMessage, out string configValue)
        {
            errorMessage = null;
            configValue = null;

            con_configuration jiraConfig = crmContext.con_configurationSet.FirstOrDefault(x => x.con_name == configName);

            if (jiraConfig is null)
            {
                return false;
            }

            configValue = jiraConfig.con_value;
            return true;
        }
        #endregion
    }
}
