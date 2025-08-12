using System;

namespace TryIT.MicrosoftGraphApi.Helper
{
    /// <summary>
    /// helper utility with static method
    /// </summary>
    public static class UtilityHelper
    {
        /// <summary>
        /// replace special character with _, because the sharepoint item name cannot has special character
        /// <para>replace last . to _, solve error - The file or folder name cannnot end with .</para>
        /// <para>Removes all leading and trailing white-space characters, solve error - The provided name cannot contain leading, or trailing, spaces.</para>
        /// <para>https://support.microsoft.com/en-gb/office/restrictions-and-limitations-in-onedrive-and-sharepoint-64883a5d-228e-48f5-b3d2-eb39e07630fa#invalidcharacters</para>
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string CleanItemName(string itemName)
        {
            if (itemName == null)
            {
                throw new ArgumentNullException(nameof(itemName), "Item name cannot be null.");
            }

            // " * : < > ? / \ |
            itemName = itemName
                .Replace("#", "_")
                .Replace("%", "_")
                .Replace("\"", "_")
                .Replace("*", "_")
                .Replace(":", "_")
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace("?", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace("|", "_")
                .Replace("\t", "_")
                .Replace("\r", "_")
                .Replace("\n", "_");

            if (itemName.EndsWith("."))
            {
                itemName = itemName.TrimEnd('.') + "_";
            }

            return itemName.Trim();
        }

        /// <summary>
        /// append query string into url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query">key=value</param>
        /// <returns></returns>
        public static string AppendQueryToUrl(this string url, string query)
        {
            if (!url.Contains("?"))
            {
                url += "?";
            }

            if (url.EndsWith("?"))
            {
                url += query;
            }
            else
            {
                if (url.EndsWith("&"))
                {
                    url += query;
                }
                else
                {
                    url += $"&{query}";
                }
            }

            return url;
        }
    }
}
