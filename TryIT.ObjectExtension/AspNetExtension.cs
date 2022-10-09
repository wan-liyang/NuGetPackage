using System;
namespace TryIT.ObjectExtension
{
    /// <summary>
    /// the common extension method for AspNet
    /// </summary>
    public static class AspNetExtension
    {
        /// <summary>
        /// get caller Assembly namespace
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentNamespace()
        {
            return System.Reflection.Assembly.GetCallingAssembly().EntryPoint.DeclaringType.Namespace;
        }
    }
}
