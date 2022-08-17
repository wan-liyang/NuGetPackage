using Newtonsoft.Json;
using System;
using System.Text;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// extension method for Object type
    /// </summary>
    public static class ObjExtension
    {
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
                catch (Exception ex)
                {
                    throw ex;
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
        #endregion
    }
}
