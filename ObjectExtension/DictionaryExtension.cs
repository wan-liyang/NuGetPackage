using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectExtension
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// check whether string dictionary contains provided key, default ignore case, return false if key is null
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsContainsKey(this Dictionary<string, string> dic, string key, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (dic == null || dic.Count == 0 || key == null)
            {
                return false;
            }
            return dic.Keys.Any(p => p.IsEquals(key, comparisonType));
        }
        /// <summary>
        /// check whether string dictionary contains provided value, default ignore case, return false if value is null
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsContainsValue(this Dictionary<string, string> dic, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (dic == null || dic.Count == 0 || value == null)
            {
                return false;
            }
            return dic.Values.Any(p => p.IsEquals(value, comparisonType));
        }

        /// <summary>
        /// try get value from Dictionary, return null if not exists
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TryGetValue(this Dictionary<string, string> keyValuePairs, string key)
        {
            string value = null;
            if (keyValuePairs != null && keyValuePairs.Count() > 0)
            {
                if (keyValuePairs.IsContainsKey(key))
                {
                    value = keyValuePairs[key];
                }
            }
            return value;
        }
    }
}
