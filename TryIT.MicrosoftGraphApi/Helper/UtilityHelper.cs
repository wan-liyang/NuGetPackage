namespace TryIT.MicrosoftGraphApi.Helper
{
    /// <summary>
    /// helper utility with static method
    /// </summary>
    public static class UtilityHelper
    {
        /// <summary>
        /// https://support.microsoft.com/en-gb/office/restrictions-and-limitations-in-onedrive-and-sharepoint-64883a5d-228e-48f5-b3d2-eb39e07630fa#invalidcharacters
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string CleanItemName(string itemName)
        {
            // " * : < > ? / \ |
            return itemName
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
        }
    }
}
