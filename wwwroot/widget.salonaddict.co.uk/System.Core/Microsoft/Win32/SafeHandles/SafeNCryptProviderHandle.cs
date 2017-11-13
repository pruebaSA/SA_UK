﻿namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Contracts;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [SecurityCritical(SecurityCriticalScope.Everything), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
    public sealed class SafeNCryptProviderHandle : SafeNCryptHandle
    {
        internal SafeNCryptProviderHandle Duplicate() => 
            base.Duplicate<SafeNCryptProviderHandle>();

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("ncrypt.dll")]
        private static extern int NCryptFreeObject(IntPtr hObject);
        protected override bool ReleaseNativeHandle() => 
            (NCryptFreeObject(base.handle) == 0);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void SetHandleValue(IntPtr newHandleValue)
        {
            Contract.Requires(newHandleValue != IntPtr.Zero);
            Contract.Requires(!base.IsClosed);
            base.SetHandle(newHandleValue);
        }
    }
}

