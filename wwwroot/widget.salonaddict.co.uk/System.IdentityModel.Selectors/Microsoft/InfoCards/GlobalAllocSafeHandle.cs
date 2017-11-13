namespace Microsoft.InfoCards
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal class GlobalAllocSafeHandle : SafeHandle
    {
        private int m_bytes;

        private GlobalAllocSafeHandle() : base(IntPtr.Zero, true)
        {
            this.m_bytes = 0;
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", CallingConvention=CallingConvention.StdCall)]
        public static extern IntPtr GlobalFree(IntPtr hMem);
        protected override bool ReleaseHandle()
        {
            if (this.m_bytes > 0)
            {
                ZeroMemory(base.handle, this.m_bytes);
                GlobalFree(base.handle);
                this.m_bytes = 0;
            }
            return true;
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("Kernel32.dll", EntryPoint="RtlZeroMemory")]
        public static extern void ZeroMemory(IntPtr dest, int size);

        public override bool IsInvalid =>
            (IntPtr.Zero == base.handle);

        public int Length
        {
            get => 
                this.m_bytes;
            set
            {
                this.m_bytes = value;
            }
        }
    }
}

