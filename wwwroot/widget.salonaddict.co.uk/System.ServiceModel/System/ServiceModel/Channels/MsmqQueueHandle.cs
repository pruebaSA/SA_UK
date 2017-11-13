namespace System.ServiceModel.Channels
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal sealed class MsmqQueueHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal MsmqQueueHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle() => 
            (UnsafeNativeMethods.MQCloseQueue(base.handle) >= 0);
    }
}

