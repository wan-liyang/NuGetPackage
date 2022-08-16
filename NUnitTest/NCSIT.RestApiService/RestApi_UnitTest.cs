using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestApiService;
using ObjectExtension;

namespace NUnitTest.RestApiService
{
    class RestApi_UnitTest
    {
        [Test]
        public void Get_Test()
        {
            var request = new ApiRequest
            {
                Url = "https://agiledev.ncs.com.sg/ICAM/rest/UserAccessRight/GetUserRole?EmailAddress=wan2@ncs.com.sg&ModuleName=IPMS,Common"
            };

            Assert.DoesNotThrow(() => {
                var result = RestApi.Get(request);

                Assert.AreEqual(result.StatusCode.ToString(), "OK");
            });
        }

        [Test]
        public void Post_Test()
        {
            var obj = new
            {
                ProjectCode = "P1010E003001R01",
                QALeadStaffID = "115490"
            };

            var request = new ApiRequest
            {
                Url = "https://agiledev.ncs.com.sg/InputFromIpms/rest/ProjectInfo/UpdateProject",
                Username = "B21AE060-BFC4-4FC9-B34A-B48C93316796",
                Password = "19FB25AB-19F3-4984-A18E-C10F24DAE8DD",
                Body = obj.ObjectToJson()
            };

            Assert.DoesNotThrow(() => {
                var result = RestApi.Post(request);

                Assert.AreEqual(result.StatusCode.ToString(), "OK");
            });
        }
    }
}
