using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// extension method for Object type
    /// </summary>
    public static class ObjExtension
    {
        /// <summary>
        /// convert object value to string value, default as empty string if object value is null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringDefaultEmpty(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// deep copy by serializing it and then returning a deserialized copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        /// <summary>
        /// convert object value to boolean, return false if value is null
        /// <para>true: value of "Y", "Yes", "On" will be true</para>
        /// <para>false: value of "N", "No", "Off" will be false</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object value)
        {
            bool result = false;
            if (value != null)
            {
                result = ConvertValue<bool>(value);
            }
            return result;
        }

        /// <summary>
        /// Convert object value to specific type value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static T ConvertValue<T>(this object objValue)
        {
            T value = default(T);
            if (objValue != null && objValue != DBNull.Value)
            {
                Type type = typeof(T);
                try
                {
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                            if (string.IsNullOrEmpty(objValue.ToString()))
                            {
                                return value;
                            }
                            else if (objValue.GetType().FullName.IsEquals("System.Boolean"))
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            else if (objValue.ToString().IsIn("Y", "Yes", "On"))
                            {
                                value = (T)Convert.ChangeType(true, type);
                            }
                            else if (objValue.ToString().IsIn("N", "No", "Off"))
                            {
                                value = (T)Convert.ChangeType(false, type);
                            }
                            else
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            break;
                        case TypeCode.Int32:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            if (string.IsNullOrEmpty(objValue.ToString()))
                            {
                                return value;
                            }
                            else
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            break;
                        default:
                            value = (T)Convert.ChangeType(objValue, type);
                            break;
                    }
                }
                catch
                {
                    throw;
                }
            }
            return value;
        }

        #region Object <=> Base64
        /// <summary>
        /// Convert object to Base64 string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToBase64String(this object obj)
        {
            string json = JsonConvert.SerializeObject(obj);

            byte[] bytes = Encoding.Default.GetBytes(json);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert Base64 string to specific object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringBase64"></param>
        /// <returns></returns>
        public static T Base64StringToObj<T>(this string stringBase64)
        {
            byte[] bytes = Convert.FromBase64String(stringBase64);

            string json = Encoding.Default.GetString(bytes);

            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

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
        /// <returns></returns>
        public static string ObjectToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
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

            string[] jsonKeys = keyPath.Split(":");

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

        #endregion
    }
}
