using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Models.Jira
{
    public class Fields
    {
        [JsonPropertyName("fixVersions")]
        public List<FixVersion> FixVersions{ get; set; }

        [JsonPropertyName("customfields")]
        public Dictionary<string, object> CustomFields { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("resolution")]
        public object Resolution { get; set; }

        [JsonPropertyName("lastViewed")]
        public DateTimeOffset LastViewed { get; set; }

        [JsonPropertyName("priority")]
        public Priority Priority { get; set; }

        [JsonPropertyName("labels")]
        public List<object> Labels{ get; set; }

        [JsonPropertyName("timeestimate")]
        public List<object> TimeEstimate { get; set; }
        
        [JsonPropertyName("aggregatetimeoriginalestimate")]
        public object AggregateTimeOriginalEstimate { get; set; }
        
        [JsonPropertyName("versions")]
        public List<object> Versions{ get; set; }

        [JsonPropertyName("issuelinks")]
        public List<IssueLink> IssueLinks { get; set; }

        [JsonPropertyName("assignee")]
        public Assignee Assignee { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("components")]
        public List<object> Components { get; set; }

        [JsonPropertyName("aggregatetimeestimate")]
        public object AggregateTimeEstimate { get; set; }

        [JsonPropertyName("creator")]
        public Creator Creator { get; set; }

        [JsonPropertyName("subtasks")]
        public List<object> Subtasks { get; set; }

        [JsonPropertyName("reporter")]
        public Reporter Reporter { get; set; }

        [JsonPropertyName("aggregateprogress")]
        public ProgressContainer AggregateProgress { get; set; }

        [JsonPropertyName("progress")]
        public ProgressContainer Progress { get; set; }

        [JsonPropertyName("votes")]
        public Vote Votes { get; set; }

        [JsonPropertyName("issuetype")]
        public IssueType IssueType { get; set; }

        [JsonPropertyName("timespent")]
        public object Timespent { get; set; }

        [JsonPropertyName("project")]
        public Project Project { get; set; }

        [JsonPropertyName("aggregatetimespent")]
        public object AggregateTimeSpent { get; set; }

        [JsonPropertyName("resolutiondate")]
        public object ResolutionDate { get; set; }

        [JsonPropertyName("workratio")]
        public int Workratio { get; set; }

        [JsonPropertyName("watches")]
        public Watches Watches { get; set; }

        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; }

        [JsonPropertyName("updated")]
        public DateTimeOffset Updated { get; set; }

        [JsonPropertyName("timeoriginalestimate")]
        public object TimeOriginalEstimate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("environment")]
        public object Environment { get; set; }

        [JsonPropertyName("duedate")]
        public object DueDate { get; set; }

    }
}
