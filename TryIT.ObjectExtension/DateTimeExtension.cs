using System;
namespace TryIT.ObjectExtension
{
    /// <summary>
    /// fiscal year information
    /// </summary>
    public class FiscalYear
    {
        public DateTime InputDateTime { get; set; }
        public int FY { get; set; }
        public int FY_MonthIndex { get; set; }
        public int FY_QuarterIndex { get; set; }
        public DateTime FY_StartDate { get; set; }
        public DateTime FY_EndDate { get; set; }
    }

    /// <summary>
    /// <see cref="DateTime"/> extension method
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// get fiscal year information based on input <paramref name="dateTime"/>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static FiscalYear GetFiscalYear(this DateTime dateTime)
        {
            int _fy = dateTime.Year;
            int _fy_month = 0;
            DateTime _fy_start = dateTime;
            DateTime _fy_end = dateTime;

            if (dateTime.Month >= 1 && dateTime.Month <= 3)
            {
                _fy = dateTime.Year;
                _fy_month = 9 + dateTime.Month;
                _fy_start = new DateTime(dateTime.Year - 1, 4, 1);
                _fy_end = new DateTime(dateTime.Year, 3, 31);
            }
            else
            {
                _fy = dateTime.Year + 1;
                _fy_month = dateTime.Month - 3;
                _fy_start = new DateTime(dateTime.Year, 4, 1);
                _fy_end = new DateTime(dateTime.Year + 1, 3, 31);
            }

            int _fy_quarter = 0;
            if (_fy_month >= 1 && _fy_month <= 3)
            {
                _fy_quarter = 1;
            }
            else if (_fy_month >= 4 && _fy_month <= 6)
            {
                _fy_quarter = 2;
            }
            else if (_fy_month >= 7 && _fy_month <= 9)
            {
                _fy_quarter = 3;
            }
            else
            {
                _fy_quarter = 4;
            }

            return new FiscalYear
            {
                InputDateTime = dateTime,
                FY = _fy,
                FY_MonthIndex = _fy_month,
                FY_QuarterIndex = _fy_quarter,
                FY_StartDate = _fy_start,
                FY_EndDate = _fy_end
            };
        }
    }
}

