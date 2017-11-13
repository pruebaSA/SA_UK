namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;

    [FriendAccessAllowed]
    internal static class CriticalExceptions
    {
        [FriendAccessAllowed]
        internal static bool IsCriticalException(Exception ex) => 
            (((((ex is NullReferenceException) || (ex is StackOverflowException)) || ((ex is OutOfMemoryException) || (ex is ThreadAbortException))) || (ex is SEHException)) || (ex is SecurityException));
    }
}

