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
        /// convert Exception object to Customize String
        /// <para>return <paramref name="ex"/> + <paramref name="ex"/>.InnerException + <paramref name="ex"/>.InnerExeption.InnerException</para>
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToCustomizeString(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            if (null != ex)
            {
                sb.AppendLine("Exception Detail:");
                sb.AppendLine("Error Message : " + ex.Message);
                sb.AppendLine("Exception Type : " + ex.GetType());
                sb.AppendLine("Stack Trace : " + ex.StackTrace);

                if (null != ex.InnerException)
                {
                    var ex1 = ex.InnerException;
                    sb.AppendLine("Inner Exception Detail:");
                    sb.AppendLine((string)("Error Message : " + ex1.Message));
                    sb.AppendLine("Exception Type : " + ex1.GetType());
                    sb.AppendLine((string)("Stack Trace : " + ex1.StackTrace));

                    if (null != ex1.InnerException)
                    {
                        var ex2 = ex1.InnerException;
                        sb.AppendLine("Inner Exception Detail:");
                        sb.AppendLine((string)("Error Message : " + ex2.Message));
                        sb.AppendLine("Exception Type : " + ex2.GetType());
                        sb.AppendLine((string)("Stack Trace : " + ex2.StackTrace));
                    }
                }
            }
            return sb.ToString();
        }
    }
}
