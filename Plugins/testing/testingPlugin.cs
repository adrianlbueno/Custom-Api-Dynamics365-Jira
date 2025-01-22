public class TestingPluing : IPlugin
{
    public void Execute(IServiceProvider serviveProvide) {

        ITracingService tracingService =(ITracingService)serviceProvider.GetService(typeof(ITracingService));

        tracingService.Trace("Implemented tracing service succesfully!");

        // Obtain the execution context from the service provider.

        IPluginExecutionContext context = localContext.PluginExecutionContext;

        // Get a reference to the Organization service.

        IOrganizationService service = localContext.OrganizationService;

        if (context.InputParameters.Contains("Target"))

        {
        //Confirm that Target is actually an Entity

        if (context.InputParameters["Target"] is Entity)

        {

            Guid _userID = context.InitiatingUserId;

            //Retrieve the name of the user (used later)

            Entity user = service.Retrieve("systemuser", _userID, new ColumnSet("fullname"));

            string userName = user.GetAttributeValue<string>("fullname");

            Entity lead = (Entity)context.InputParameters["Target"];

            Entity preLead = (Entity)context.PreEntityImages["Image"];

            Entity postLead = (Entity)context.PostEntityImages["Image"];

            string preCompanyName = preLead.GetAttributeValue<string>("companyname");

            string postCompanyName = postLead.GetAttributeValue<string>("companyname");

            tracingService.Trace("Pre-Company Name: " + preCompanyName + " Post-Company Name: " + postCompanyName);

            if (preCompanyName != postCompanyName)

            {
                tracingService.Trace("Pre-Company Name does not match Post-Company Name, alerting sales manager...");

                //Queue ID for our Sales Manager

                Guid _salesManagerQueueID = new Guid("41b22ba9-c866-e611-80c9-00155d02dd0d");

                Entity fromParty = new Entity("activityparty");
                Entity toParty = new Entity("activityparty");

                //Email body text is in HTML

                string emailBody = "<html lang='en'><head><meta charset='UTF-8'></head><body><p>Hello,</p><p>Please be advised that I have just changed the Company Name of a Lead record in CRM:</p><p>Lead Record URL:  <a href='http://mycrm/MyCrmInstance/main.aspx?etn=lead&pagetype=entityrecord&id=%7B" + lead.Id + "%7D'>" + postCompanyName + "</a></p><p>Old Company Name Value: " + preCompanyName + "</p><p>New Company Name Value: " + postCompanyName + "</p><p>Kind Regards</p><p>" + userName + "</p></body></html>";

                fromParty["partyid"] = new EntityReference("systemuser", _userID);
                toParty["partyid"] = new EntityReference("queue", _salesManagerQueueID);

                Entity email = new Entity("email");

                email["from"] = new Entity[] { fromParty };
                email["to"] = new Entity[] { toParty };
                email["subject"] = "Lead Company Name Changed";
                email["directioncode"] = true;
                email["description"] = emailBody;

                //This bit just creates the e-mail record and gives us the GUID for the new record...

                Guid _emailID = service.Create(email);

                tracingService.Trace("Email record " + _emailID + " succesfully created.");

                //...to actually send it, we need to use SendEmailRequest & SendEmailResponse, using the _emailID to reference the record

                SendEmailRequest sendEmailreq = new SendEmailRequest
                {
                    EmailId = _emailID,
                    TrackingToken = "",
                    IssueSend = true

                };

                SendEmailResponse sendEmailResp = (SendEmailResponse)service.Execute(sendEmailreq);

                tracingService.Trace("Email record " + _emailID + " queued succesfully.");
            }

            else
            {
                
                tracingService.Trace("Company Name does not appear to have changed, is this correct?");
                return;
            }

            tracingService.Trace("Ending plugin execution.");
    }
}

           