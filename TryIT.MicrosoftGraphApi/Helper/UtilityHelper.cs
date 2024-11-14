namespace TryIT.MicrosoftGraphApi.Helper
{
    /// <summary>
    /// helper utility with static method
    /// </summary>
    public static class UtilityHelper
    {
        /// <summary>
        /// replace special character and end . with _, because the sharepoint item name cannot has special character and end with .
        /// <para>https://support.microsoft.com/en-gb/office/restrictions-and-limitations-in-onedrive-and-sharepoint-64883a5d-228e-48f5-b3d2-eb39e07630fa#invalidcharacters</para>
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string CleanItemName(string itemName)
        {
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
                .Replace("|", "_");

            if (itemName.EndsWith("."))
            {
                itemName = itemName.TrimEnd('.') + "_";
            }

            return itemName;
        }
    }
}
