using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Plugins.Helper;
using Plugins.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Plugins.CustomAPIs
{
    public class SetJiraTickets : IPlugin
    {
        #region Constants

        private const string OpportunityIdRequestParameter = "con_opportunityid";

        private const string JiraApiUrlConfigName = "Sales.OpportunityProcess.JiraUrl";

        private const string JiraFunctionUrlConfigName = "Sales.OpportunityProcess.JiraFunctionUrl";

        private const string EnvironmentJiraFuntionKeyName = "con_JiraFunction";

        private const string JiraSetTicketsFunctionName = "SetJiraTicketState";

        private const string ResponseParameter = "con_response";

        #endregion

        #region execute Method
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;

            trace.Trace($"Method start '{nameof(Execute)}'");

            ApiResponse apiResponse = new ApiResponse();

            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            IOrganizationServiceFactory serviceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (!context.InputParameters.ContainsKey(OpportunityIdRequestParameter) ||
                !(context.InputParameters[OpportunityIdRequestParameter] is Guid sourceOpportunityId))
            {
                string msg = "Request parameters are invalid";
                trace.Trace(msg);

                apiResponse.Message = msg;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            CrmServiceContext crmServiceContext = GetCrmContext(service);

            Opportunity opportunity = crmServiceContext.OpportunitySet.First(x => x.Id == sourceOpportunityId);

            if (opportunity is null)
            {
                return;
            }

            bool isOpportunityOpen = TryGetOpportunityStatus(context, opportunity, apiResponse, out int status);

            if (isOpportunityOpen == false)
            {
                return;

            }

            bool isJiraEnvironment = TryGetJiraEnvironmentDefinitionName(crmServiceContext, out string errorMessage, out EnvironmentVariableDefinition jiraFunctionEnvironmentDefinition);

            if (isJiraEnvironment == false)
            {
                return;
            }

            bool isJiraEnviromentValueFound = TryGetJiraEnviromentValueId(crmServiceContext, jiraFunctionEnvironmentDefinition, errorMessage, out string azureFunctionCode);

            string requestUrl = GetRequestUrl(azureFunctionCode);

            if (!isOpportunityOpen)
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            if (!isJiraEnviromentValueFound)
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
            }

            if (!TryGetConfigurationValue(crmServiceContext, JiraApiUrlConfigName, errorMessage, out string jiraUrl))
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            if (!TryGetConfigurationValue(crmServiceContext, JiraFunctionUrlConfigName, errorMessage, out string JiraFunctionUrl))
            {
                apiResponse.Message = errorMessage;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return;
            }

            SetJiraTicketStateRequest request = new SetJiraTicketStateRequest(jiraUrl, opportunity.con_OpportunityNumber, status);

            ValidatesIfRequestIsSuccesful(context, requestUrl, JiraFunctionUrl, apiResponse, request);
        }
        #endregion

        private CrmServiceContext GetCrmContext(IOrganizationService service)
        {
            return new CrmServiceContext(service);
        }

        private string GetRequestUrl(string azureFunctionCode)
        {
            return $"{JiraSetTicketsFunctionName}?code={azureFunctionCode}";
        }

        private bool ValidatesIfRequestIsSuccesful(IPluginExecutionContext context, string uriRequest, string jiraFunctionUrl, ApiResponse apiResponse, SetJiraTicketStateRequest request)
        {
            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(jiraFunctionUrl) })
            {
                HttpResponseMessage response = null;

                string jsonToSend = JSONSerializer.Serialize(request);

                StringContent stringContent = new StringContent(jsonToSend, Encoding.UTF8, "application/json");

                try
                {
                    response = client.PostAsync(uriRequest, stringContent).ConfigureAwait(false).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();

                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.ToString();
                    context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                    return false;
                }

                apiResponse.Successful = true;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return true;
            }
        }

        private bool TryGetOpportunityStatus(IPluginExecutionContext context, Opportunity opportunity, ApiResponse apiResponse, out int status)
        {
            status = opportunity.StateCode == OpportunityState.Open && opportunity.StatusCode == Opportunity_StatusCode.Annahme
                ? 81
                : opportunity.StateCode == OpportunityState.Lost
                ? 251
                : -1;

            if (status == -1)
            {
                string msg = $"Opportunity is not {OpportunityState.Open} or {OpportunityState.Lost}. End execution";
                apiResponse.Message = msg;
                context.OutputParameters[ResponseParameter] = JSONSerializer.Serialize(apiResponse);
                return false;
            }

            return true;
        }

        private bool TryGetJiraEnvironmentDefinitionName(CrmServiceContext crmServiceContext, out string errorMessage,
            out EnvironmentVariableDefinition jiraFunctionEnvironmentDefinition)
        {
            errorMessage = null;

            jiraFunctionEnvironmentDefinition = crmServiceContext.EnvironmentVariableDefinitionSet
                .FirstOrDefault(x => x.SchemaName == EnvironmentJiraFuntionKeyName);

            if (jiraFunctionEnvironmentDefinition is null)
            {
                errorMessage = $"Could not be found variable definition with name '{EnvironmentJiraFuntionKeyName}'.";
                return false;
            }

            return true;
        }

        private bool TryGetJiraEnviromentValueId(CrmServiceContext crmServiceContext, EnvironmentVariableDefinition jiraFunctionEnvironmentDefinition,
            string errorMessage, out string azureFunctionCode)
        {
            azureFunctionCode = null;
            EnvironmentVariableValue jiraFunctionEnvironmentValue = crmServiceContext.EnvironmentVariableValueSet
                .FirstOrDefault(x => x.EnvironmentVariableDefinitionId.Id == jiraFunctionEnvironmentDefinition.Id);


            if (jiraFunctionEnvironmentValue is null)
            {
                errorMessage = $"Could not found variable value with the defintion id {jiraFunctionEnvironmentDefinition.Id}";
                return false;
            }

            azureFunctionCode = jiraFunctionEnvironmentValue.Value;
            return true;
        }

        private bool TryGetConfigurationValue(CrmServiceContext crmContext, string configName,
            string message, out string configValue)
        {
            configValue = null;

            con_configuration jiraConfig = crmContext.con_configurationSet.FirstOrDefault(x => x.con_name == configName);

            if (jiraConfig is null)
            {
                message = $"Could not be found config entry with the name '{configName}'.";
                return false;
            }

            message = $"It works";
            configValue = jiraConfig.con_value;
            return true;
        }
    }
}
