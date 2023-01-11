using TryIT.ObjectExtension;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

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
    }
}
