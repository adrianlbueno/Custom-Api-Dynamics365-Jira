using IHK.FinalProject.Shared.Converter;
using IHK.FinalProject.Shared.Models.Jira;
using Jira.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using static Jira.JiraEnvironment.EnvironmentVariables;

namespace Jira
{
    public static class GetJiraTickets
    {
        #region MockJson
        private const string MockJson = "[{\"expand\":\"operations,versionedRepresentations,editmeta,changelog,renderedFields\",\"id\":\"398880\",\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issue/398880\",\"key\":\"BM-5921\",\"fields\":{\"fixVersions\":[{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/version/12302\",\"id\":\"12302\",\"description\":\"z.B. Hostausbringung\",\"name\":\"Hostausbringung\",\"archived\":false,\"released\":false,\"releaseDate\":\"2024-01-01\"}],\"customfield_11200\":null,\"resolution\":null,\"customfield_10104\":null,\"customfield_10500\":\"test\",\"customfield_10105\":null,\"customfield_10106\":null,\"customfield_10900\":150,\"customfield_10902\":\"2021-11-30\",\"customfield_10903\":null,\"customfield_10904\":\"A00001000015\",\"customfield_10905\":null,\"customfield_10906\":null,\"customfield_10907\":null,\"customfield_10908\":null,\"customfield_10909\":null,\"lastViewed\":\"2021-11-16T09:47:42.497+0100\",\"priority\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/priority/3\",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/images/icons/priorities/medium.svg\",\"name\":\"Medium\",\"id\":\"3\"},\"customfield_10100\":null,\"customfield_10101\":null,\"customfield_10102\":null,\"labels\":[],\"customfield_10103\":null,\"timeestimate\":null,\"aggregatetimeoriginalestimate\":null,\"versions\":[],\"issuelinks\":[{\"id\":\"425657\",\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issueLink/425657\",\"type\":{\"id\":\"10300\",\"name\":\"Vorgangsaufteilung\",\"inward\":\"aufteilen von\",\"outward\":\"aufteilen auf\",\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issueLinkType/10300\"},\"outwardIssue\":{\"id\":\"398881\",\"key\":\"BM-5922\",\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issue/398881\",\"fields\":{\"summary\":\"Test 2 Sub\",\"status\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/status/10516\",\"description\":\" \",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/images/icons/statuses/generic.png\",\"name\":\"Gesch�tzt\",\"id\":\"10516\",\"statusCategory\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/statuscategory/4\",\"id\":4,\"key\":\"indeterminate\",\"colorName\":\"yellow\",\"name\":\"In Arbeit\"}},\"priority\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/priority/3\",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/images/icons/priorities/medium.svg\",\"name\":\"Medium\",\"id\":\"3\"},\"issuetype\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issuetype/10702\",\"id\":\"10702\",\"description\":\"\",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/secure/viewavatar?size=xsmall&avatarId=10300&avatarType=issuetype\",\"name\":\"Sub-Request\",\"subtask\":false,\"avatarId\":10300}}}}],\"assignee\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/user?username=xcm1055\",\"name\":\"xcm1055\",\"key\":\"xcm1055\",\"emailAddress\":\"michael.buchberger@atruvia.de\",\"avatarUrls\":{\"48x48\":\"https://www.gravatar.com/avatar/a6ae11dea65a8f04cbd97ed6e2badc9f?d=mm&s=48\",\"24x24\":\"https://www.gravatar.com/avatar/a6ae11dea65a8f04cbd97ed6e2badc9f?d=mm&s=24\",\"16x16\":\"https://www.gravatar.com/avatar/a6ae11dea65a8f04cbd97ed6e2badc9f?d=mm&s=16\",\"32x32\":\"https://www.gravatar.com/avatar/a6ae11dea65a8f04cbd97ed6e2badc9f?d=mm&s=32\"},\"displayName\":\"Michael Buchberger\",\"active\":true,\"timeZone\":\"Europe/Berlin\"},\"status\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/status/10507\",\"description\":\" \",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/images/icons/statuses/generic.png\",\"name\":\"Angebot erstellen\",\"id\":\"10507\",\"statusCategory\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/statuscategory/4\",\"id\":4,\"key\":\"indeterminate\",\"colorName\":\"yellow\",\"name\":\"In Arbeit\"}},\"components\":[],\"customfield_11301\":null,\"aggregatetimeestimate\":null,\"creator\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/user?username=ta000g6q\",\"name\":\"ta000g6q\",\"key\":\"JIRAUSER29925\",\"emailAddress\":\"\",\"avatarUrls\":{\"48x48\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=48\",\"24x24\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=24\",\"16x16\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=16\",\"32x32\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=32\"},\"displayName\":\"Angebotsbuch INTE\",\"active\":true,\"timeZone\":\"Europe/Berlin\"},\"subtasks\":[],\"reporter\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/user?username=ta000g6q\",\"name\":\"ta000g6q\",\"key\":\"JIRAUSER29925\",\"emailAddress\":\"\",\"avatarUrls\":{\"48x48\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=48\",\"24x24\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=24\",\"16x16\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=16\",\"32x32\":\"https://www.gravatar.com/avatar/d41d8cd98f00b204e9800998ecf8427e?d=mm&s=32\"},\"displayName\":\"Angebotsbuch INTE\",\"active\":true,\"timeZone\":\"Europe/Berlin\"},\"aggregateprogress\":{\"progress\":0,\"total\":0},\"customfield_10200\":null,\"customfield_10201\":null,\"customfield_10951\":null,\"customfield_10953\":null,\"progress\":{\"progress\":0,\"total\":0},\"votes\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issue/BM-5921/votes\",\"votes\":0,\"hasVoted\":false},\"issuetype\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issuetype/10701\",\"id\":\"10701\",\"description\":\"\",\"iconUrl\":\"https://ccme.in.fiduciagad.de/jira/secure/viewavatar?size=xsmall&avatarId=10309&avatarType=issuetype\",\"name\":\"Request\",\"subtask\":false,\"avatarId\":10309},\"timespent\":null,\"project\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/project/11014\",\"id\":\"11014\",\"key\":\"BM\",\"name\":\"Beauftragungsmanagement\",\"projectTypeKey\":\"software\",\"avatarUrls\":{\"48x48\":\"https://ccme.in.fiduciagad.de/jira/secure/projectavatar?avatarId=10324\",\"24x24\":\"https://ccme.in.fiduciagad.de/jira/secure/projectavatar?size=small&avatarId=10324\",\"16x16\":\"https://ccme.in.fiduciagad.de/jira/secure/projectavatar?size=xsmall&avatarId=10324\",\"32x32\":\"https://ccme.in.fiduciagad.de/jira/secure/projectavatar?size=medium&avatarId=10324\"}},\"customfield_11001\":null,\"aggregatetimespent\":null,\"customfield_10940\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10820\",\"value\":\"Niedrig\",\"id\":\"10820\",\"disabled\":false},\"customfield_10941\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10824\",\"value\":\"Mittel\",\"id\":\"10824\",\"disabled\":false},\"customfield_10942\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10826\",\"value\":\"Niedrig\",\"id\":\"10826\",\"disabled\":false},\"customfield_10943\":52,\"customfield_10944\":392,\"customfield_10945\":192,\"customfield_10946\":200,\"resolutiondate\":null,\"customfield_10947\":null,\"customfield_10948\":null,\"customfield_10949\":null,\"workratio\":-1,\"watches\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/issue/BM-5921/watchers\",\"watchCount\":1,\"isWatching\":true},\"created\":\"2021-11-16T09:27:29.000+0100\",\"customfield_10300\":null,\"customfield_10301\":null,\"customfield_10930\":[\"GFSGUX\"],\"customfield_10931\":[\"003\"],\"customfield_10932\":[\"XCM1055\"],\"customfield_10933\":[\"xgadmae\"],\"customfield_10934\":[\"BBLPMA\"],\"customfield_10935\":[\"h8-A\"],\"customfield_10936\":null,\"customfield_10937\":null,\"customfield_10938\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10815\",\"value\":\"Mittel\",\"id\":\"10815\",\"disabled\":false},\"customfield_10939\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10817\",\"value\":\"Niedrig\",\"id\":\"10817\",\"disabled\":false},\"updated\":\"2021-11-16T09:45:43.000+0100\",\"timeoriginalestimate\":null,\"description\":\"sdfg\",\"customfield_11100\":52,\"customfield_10005\":\"2|i02i1z:\",\"customfield_10920\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10722\",\"value\":\"Niedrig\",\"id\":\"10722\",\"disabled\":false},\"customfield_10921\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10726\",\"value\":\"Mittel\",\"id\":\"10726\",\"disabled\":false},\"customfield_10922\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10728\",\"value\":\"weniger als 10\",\"id\":\"10728\",\"disabled\":false},\"customfield_10923\":null,\"customfield_10924\":null,\"customfield_10928\":[\"000000000010199922\"],\"customfield_10929\":[\"xcm1055\"],\"summary\":\"Test2 CRM \",\"customfield_10000\":null,\"customfield_10004\":null,\"customfield_10400\":null,\"environment\":null,\"customfield_10910\":null,\"customfield_10911\":null,\"customfield_10912\":null,\"duedate\":null,\"customfield_10915\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10707\",\"value\":\"Auftragsprogrammierung\",\"id\":\"10707\",\"disabled\":false},\"customfield_10916\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10710\",\"value\":\"Niedrig\",\"id\":\"10710\",\"disabled\":false},\"customfield_10917\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10713\",\"value\":\"Niedrig\",\"id\":\"10713\",\"disabled\":false},\"customfield_10918\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10716\",\"value\":\"Niedrig\",\"id\":\"10716\",\"disabled\":false},\"customfield_10919\":{\"self\":\"https://ccme.in.fiduciagad.de/jira/rest/api/2/customFieldOption/10719\",\"value\":\"Niedrig\",\"id\":\"10719\",\"disabled\":false}}}]";
        #endregion

        [FunctionName("GetJiraTickets")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("Http Function 'GetJiraTickets' called");

            if (!request.Query.TryGetValue("opportunitynumber", out StringValues opportunityNumber) ||
                !request.Query.TryGetValue("jiraurl", out StringValues jiraUrl))
            {
                return new BadRequestObjectResult("Required query parameters are missing.");
            }

            string jiraBearerToken = Environment.GetEnvironmentVariable(BearerTokenDev);

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(jiraUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jiraBearerToken);

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new FieldsConverter());
            jsonSerializerOptions.Converters.Add(new StringToDateTimeOffsetConverter());

            RefitSettings refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(jsonSerializerOptions));

            try
            {
                IJiraApi jiraApi = RestService.For<IJiraApi>(httpClient, refitSettings);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }

            try
            {
                List<Issue> issues = JsonSerializer.Deserialize<List<Issue>>(MockJson, jsonSerializerOptions);
                issues.ForEach((issue) =>
                    {
                        issue.Name = "10212323"; 
                                                  
                    });

                return new OkObjectResult(issues); 
            }
            catch (Exception ex)
            {

                log.LogError(ex.ToString());
                return new BadRequestResult();
            }
        }
    }
}
