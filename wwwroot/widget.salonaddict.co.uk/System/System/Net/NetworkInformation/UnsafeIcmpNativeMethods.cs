namespace System.Net.NetworkInformation
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeIcmpNativeMethods
    {
        private const string ICMP = "icmp.dll";

        [DllImport("icmp.dll", SetLastError=true)]
        internal static extern bool IcmpCloseHandle(IntPtr icmpHandle);
        [DllImport("icmp.dll", SetLastError=true)]
        internal static extern SafeCloseIcmpHandle IcmpCreateFile();
        [DllImport("icmp.dll", SetLastError=true)]
        internal static extern uint IcmpParseReplies(IntPtr replyBuffer, uint replySize);
        [DllImport("icmp.dll", SetLastError=true)]
        internal static extern uint IcmpSendEcho2(SafeCloseIcmpHandle icmpHandle, SafeWaitHandle Event, IntPtr apcRoutine, IntPtr apcContext, uint ipAddress, [In] SafeLocalFree data, ushort dataSize, ref IPOptions options, SafeLocalFree replyBuffer, uint replySize, uint timeout);
        [DllImport("icmp.dll", SetLastError=true)]
        internal static extern uint IcmpSendEcho2(SafeCloseIcmpHandle icmpHandle, IntPtr Event, IntPtr apcRoutine, IntPtr apcContext, uint ipAddress, [In] SafeLocalFree data, ushort dataSize, ref IPOptions options, SafeLocalFree replyBuffer, uint replySize, uint timeout);
    }
}

