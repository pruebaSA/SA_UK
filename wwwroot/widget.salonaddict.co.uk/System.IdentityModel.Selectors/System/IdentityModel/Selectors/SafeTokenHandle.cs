namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal class SafeTokenHandle : SafeHandle
    {
        private SafeTokenHandle() : base(IntPtr.Zero, true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        public static extern int FreeToken([In] IntPtr token);
        protected override bool ReleaseHandle() => 
            (FreeToken(base.handle) != 0);

        public override bool IsInvalid =>
            (IntPtr.Zero == base.handle);
    }
}

