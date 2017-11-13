namespace System.Runtime.Remoting.Channels.Ipc
{
    using Microsoft.Win32.SafeHandles;
    using System;

    internal class PipeHandle : CriticalHandleMinusOneIsInvalid
    {
        internal PipeHandle()
        {
        }

        internal PipeHandle(IntPtr handle)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (NativePipe.CloseHandle(base.handle) != 0);

        public IntPtr Handle =>
            base.handle;
    }
}

