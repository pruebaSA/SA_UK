﻿namespace System.Web.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Web.Services.Interop;

    [ComVisible(false), SuppressUnmanagedCodeSecurity, SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    internal class UnsafeNativeMethods
    {
        private UnsafeNativeMethods()
        {
        }

        [DllImport("ole32.dll", ExactSpelling=true)]
        internal static extern int CoCreateInstance([In] ref Guid clsid, [MarshalAs(UnmanagedType.Interface)] object punkOuter, int context, [In] ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object punk);
        internal static void OnSyncCallEnter(INotifySink2 sink, CallId callId, byte[] in_pBuffer, int in_BufferSize)
        {
            sink.OnSyncCallEnter(callId, in_pBuffer, in_BufferSize);
        }

        internal static void OnSyncCallExit(INotifySink2 sink, CallId callId, out IntPtr out_ppBuffer, ref int inout_pBufferSize)
        {
            sink.OnSyncCallExit(callId, out out_ppBuffer, ref inout_pBufferSize);
        }

        internal static void OnSyncCallOut(INotifySink2 sink, CallId callId, out IntPtr out_ppBuffer, ref int inout_pBufferSize)
        {
            sink.OnSyncCallOut(callId, out out_ppBuffer, ref inout_pBufferSize);
        }

        internal static void OnSyncCallReturn(INotifySink2 sink, CallId callId, byte[] in_pBuffer, int in_BufferSize)
        {
            sink.OnSyncCallReturn(callId, in_pBuffer, in_BufferSize);
        }

        internal static INotifySink2 RegisterNotifySource(INotifyConnection2 connection, INotifySource2 source) => 
            connection.RegisterNotifySource(source);

        internal static void UnregisterNotifySource(INotifyConnection2 connection, INotifySource2 source)
        {
            connection.UnregisterNotifySource(source);
        }
    }
}

