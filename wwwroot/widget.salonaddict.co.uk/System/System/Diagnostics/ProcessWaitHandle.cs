namespace System.Diagnostics
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class ProcessWaitHandle : WaitHandle
    {
        internal ProcessWaitHandle(Microsoft.Win32.SafeHandles.SafeProcessHandle processHandle)
        {
            SafeWaitHandle targetHandle = null;
            if (!NativeMethods.DuplicateHandle(new HandleRef(this, NativeMethods.GetCurrentProcess()), processHandle, new HandleRef(this, NativeMethods.GetCurrentProcess()), out targetHandle, 0, false, 2))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            base.SafeWaitHandle = targetHandle;
        }
    }
}

