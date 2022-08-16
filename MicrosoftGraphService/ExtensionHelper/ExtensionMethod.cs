using Newtonsoft.Json;

namespace MicrosoftGraphService.ExtensionHelper
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
        /// <returns></returns>
        public static string ObjectToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion
    }
}
