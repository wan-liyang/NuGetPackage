using System.Collections.Generic;
using TryIT.MicrosoftGraphService.Model;
using static TryIT.MicrosoftGraphService.ApiModel.AppliationResponse;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class AppliationResponseExt
    {
        public static ApplicationModel ToApplicationModel(this GetAppliationResponse response)
        {
            if (response == null || response.value == null || response.value.Count <= 0)
            {
                return null;
            }

            ApplicationModel model = new ApplicationModel();

            var responseApp = response.value[0];

            model.ObjectId = responseApp.id;
            model.DeletedDateTime = responseApp.deletedDateTime;
            model.ApplicationId = responseApp.appId;
            model.CreatedDateTime = responseApp.createdDateTime;
            model.DisplayName = responseApp.displayName;

            if (responseApp.passwordCredentials != null && responseApp.passwordCredentials.Count > 0)
            {
                model.ClientSecrets = new List<ApplicationModel.ClientSecret>();

                responseApp.passwordCredentials?.ForEach(x =>
                {
                    model.ClientSecrets.Add(new ApplicationModel.ClientSecret
                    {
                        DisplayName = x.displayName,
                        EndDateTime = x.endDateTime,
                        SecretId = x.keyId,
                        StartDateTime = x.startDateTime
                    });
                });
            }

            return model;
        }
    }
}
