namespace System.IdentityModel
{
    using Microsoft.Win32.SafeHandles;
    using System;

    internal sealed class SafeLsaReturnBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaReturnBufferHandle() : base(true)
        {
        }

        internal SafeLsaReturnBufferHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (NativeMethods.LsaFreeReturnBuffer(base.handle) >= 0);

        internal static System.IdentityModel.SafeLsaReturnBufferHandle InvalidHandle =>
            new System.IdentityModel.SafeLsaReturnBufferHandle(IntPtr.Zero);
    }
}

