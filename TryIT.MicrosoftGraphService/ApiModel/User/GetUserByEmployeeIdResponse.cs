using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TryIT.MicrosoftGraphService.Model.User;

namespace TryIT.MicrosoftGraphService.ApiModel.User
{
    internal class GetUserByEmployeeIdResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<Value> value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string userPrincipalName { get; set; }
            public string displayName { get; set; }
            public string mail { get; set; }
            public string employeeId { get; set; }
        }
    }

    internal static class GetUserByEmployeeIdResponseExtension
    {
        public static EmployeeModel ToEmployeeModel(this GetUserByEmployeeIdResponse.Response user)
        {
            EmployeeModel model = new EmployeeModel();

            var emp = user.value.First();
            if (emp != null)
            {
                model.Id = emp.id;
                model.EmailAddress = emp.mail;
                model.Name = emp.displayName;
                model.EmployeeId = emp.employeeId;
            }
            return model;
        }
    }
}
