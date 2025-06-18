using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// extension class for HttpResponseMessage
    /// </summary>
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Extension method to get the status code of an HttpResponseMessage as a formatted string, e.g. "200 - OK".
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetStatusCodeString(this System.Net.Http.HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response), "Response cannot be null");
            }

            return $"{(int)response.StatusCode} - {response.StatusCode}";
        }
    }
}
