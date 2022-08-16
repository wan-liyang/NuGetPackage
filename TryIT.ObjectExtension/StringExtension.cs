using System;
using System.Globalization;
using System.Linq;

namespace TryIT.ObjectExtension
{
    public static class StringExtension
    {
        /// <summary>
        /// check whether string is equals to provide value, default ignore case, return false if value is null
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <param name="isTrim">whether use string.Trim() to remove space.</param>
        /// <returns></returns>
        public static bool IsEquals(this string sourceValue, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase, bool isTrim = true)
        {
            if (sourceValue == null || value == null)
            {
                return false;
            }
            if (isTrim)
            {
                return sourceValue.Trim().Equals(value.Trim(), comparisonType);
            }
            return sourceValue.Equals(value, comparisonType);
        }

        /// <summary>
        /// check whether this string value exists in values list, ignore case
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIn(this string value, params string[] values)
        {
            if (values == null || values.Count() == 0)
            {
                return false;
            }
            return values.IsContains(value);
        }

        /// <summary>
        /// check whether string contains provided value, default ignore case, return false if value is null
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsContains(this string sourceValue, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (sourceValue == null || value == null)
            {
                return false;
            }
            if (comparisonType == StringComparison.CurrentCultureIgnoreCase
                || comparisonType == StringComparison.InvariantCultureIgnoreCase
                || comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                return sourceValue.ToUpper().Contains(value.ToUpper());
            }
            return sourceValue.Contains(value);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string, default ignore case
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
        public static bool IsStartsWith(this string sourceValue, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (sourceValue == null || value == null)
            {
                return false;
            }
            if (comparisonType == StringComparison.CurrentCultureIgnoreCase
                || comparisonType == StringComparison.InvariantCultureIgnoreCase
                || comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                return sourceValue.ToUpper().StartsWith(value.ToUpper());
            }
            return sourceValue.StartsWith(value, comparisonType);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches the specified string, default ignore case
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
        public static bool IsEndsWith(this string sourceValue, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (sourceValue == null || value == null)
            {
                return false;
            }
            if (comparisonType == StringComparison.CurrentCultureIgnoreCase
                || comparisonType == StringComparison.InvariantCultureIgnoreCase
                || comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                return sourceValue.ToUpper().EndsWith(value.ToUpper());
            }
            return sourceValue.EndsWith(value, comparisonType);
        }

        /// <summary>
        /// split string <paramref name="value"/> by <paramref name="separator"/>, will remove empty item
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(this string value, params string[] separator)
        {
            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] Split(this string value, bool isRemoveDuplicate, params string[] separator)
        {
            string[] arr = value.Split(separator);
            if (isRemoveDuplicate)
            {
                arr = arr.Distinct().ToArray();
            }
            return arr;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current System.String object
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <param name="isWholeWord">true to match whold world during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is System.String.Empty, the return value is startIndex.
        /// </returns>
        public static int IndexOf(this string str, string value, int startIndex = 0, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase, bool isWholeWord = false)
        {
            if (isWholeWord)
            {
                if (startIndex < 0)
                {
                    startIndex = 0;
                }
                for (int j = startIndex; j < str.Length && (j = str.IndexOf(value, j, comparisonType)) >= 0; j++)
                {
                    if (
                        (
                            j == 0
                            || !char.IsLetterOrDigit(str, j - 1)
                        )
                        && (
                            j + value.Length == str.Length
                            || !char.IsLetterOrDigit(str, j + value.Length)
                            )
                       )
                    {
                        return j;
                    }
                }
            }
            else
            {
                return str.IndexOf(value, startIndex, comparisonType);
            }
            return -1;
        }
    }
}
