namespace System.Web
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class PerfInstanceDataHandle : SafeHandle
    {
        internal PerfInstanceDataHandle() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            UnsafeNativeMethods.PerfCloseAppCounters(base.handle);
            base.handle = IntPtr.Zero;
            return true;
        }

        public override bool IsInvalid =>
            (base.handle == IntPtr.Zero);

        internal IntPtr UnsafeHandle =>
            base.handle;
    }
}

