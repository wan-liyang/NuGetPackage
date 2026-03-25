using System;
using TryIT.ActiveDirectory;

namespace NUnitTest02.TryIT_ActiveDirectory
{
    public class AdUser_UnitTest
    {
        public AdUser_UnitTest()
        {
        }

        [Test]
        public void GetUserByKeyword()
        {
            var user = ADService.User.FindUser("abc");
            var group = ADService.User.FindUserGroup(user.DistinguishedName);

            var manager = ADService.User.FindUserManager(user.DistinguishedName);

            Assert.NotNull(user);

            user = ADService.User.FindUser("abc");


            Assert.IsNull(user);
        }

        [Test]
        public void GetGroup()
        {
            var members = ADService.Group.GetGroupMembers("");

            Assert.IsNotNull(members);
        }
    }
}

