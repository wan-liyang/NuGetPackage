using NUnit.Framework;
using DataService.DataConversion;
using System;

namespace NUnitTest.DataService
{
    class UrlBuilder_UnitTest
    {
        [Test]
        public void ToDateTime()
        {
            DateTime dtValue = new DateTime(2022, 2, 8);
            DateTime dtTest = DateTime.MinValue;

            string value = "2022-02-08";
            dtTest = StringToValue.ToDateTime(value);
            Assert.AreEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd");
            Assert.AreEqual(dtValue, dtTest);

            string[] arrFormat = "".Split(';');
            dtTest = StringToValue.ToDateTime(value, arrFormat);
            Assert.AreEqual(dtValue, dtTest);

            // dd/MMM/yyyy, dd/MM/yyyy, dd/MMM/yy, dd/MM/yy, MM-yy, MMM-yyyy, ddmmyyyy
            value = "08/02/2022";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MM/yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/02/22";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MM/yy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/Feb/2022";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MMM/yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "08/Feb/22";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "dd/MMM/yy");
            Assert.AreEqual(dtValue, dtTest);


            dtValue = new DateTime(2022, 2, 1);

            value = "02-22";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MM-yy");
            Assert.AreEqual(dtValue, dtTest);

            value = "02-2022";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MM-yyyy");
            Assert.AreEqual(dtValue, dtTest);

            value = "Feb-2022";
            dtTest = StringToValue.ToDateTime(value, "yyyy-MM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd");
            Assert.AreNotEqual(dtValue, dtTest);
            dtTest = StringToValue.ToDateTime(value, "yyyy-MMM-dd", "yyyy-MM-dd", "MMM-yyyy");
            Assert.AreEqual(dtValue, dtTest);



            arrFormat = "yyyy-MMM-dd;MMM-yyyy".Split(';');
            dtTest = StringToValue.ToDateTime(value, arrFormat);
            Assert.AreEqual(dtValue, dtTest);
        }

        public void ToDateTimeWithDefault()
        {
            DateTime dtValue = new DateTime(2022, 2, 8);
            DateTime dtTest = DateTime.MinValue;

            string value = "2022-02-08";

            DateTime dt = DateTime.Now;

            dtTest = StringToValue.ToDateTimeWithDefault(value, dt);
        }
    }
}
