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
            var user = ADService.User.FindUser("p1207563");

            Assert.NotNull(user);

            user = ADService.User.FindUser("p12075634");

            Assert.IsNull(user);
        }
    }
}

