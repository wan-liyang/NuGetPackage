using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;

namespace TryIT.MicrosoftGraphApi.Helper
{
    internal static class ExtensionMethod
    {
        #region Object <=> Json
        /// <summary>
        /// covnert Json string to specific object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// convert object to Json string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string ObjectToJson(this object obj, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(obj, converters);
        }
        #endregion


        public static bool IsEquals(this string value, string compareTo)
        {
            if (value == null)
            {
                return false;
            }

            return value.Equals(compareTo, System.StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// get value from Json string and convert to specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <param name="keyPath">key name or key path
        /// <para>keyPath = key1, this return value of key1</para>
        /// <para>keyPath = key1:key2, this look for key2 under key1, then return value of key2</para>
        /// <para>keyPath = key1:key2[1]:key3, this look for second object under key2, then return value of key3</para>
        /// </param>
        /// <returns></returns>
        public static T GetJsonValue<T>(this string jsonString, string keyPath)
        {
            T defaultValue = default(T);
            if (string.IsNullOrEmpty(jsonString))
            {
                return defaultValue;
            }

            if (string.IsNullOrEmpty(keyPath))
            {
                return defaultValue;
            }

            JObject jObject = JObject.Parse(jsonString);
            if (jObject == null)
            {
                return defaultValue;
            }

            string[] jsonKeys = keyPath.Split(':');

            JToken jToken = jObject;
            foreach (string jsonKey in jsonKeys)
            {
                Regex reg = new Regex(".*(\\[\\d*\\])");
                var match = reg.Match(jsonKey);
                if (match.Success)
                {
                    string key = match.Groups[0].Value.Replace(match.Groups[1].Value, "");
                    int index = Convert.ToInt32(match.Groups[1].Value.TrimStart('[').TrimEnd(']'));

                    jToken = ((JObject)jToken).GetValue(key, StringComparison.CurrentCultureIgnoreCase)?[index];
                }
                else
                {
                    jToken = ((JObject)jToken).GetValue(jsonKey, StringComparison.CurrentCultureIgnoreCase);
                }

                if (jToken == null)
                {
                    return defaultValue;
                }
            }

            if (typeof(T).IsValueType || typeof(T) == typeof(String))
            {
                return jToken.Value<T>();
            }
            else
            {
                return jToken.ToString().JsonToObject<T>();
            }
        }

        /// <summary>
        /// convert path a\b\c\d to list of individual path
        /// 
        /// a,
        /// a\b,
        /// a\b\c,
        /// a\b\c\d,
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> ConvertPathToList(this string path)
        {
            // Split the path into individual components
            string[] components = path.TrimEnd('\\').Split('\\');

            // Use LINQ to create a list of progressively longer paths
            return components
                .Select((_, index) => string.Join("\\", components.Take(index + 1)))
                .ToList();
        }
    }
}
