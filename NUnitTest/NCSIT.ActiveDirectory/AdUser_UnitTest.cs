using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ActiveDirectory;

namespace NUnitTest.ActiveDirectory
{
    class DESEncryption_UnitTest
    {
        [Test]
        public void GetUserByKeyword()
        {
            var user = ADService.User.FindUser("p1207563");

            Assert.NotNull(user);

            user = ADService.User.FindUser("p12075634");

            Assert.IsNull(user);
        }

        [Test]
        public void GetUserByEmployeeId()
        {
            var user = ADService.User.FindUserByEmployeeId("1207563");

            Assert.NotNull(user);

            user = ADService.User.FindUserByEmployeeId("p1207563");

            Assert.IsNull(user);
        }

        [Test]
        public void GetUserByUserLogin()
        {
            var user = ADService.User.FindUserByEmployeeId("p1207563");

            Assert.NotNull(user);

            user = ADService.User.FindUserByEmployeeId("1207563");

            Assert.IsNull(user);
        }


        [Test]
        public void GetUserByEmailAddress()
        {
            var user = ADService.User.FindUserByEmailAddress("wan2@ncs.com.sg");

            Assert.NotNull(user);

            user = ADService.User.FindUserByEmailAddress("p1207563");

            Assert.IsNull(user);
        }

        [Test]
        public void GetUserManager()
        {
            var user = ADService.User.FindUser("wan2@ncs.com.sg");

            Assert.NotNull(user);

            var manager = ADService.User.FindUserManager(user.DistinguishedName);

            Assert.NotNull(manager);
        }

        [Test]
        public void GetUserGroup()
        {
            var user = ADService.User.FindUser("wan2@ncs.com.sg");

            Assert.NotNull(user);

            var groups = ADService.User.FindUserGroup(user.DistinguishedName);

            Assert.NotNull(groups);
        }
    }
}
