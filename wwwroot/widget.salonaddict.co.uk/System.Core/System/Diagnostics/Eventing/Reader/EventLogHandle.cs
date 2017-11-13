namespace System.Diagnostics.Eventing.Reader
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityTreatAsSafe, SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class EventLogHandle : SafeHandle
    {
        private EventLogHandle() : base(IntPtr.Zero, true)
        {
        }

        internal EventLogHandle(IntPtr handle, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            NativeWrapper.EvtClose(base.handle);
            base.handle = IntPtr.Zero;
            return true;
        }

        public override bool IsInvalid
        {
            get
            {
                if (!base.IsClosed)
                {
                    return (base.handle == IntPtr.Zero);
                }
                return true;
            }
        }

        public static EventLogHandle Zero =>
            new EventLogHandle();
    }
}

