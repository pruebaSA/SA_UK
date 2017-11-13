namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    internal class ComProxy : IDisposable
    {
        private IDisposable ccw;
        private IntPtr inner;

        internal ComProxy(IntPtr inner, IDisposable disp)
        {
            this.inner = inner;
            this.ccw = disp;
        }

        public ComProxy Clone()
        {
            if (this.inner == IntPtr.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            Marshal.AddRef(this.inner);
            return new ComProxy(this.inner, null);
        }

        internal static ComProxy Create(IntPtr outer, object obj, IDisposable disp)
        {
            if (outer == IntPtr.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            IntPtr zero = IntPtr.Zero;
            zero = Marshal.CreateAggregatedObject(outer, obj);
            int num = Marshal.AddRef(zero);
            if (3 == num)
            {
                Marshal.Release(zero);
            }
            Marshal.Release(zero);
            return new ComProxy(zero, disp);
        }

        private void Dispose(bool disposing)
        {
            if (this.inner == IntPtr.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            Marshal.Release(this.inner);
            if (disposing && (this.ccw != null))
            {
                this.ccw.Dispose();
            }
        }

        internal void QueryInterface(ref Guid riid, out IntPtr tearoff)
        {
            if (this.inner == IntPtr.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            if (Marshal.QueryInterface(this.inner, ref riid, out tearoff) != HR.S_OK)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}

