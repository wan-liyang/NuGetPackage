using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Group
{
    public class GetGroupResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<Group> value { get; set; }
        }

        public class Group
        {
            public string id { get; set; }
            public object deletedDateTime { get; set; }
            public object classification { get; set; }
            public DateTime createdDateTime { get; set; }
            public List<object> creationOptions { get; set; }
            public string description { get; set; }
            public string displayName { get; set; }
            public object expirationDateTime { get; set; }
            public List<object> groupTypes { get; set; }
            public object isAssignableToRole { get; set; }
            public object mail { get; set; }
            public bool mailEnabled { get; set; }
            public string mailNickname { get; set; }
            public object membershipRule { get; set; }
            public object membershipRuleProcessingState { get; set; }
            public object onPremisesDomainName { get; set; }
            public object onPremisesLastSyncDateTime { get; set; }
            public object onPremisesNetBiosName { get; set; }
            public object onPremisesSamAccountName { get; set; }
            public object onPremisesSecurityIdentifier { get; set; }
            public object onPremisesSyncEnabled { get; set; }
            public object preferredDataLocation { get; set; }
            public object preferredLanguage { get; set; }
            public List<object> proxyAddresses { get; set; }
            public DateTime renewedDateTime { get; set; }
            public List<object> resourceBehaviorOptions { get; set; }
            public List<object> resourceProvisioningOptions { get; set; }
            public bool securityEnabled { get; set; }
            public string securityIdentifier { get; set; }
            public object theme { get; set; }
            public object visibility { get; set; }
            public List<object> serviceProvisioningErrors { get; set; }
            public List<object> onPremisesProvisioningErrors { get; set; }
        }
    }
}
