using Microsoft.Win32.SafeHandles;
using System;
using System.Security.Principal;

namespace Impersonation.Win
{
    /*
        this class is refer to https://github.com/mj1856/SimpleImpersonation
        example code:
        ==============================================================================================
        var userNameBefore = WindowsIdentity.GetCurrent().Name;
        string userNameDuring = null;
        Impersonation.RunAsUser("domain", "username", "password", LogonType.Interactive, () =>
        {
            userNameDuring = WindowsIdentity.GetCurrent().Name;
            // do whatever you want as this user in here.
        });

        var userNameAfter = WindowsIdentity.GetCurrent().Name;

        Console.WriteLine("Before: " + userNameBefore);
        Console.WriteLine("During: " + userNameDuring);
        Console.WriteLine("After: " + userNameAfter);
        ==============================================================================================

        will get result below => 
        Before: [current login]
        During: [domain\username]
        After: [current login]
    */

    /// <summary>
    /// Impersonation Library, use function <see cref="RunAsUser"/> to perform action, refer to above code example
    /// </summary>
    public static class Impersonation
    {
        /// <summary>
        /// Impersonates a specific user account to perform the specified action, refer to above code example.
        /// </summary>
        /// <param name="domain">The domain for credentials of the user account to impersonate.</param>
        /// <param name="username">The username for credentials of the user account to impersonate.</param>
        /// <param name="password">The password for credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        public static void RunAsUser(string domain, string username, string password, LogonType logonType, Action action)
        {
            UserCredentials credentials = new UserCredentials(domain, username, password);

            using (var userHandle = credentials.LogonUser(logonType))
            {
                RunImpersonated(userHandle, _ => action());
            }
        }

        private static void RunImpersonated(SafeAccessTokenHandle tokenHandle, Action<SafeAccessTokenHandle> action)
        {
            WindowsIdentity.RunImpersonated(tokenHandle, () => action(tokenHandle));
        }
    }

    ///// <summary>
    ///// Exception thrown when impersonation fails.
    ///// </summary>
    ///// <remarks>
    ///// Inherits from <see cref="ApplicationException"/> for backwards compatibility reasons.
    ///// </remarks>
    //internal class ImpersonationException : ApplicationException
    //{
    //    private readonly Win32Exception _win32Exception;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ImpersonationException"/> class from a specific <see cref="Win32Exception"/>.
    //    /// </summary>
    //    /// <param name="win32Exception">The exception to base this exception on.</param>
    //    public ImpersonationException(Win32Exception win32Exception)
    //        : base(win32Exception.Message, win32Exception)
    //    {
    //        // Note that the Message is generated inside the Win32Exception class via the Win32 FormatMessage function.

    //        _win32Exception = win32Exception;
    //    }

    //    /// <summary>
    //    /// Returns the Win32 error code handle for the exception.
    //    /// </summary>
    //    public int ErrorCode => _win32Exception.ErrorCode;

    //    /// <summary>
    //    /// Returns the Win32 native error code for the exception.
    //    /// </summary>
    //    public int NativeErrorCode => _win32Exception.NativeErrorCode;
    //}
}
