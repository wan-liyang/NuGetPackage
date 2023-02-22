using System;
using System.Globalization;

namespace TryIT.DataConversion
{
    /// <summary>
    /// convert string value to other type of value, e.g. convert string date to datetime
    /// </summary>
    public static class ConvertString
    {
        /// <summary>
        /// Convert string to datetime, return default value if convert failed
        /// </summary>
        /// <param name="value">string value going to convert</param>
        /// <param name="defaultValue">default datetime value if convert failed</param>
        /// <param name="valueFormats">string values's format, if multiple format defined, will return with first valid format which convert success</param>
        /// <returns></returns>
        public static DateTime ToDateTimeWithDefault(this string value, DateTime defaultValue, params string[] valueFormats)
        {
            DateTime dt = value.ToDateTime(valueFormats);

            if (dt == DateTime.MinValue)
            {
                dt = defaultValue;
            }
            return dt;
        }

        /// <summary>
        /// Convert string to datetime, return 0001-01-01 if convert failed
        /// </summary>
        /// <param name="value">string value going to convert</param>
        /// <param name="valueFormats">string values's format, if multiple format defined, will return with first valid format which convert success</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value, params string[] valueFormats)
        {
            if (valueFormats == null || valueFormats.Length == 0)
            {
                return value.ToDateTime(string.Empty);
            }

            foreach (string val in valueFormats)
            {
                DateTime dt = value.ToDateTime(val);
                if (dt != DateTime.MinValue)
                {
                    return dt;
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Convert string to datetime
        /// </summary>
        /// <param name="value">string value going to convert</param>
        /// <param name="valueFormat">string values's format</param>
        /// <returns></returns>
        private static DateTime ToDateTime(this string value, string valueFormat)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                value = value.Trim();
                if (string.IsNullOrEmpty(valueFormat))
                {
                    DateTime.TryParse(value, out dt);
                }
                else
                {
                    DateTime.TryParseExact(value, valueFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                }
            }
            catch
            {
                throw;
            }
            return dt;
        }

        /// <summary>
        /// convert string value to int, return 0 if convert failed
        /// <para>if value is "0", it will be 0 as well</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// convert string value to Enum item, return defaultEnum if convert failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultEnum"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultEnum) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultEnum;
            }

            T result;

            return Enum.TryParse<T>(value, true, out result) ? result : defaultEnum;
        }

        /// <summary>
        /// try convert string value to decimal value, return null if input value is empty or convert failed, 1% will convert to 0.01
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberStyles">default to <see cref="NumberStyles.Any"/> for avoid failure for scientific value, e.g. if data from excel 0.0000159515001096899 will become 1.59515001096899E-05</param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string value, NumberStyles numberStyles = NumberStyles.Any)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (value.EndsWith("%"))
            {
                decimal output;
                if (decimal.TryParse(value.TrimEnd('%'), numberStyles, null, out output))
                {
                    return output / 100;
                }
                return null;
            }
            else
            {
                decimal output;
                if(decimal.TryParse(value, numberStyles, null, out output))
                {
                    return output;
                }
                return null;
            }
        }
    }
}
