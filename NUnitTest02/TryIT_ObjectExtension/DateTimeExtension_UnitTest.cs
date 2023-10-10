using TryIT.ObjectExtension;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;

namespace NUnitTest02.TryIT_ObjectExtension
{
    class DateTimeExtension_UnitTest
    {
        [Test]
        public void GetFiscalYear_Test()
        {
            DateTime dt = new DateTime(2022, 3, 1);
            FiscalYear fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2022));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(12));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(4));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2021, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2022, 3, 31)));

            dt = new DateTime(2022, 4, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 5, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(2));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 6, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(3));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 7, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(4));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(2));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 8, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(5));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(2));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 9, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(6));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(2));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 10, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(7));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(3));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 11, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(8));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(3));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2022, 12, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(9));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(3));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2023, 1, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(10));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(4));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2023, 2, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(11));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(4));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2023, 3, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2023));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(12));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(4));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2023, 3, 31)));

            dt = new DateTime(2023, 4, 1);
            fiscalYear = dt.GetFiscalYear();
            Assert.That(fiscalYear.FY, Is.EqualTo(2024));
            Assert.That(fiscalYear.FY_MonthIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_QuarterIndex, Is.EqualTo(1));
            Assert.That(fiscalYear.FY_StartDate, Is.EqualTo(new DateTime(2023, 4, 1)));
            Assert.That(fiscalYear.FY_EndDate, Is.EqualTo(new DateTime(2024, 3, 31)));
        }

        [Test]
        public void DateTime_IsSame_Test()
        {
            DateTime dt = DateTime.Now;
            bool isSame = false;

            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(true, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(true, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(true, isSame);

            dt = DateTime.Now.AddDays(-1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(true, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(true, isSame);

            dt = DateTime.Now.AddDays(+1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(true, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(true, isSame);

            dt = DateTime.Now.AddMonths(-1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(true, isSame);

            dt = DateTime.Now.AddMonths(+1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(true, isSame);

            dt = DateTime.Now.AddYears(-1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(false, isSame);

            dt = DateTime.Now.AddYears(+1);
            isSame = dt.IsSame(DateTime.Now, DatePart.Day);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Month);
            Assert.AreEqual(false, isSame);
            isSame = dt.IsSame(DateTime.Now, DatePart.Year);
            Assert.AreEqual(false, isSame);
        }

        [Test]
        public void DateTime_IsBetween_Test()
        {
            DateTime dt = new DateTime(2021, 1, 19);
            DateTime dtStart = new DateTime(2021, 1, 19);
            DateTime dtEnd = new DateTime(2021, 1, 20);

            // Day
            dt = new DateTime(2021, 1, 18, 23, 59, 59);
            bool isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(false, isBetween);

            dt = new DateTime(2021, 1, 19, 0, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 0, 0, 1);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 23, 59, 59);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 20, 0, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 20, 23, 59, 59);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 21, 0, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Day);
            Assert.AreEqual(false, isBetween);

            // Second
            dt = new DateTime(2021, 1, 19, 17, 59, 59);
            dtStart = new DateTime(2021, 1, 19, 18, 0, 0);
            dtEnd = new DateTime(2021, 1, 19, 20, 0, 0);

            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(false, isBetween);

            dt = new DateTime(2021, 1, 19, 18, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 18, 0, 1);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 19, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 20, 0, 0);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(true, isBetween);

            dt = new DateTime(2021, 1, 19, 20, 0, 1);
            isBetween = dt.IsBetween(dtStart, dtEnd, DatePart.Second);
            Assert.AreEqual(false, isBetween);
        }

        [Test]
        public void Diff_Test()
        {
            DateTime dt = new DateTime(2021, 1, 19);
            DateTime dt2 = new DateTime(2021, 1, 19);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Year) == 0);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Month) == 0);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Day) == 0);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Hour) == 0);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Minute) == 0);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Second) == 0);

            dt2 = new DateTime(2021, 2, 1);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Month) == -1);
            Assert.IsTrue(dt.Diff(dt2, DatePart.Day) == -13);

            Assert.IsTrue(dt2.Diff(dt, DatePart.Month) == 1);
            Assert.IsTrue(dt2.Diff(dt, DatePart.Day) == 13);
        }
    }
}
