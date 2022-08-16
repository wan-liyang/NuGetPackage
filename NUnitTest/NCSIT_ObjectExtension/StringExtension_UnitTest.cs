using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectExtension;
using NUnit.Framework;
using DataService.DataConversion;

namespace NUnitTest.Liyang_ObjectExtension
{
    class StringExtension_UnitTest
    {
        [Test]
        public void ToDateTime_UnitTest()
        {
            var dateE = new DateTime(2022, 11, 10, 18, 35, 31);
            var dateA = "11/10/2022 6:35:31 PM".ToDateTime("MM/dd/yyyy h:m:s tt");
            Assert.AreEqual(dateE, dateA);

            dateE = new DateTime(2022, 9, 8, 18, 35, 31);
            dateA = "9/8/2022 6:35:31 PM".ToDateTime("M/d/yyyy h:m:s tt");
            Assert.AreEqual(dateE, dateA);
        }
    }
}
