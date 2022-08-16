namespace TryIT.CryptoService
{
    internal class UrlHelper
    {
        /// <summary>
        /// Standard Base64 may contain '/','+' it will show %xx in Url, use this function to conver to readable string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string UrlBeauty_E(string source)
        {
            return source.Replace("+", "-").Replace("/", "_").Replace("=", ".");
        }
        public static string UrlBeauty_D(string source)
        {
            return source.Replace("-", "+").Replace("_", "/").Replace(".", "=");
        }
    }
}
