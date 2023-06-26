using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphService.Model;
using TryIT.MicrosoftGraphService.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphService
{
    internal class Group_UnitTest
    {
        [Test]
        public void Group_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            GroupApi api = new GroupApi(config);

            string groupDisplayName = "";
            string userEmail = "";

            var group = api.GetGroup(groupDisplayName);
            Assert.IsNotNull(group);
            Debug.WriteLine(TryIT.ObjectExtension.ObjExtension.ObjectToJson(group));

            var members = api.GetMembers(groupDisplayName);
            Assert.Greater(members.Count, 0);
            Debug.WriteLine(TryIT.ObjectExtension.ObjExtension.ObjectToJson(members));

            var ismember = api.IsMemberOf(userEmail, groupDisplayName);
            Assert.IsFalse(ismember);

            api.AddMember(userEmail, groupDisplayName);
            ismember = api.IsMemberOf(userEmail, groupDisplayName);
            Assert.IsTrue(ismember);


            api.RemoveMember(userEmail, groupDisplayName);
            ismember = api.IsMemberOf(userEmail, groupDisplayName);
            Assert.IsFalse(ismember);
        }
    }
}
