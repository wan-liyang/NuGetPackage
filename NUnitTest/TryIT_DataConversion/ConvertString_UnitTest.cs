using NUnit.Framework;
using TryIT.DataConversion;
using System;

namespace NUnitTest.TryIT_DataConversion
{
    class ConvertString_UnitTest
    {
        [Test]
        public void ToDateTime()
        {
            DateTime dtValue = new DateTime(2022, 2, 8);
            DateTime dtTest = DateTime.MinValue;

            string value = "2022-02-08";
            dtTest = ConvertString.ToDateTime(value);
            Assert.AreEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd");
            Assert.AreEqual(dtValue, dtTest);

            string[] arrFormat = "".Split(';');
            dtTest = ConvertString.ToDateTime(value, arrFormat);
            Assert.AreEqual(dtValue, dtTest);

            // dd/MMM/yyyy, dd/MM/yyyy, dd/MMM/yy, dd/MM/yy, MM-yy, MMM-yyyy, ddmmyyyy
            value = "08/02/2022";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MM/yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/02/22";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MM/yy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/Feb/2022";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MMM/yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/Feb/22";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MMM/yy");
            Assert.AreEqual(dtValue, dtTest);


            dtValue = new DateTime(2022, 2, 1);

            value = "02-22";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MM-yy");
            Assert.AreEqual(dtValue, dtTest);

            value = "02-2022";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MM-yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "Feb-2022";
            dtTest = ConvertString.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = ConvertString.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MMM-yyyy");
            Assert.AreEqual(dtValue, dtTest);



            arrFormat = "yyyy-MMM-dd;MMM-yyyy".Split(';');
            dtTest = ConvertString.ToDateTime(value, arrFormat);
            Assert.AreEqual(dtValue, dtTest);
        }

        [Test]
        public void ToDateTimeWithDefault()
        {
            DateTime dtValue = new DateTime(2022, 2, 8);
            DateTime dtTest = DateTime.MinValue;

            string value = "2022-02-08";

            DateTime dt = DateTime.Now;

            dtTest = ConvertString.ToDateTimeWithDefault(value, dt);
        }

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
