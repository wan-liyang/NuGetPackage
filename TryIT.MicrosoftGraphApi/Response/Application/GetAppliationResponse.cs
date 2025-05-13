using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Application
{
    public class GetAppliationResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }

            [JsonProperty("@odata.nextLink")]
            public string odatanextLink { get; set; }
            public List<Appliation> value { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Api
        {
            public object acceptMappedClaims { get; set; }
            public List<object> knownClientApplications { get; set; }
            public object requestedAccessTokenVersion { get; set; }
            public List<object> oauth2PermissionScopes { get; set; }
            public List<object> preAuthorizedApplications { get; set; }
        }

        public class ImplicitGrantSettings
        {
            public bool enableAccessTokenIssuance { get; set; }
            public bool enableIdTokenIssuance { get; set; }
        }

        public class Info
        {
            public object logoUrl { get; set; }
            public object marketingUrl { get; set; }
            public object privacyStatementUrl { get; set; }
            public object supportUrl { get; set; }
            public object termsOfServiceUrl { get; set; }
        }

        public class ParentalControlSettings
        {
            public List<object> countriesBlockedForMinors { get; set; }
            public string legalAgeGroupRule { get; set; }
        }

        public class PasswordCredential
        {
            public object customKeyIdentifier { get; set; }
            public string displayName { get; set; }
            public DateTime endDateTime { get; set; }
            public string hint { get; set; }
            public string keyId { get; set; }
            public object secretText { get; set; }
            public DateTime startDateTime { get; set; }
        }

        public class PublicClient
        {
            public List<string> redirectUris { get; set; }
        }

        public class RedirectUriSetting
        {
            public string uri { get; set; }
            public object index { get; set; }
        }

        public class RequiredResourceAccess
        {
            public string resourceAppId { get; set; }
            public List<ResourceAccess> resourceAccess { get; set; }
        }

        public class ResourceAccess
        {
            public string id { get; set; }
            public string type { get; set; }
        }

        public class Spa
        {
            public List<object> redirectUris { get; set; }
        }

        public class Appliation
        {
            public string id { get; set; }
            public object deletedDateTime { get; set; }
            public string appId { get; set; }
            public object applicationTemplateId { get; set; }
            public object disabledByMicrosoftStatus { get; set; }
            public DateTime createdDateTime { get; set; }
            public string displayName { get; set; }
            public object description { get; set; }
            public object groupMembershipClaims { get; set; }
            public List<object> identifierUris { get; set; }
            public object isDeviceOnlyAuthSupported { get; set; }
            public object isFallbackPublicClient { get; set; }
            public object notes { get; set; }
            public string publisherDomain { get; set; }
            public object serviceManagementReference { get; set; }
            public string signInAudience { get; set; }
            public List<object> tags { get; set; }
            public object tokenEncryptionKeyId { get; set; }
            public object samlMetadataUrl { get; set; }
            public object defaultRedirectUri { get; set; }
            public object certification { get; set; }
            public object optionalClaims { get; set; }
            public object servicePrincipalLockConfiguration { get; set; }
            public object requestSignatureVerification { get; set; }
            public List<object> addIns { get; set; }
            public Api api { get; set; }
            public List<object> appRoles { get; set; }
            public Info info { get; set; }
            public List<object> keyCredentials { get; set; }
            public ParentalControlSettings parentalControlSettings { get; set; }
            public List<PasswordCredential> passwordCredentials { get; set; }
            public PublicClient publicClient { get; set; }
            public List<RequiredResourceAccess> requiredResourceAccess { get; set; }
            public VerifiedPublisher verifiedPublisher { get; set; }
            public Web web { get; set; }
            public Spa spa { get; set; }
        }

        public class VerifiedPublisher
        {
            public object displayName { get; set; }
            public object verifiedPublisherId { get; set; }
            public object addedDateTime { get; set; }
        }

        public class Web
        {
            public object homePageUrl { get; set; }
            public object logoutUrl { get; set; }
            public List<string> redirectUris { get; set; }
            public ImplicitGrantSettings implicitGrantSettings { get; set; }
            public List<RedirectUriSetting> redirectUriSettings { get; set; }
        }
    }
}
