namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class UnsafeNativeMethods
    {
        [DllImport("wininet.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern int InternetGetCookieW(string uri, string cookieName, StringBuilder cookieValue, ref int dwSize);
        [DllImport("wininet.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern int InternetSetCookieW(string uri, string cookieName, string cookieValue);
    }
}

