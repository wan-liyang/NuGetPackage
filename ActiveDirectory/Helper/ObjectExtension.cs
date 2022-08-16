using System.Collections.Generic;
using System.Linq;

namespace ActiveDirectory.Helper
{
    internal static class ObjectExtension
    {
        public static string TryGetValue(this Dictionary<string, string> keyValuePairs, string key)
        {
            string value = null;
            if (keyValuePairs != null && keyValuePairs.Count() > 0)
            {
                if (keyValuePairs.ContainsKey(key))
                {
                    value = keyValuePairs[key];
                }
            }
            return value;
        }
    }
}
