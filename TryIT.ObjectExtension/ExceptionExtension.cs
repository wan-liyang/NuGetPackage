using System;
using System.Text;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// extension method for <see cref="Exception"/>
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// structure exception detail to string, inlcude inner exception and data
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToCustomizeString(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Exception Detail:");
            AppendExceptionMessage(sb, ex);
            AppendExceptionData(sb, ex);

            // Iterate through all inner exceptions
            Exception inner = ex.InnerException;
            int innerLevel = 1;
            while (inner != null)
            {
                sb.AppendLine($"Inner Exception Level {innerLevel} Detail:");
                AppendExceptionMessage(sb, inner);
                AppendExceptionData(sb, ex);

                inner = inner.InnerException;
                innerLevel++;
            }

            return sb.ToString();
        }

        private static void AppendExceptionMessage(StringBuilder sb, Exception ex)
        {
            sb.AppendLine($"\tError Message: {ex.Message}");
            sb.AppendLine($"\tException Type: {ex.GetType()}");
            sb.AppendLine($"\tStack Trace: {ex.StackTrace}");
        }

        private static void AppendExceptionData(StringBuilder sb, Exception ex)
        {
            if (ex.Data.Count > 0)
            {
                sb.AppendLine("\tException Data:");
                foreach (var key in ex.Data.Keys)
                {
                    sb.AppendLine($"\t\t{key} : {ex.Data[key]}");
                }
            }
        }
    }
}
