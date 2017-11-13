using Microsoft.Win32.SafeHandles;
using System;
using System.ServiceProcess;

internal class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    internal SafeServiceHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
    {
        base.SetHandle(handle);
    }

    protected override bool ReleaseHandle() => 
        SafeNativeMethods.CloseServiceHandle(base.handle);
}

