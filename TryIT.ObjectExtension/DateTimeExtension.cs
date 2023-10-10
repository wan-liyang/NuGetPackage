using System;
using System.Collections.Generic;

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

    public enum DatePart
    {
        None = 0,
        Year = 1,
        Month = 2,
        Day = 3,
        Hour = 4,
        Minute = 5,
        Second = 6,
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

        /// <summary>
        /// Compare to datetime, return number of datepart between t1 and t2, t1 - t2
        /// </summary>
        /// <param name="t1">first datetime</param>
        /// <param name="t2">second datetime</param>
        /// <param name="datepart">Year, Month, Day, compare which datepart, default compart whole date</param>
        /// <returns></returns>
        public static double Diff(this DateTime t1, DateTime t2, DatePart datepart)
        {
            double value = 0;
            DateTime dt1 = DateTime.MinValue;
            DateTime dt2 = DateTime.MinValue;
            switch (datepart)
            {
                case DatePart.None:
                    value = DateTime.Compare(t1, t2);
                    break;
                case DatePart.Year:
                    value = t1.Year - t2.Year;
                    break;
                case DatePart.Month:
                    value = ((t1.Year - t2.Year) * 12) + t1.Month - t2.Month;
                    break;
                case DatePart.Day:
                    dt1 = new DateTime(t1.Year, t1.Month, t1.Day);
                    dt2 = new DateTime(t2.Year, t2.Month, t2.Day);
                    value = (dt1 - dt2).TotalDays;
                    break;
                case DatePart.Hour:
                    dt1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, 0, 0);
                    dt2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, 0, 0);
                    value = (dt1 - dt2).TotalHours;
                    break;
                case DatePart.Minute:
                    dt1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, 0);
                    dt2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, 0);
                    value = (dt1 - dt2).TotalMinutes;
                    break;
                case DatePart.Second:
                    dt1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
                    dt2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                    value = (dt1 - dt2).TotalSeconds;
                    break;
                default:
                    value = DateTime.Compare(t1, t2);
                    break;
            }
            return value;
        }

        /// <summary>
        /// check whether the <paramref name="dt1"/> is after <paramref name="dt2"/>
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="datepart"></param>
        /// <returns></returns>
        public static bool IsAfter(this DateTime dt1, DateTime dt2, DatePart datepart)
        {
            double dtDiff = Diff(dt1, dt2, datepart);
            return dtDiff > 0;
        }
        /// <summary>
        /// check whether the <paramref name="dtSource"/> is same as <paramref name="dtValue"/>
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="dtValue"></param>
        /// <param name="datepart"></param>
        /// <returns></returns>
        public static bool IsSame(this DateTime dtSource, DateTime dtValue, DatePart datepart)
        {
            double dtDiff = Diff(dtSource, dtValue, datepart);
            return dtDiff == 0;
        }

        /// <summary>
        /// indicator whether <paramref name="dt"/> is between <paramref name="dtStart"/> and <paramref name="dtEnd"/> for specific <paramref name="datepart"/>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="datepart"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime dt, DateTime dtStart, DateTime dtEnd, DatePart datepart)
        {
            bool result = false;
            if (datepart == DatePart.None)
            {
                if (dt.Ticks >= dtStart.Ticks && dt.Ticks <= dtEnd.Ticks)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                int startValue = 0;
                int endValue = 0;
                int tmpValue = -1;
                switch (datepart)
                {
                    case DatePart.None:
                        break;
                    case DatePart.Year:
                        startValue = dtStart.Year;
                        endValue = dtEnd.Year;
                        tmpValue = dt.Year;
                        if (tmpValue >= startValue && tmpValue <= endValue)
                        {
                            result = true;
                        }
                        break;
                    case DatePart.Month:
                        startValue = dtStart.Year * 100 + dtStart.Month;
                        endValue = dtEnd.Year * 100 + dtEnd.Month;
                        tmpValue = dt.Year * 100 + dt.Month;
                        if (tmpValue >= startValue && tmpValue <= endValue)
                        {
                            result = true;
                        }
                        break;
                    case DatePart.Day:
                        startValue = dtStart.Year * 10000 + dtStart.Month * 100 + dtStart.Day;
                        endValue = dtEnd.Year * 10000 + dtEnd.Month * 100 + dtEnd.Day;
                        tmpValue = dt.Year * 10000 + dt.Month * 100 + dt.Day;
                        if (tmpValue >= startValue && tmpValue <= endValue)
                        {
                            result = true;
                        }
                        break;
                    case DatePart.Second:
                        TimeSpan timeStart = dt - dtStart;
                        TimeSpan timeEnd = dt - dtEnd;
                        if (timeStart.TotalSeconds >= 0 && timeEnd.TotalSeconds <= 0)
                        {
                            result = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// get first day in month of <paramref name="dt"/>, default time to 12:00:00 am
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime FirstDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// get last day in month of <paramref name="dt"/>, default time to 12:00:00 am
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime LastDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}

