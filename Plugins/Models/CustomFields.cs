using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plugins.Models.Jira
{
    [DataContract]
    public class CustomFields
    {
        [DataMember(Name = "customfield_10900")]
        public double EffortInManDays { get; set; }

        [DataMember(Name = "customfield_10928")]
        public List<string> Products { get; set; }

        [DataMember(Name = "customfield_10907")]
        public object ArticleNumber { get; set; }
        
        [DataMember(Name = "customfield_10909")]
        public object ExpectedRevenue{ get; set; }

        [DataMember(Name = "customfield_10910")]
        public object Sponsor { get; set; }

        [DataMember(Name = "customfield_10901")]
        public object EffortProductManagementInManDays { get; set; }

    }
}