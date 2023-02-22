using IHK.FinalProject.Shared.Models.Jira.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;
using IHK.FinalProject.Shared.Models.Jira;
using Jira.Api;
using static Jira.JiraEnvironment.EnvironmentVariables;
namespace Jira.Functions
{
    public static class SetJiraTicketState
    {

        [FunctionName("SetJiraTicketState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("Http Function 'SetJiraTickets' called.'");

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();

            string bodyJson = await new StreamReader(request.Body).ReadToEndAsync();

            SetJiraTicketStateRequestBody requestBody = null;

            try
            {
                requestBody = JsonSerializer.Deserialize<SetJiraTicketStateRequestBody>(bodyJson);
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult("Required body is missing");
            }

            string jiraBearerToken = Environment.GetEnvironmentVariable(BearerTokenDev);

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(requestBody.JiraUrl)

            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jiraBearerToken);

            RefitSettings refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(jsonSerializerOptions));

            TransitionContainer transitionContainer = new TransitionContainer
            {
                Transition = new Transition
                {
                    Id = requestBody.JiraUrl
                }
            };

            try
            {
                IJiraApi jiraApi = RestService.For<IJiraApi>(httpClient, refitSettings);
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }

            return new OkResult();
        }
    }
}
