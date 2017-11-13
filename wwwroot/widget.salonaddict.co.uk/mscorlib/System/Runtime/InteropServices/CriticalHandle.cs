namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true)]
    public abstract class CriticalHandle : CriticalFinalizerObject, IDisposable
    {
        private bool _isClosed;
        protected IntPtr handle;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected CriticalHandle(IntPtr invalidHandleValue)
        {
            this.handle = invalidHandleValue;
            this._isClosed = false;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private void Cleanup()
        {
            if (!this.IsClosed)
            {
                this._isClosed = true;
                if (!this.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (!this.ReleaseHandle())
                    {
                        this.FireCustomerDebugProbe();
                    }
                    Marshal.SetLastWin32Error(error);
                    GC.SuppressFinalize(this);
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Close()
        {
            this.Dispose(true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Dispose()
        {
            this.Dispose(true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected virtual void Dispose(bool disposing)
        {
            this.Cleanup();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        ~CriticalHandle()
        {
            this.Dispose(false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void FireCustomerDebugProbe();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected abstract bool ReleaseHandle();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected void SetHandle(IntPtr handle)
        {
            this.handle = handle;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void SetHandleAsInvalid()
        {
            this._isClosed = true;
            GC.SuppressFinalize(this);
        }

        public bool IsClosed =>
            this._isClosed;

        public abstract bool IsInvalid { [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] get; }
    }
}

