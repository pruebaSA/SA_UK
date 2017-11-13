namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct GCHandle
    {
        internal static readonly IntPtr InvalidCookie;
        private IntPtr m_handle;
        private static GCHandleCookieTable s_cookieTable;
        private static bool s_probeIsActive;
        static GCHandle()
        {
            InvalidCookie = new IntPtr(-1);
            s_cookieTable = null;
            s_probeIsActive = false;
            s_probeIsActive = Mda.IsInvalidGCHandleCookieProbeEnabled();
            if (s_probeIsActive)
            {
                s_cookieTable = new GCHandleCookieTable();
            }
        }

        internal GCHandle(object value, GCHandleType type)
        {
            this.m_handle = InternalAlloc(value, type);
            if (type == GCHandleType.Pinned)
            {
                this.SetIsPinned();
            }
        }

        internal GCHandle(IntPtr handle)
        {
            InternalCheckDomain(handle);
            this.m_handle = handle;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static GCHandle Alloc(object value) => 
            new GCHandle(value, GCHandleType.Normal);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static GCHandle Alloc(object value, GCHandleType type) => 
            new GCHandle(value, type);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public void Free()
        {
            IntPtr handle = this.m_handle;
            if ((handle == IntPtr.Zero) || (Interlocked.CompareExchange(ref this.m_handle, IntPtr.Zero, handle) != handle))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
            }
            InternalFree((IntPtr) (((int) handle) & -2));
            if (s_probeIsActive)
            {
                s_cookieTable.RemoveHandleIfPresent(handle);
            }
        }

        public object Target
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                if (this.m_handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
                }
                return InternalGet(this.GetHandleValue());
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                if (this.m_handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
                }
                InternalSet(this.GetHandleValue(), value, this.IsPinned());
            }
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public IntPtr AddrOfPinnedObject()
        {
            if (this.IsPinned())
            {
                return InternalAddrOfPinnedObject(this.GetHandleValue());
            }
            if (this.m_handle == IntPtr.Zero)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
            }
            throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotPinned"));
        }

        public bool IsAllocated =>
            (this.m_handle != IntPtr.Zero);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static explicit operator GCHandle(IntPtr value) => 
            FromIntPtr(value);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static GCHandle FromIntPtr(IntPtr value)
        {
            if (value == IntPtr.Zero)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
            }
            IntPtr handle = value;
            if (s_probeIsActive)
            {
                handle = s_cookieTable.GetHandle(value);
                if (IntPtr.Zero == handle)
                {
                    Mda.FireInvalidGCHandleCookieProbe(value);
                    return new GCHandle(IntPtr.Zero);
                }
            }
            return new GCHandle(handle);
        }

        public static explicit operator IntPtr(GCHandle value) => 
            ToIntPtr(value);

        public static IntPtr ToIntPtr(GCHandle value)
        {
            if (s_probeIsActive)
            {
                return s_cookieTable.FindOrAddHandle(value.m_handle);
            }
            return value.m_handle;
        }

        public override int GetHashCode() => 
            ((int) this.m_handle);

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is GCHandle))
            {
                return false;
            }
            GCHandle handle = (GCHandle) o;
            return (this.m_handle == handle.m_handle);
        }

        public static bool operator ==(GCHandle a, GCHandle b) => 
            (a.m_handle == b.m_handle);

        public static bool operator !=(GCHandle a, GCHandle b) => 
            (a.m_handle != b.m_handle);

        internal IntPtr GetHandleValue() => 
            new IntPtr(((int) this.m_handle) & -2);

        internal bool IsPinned() => 
            ((((int) this.m_handle) & 1) != 0);

        internal void SetIsPinned()
        {
            this.m_handle = new IntPtr(((int) this.m_handle) | 1);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr InternalAlloc(object value, GCHandleType type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalFree(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object InternalGet(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalSet(IntPtr handle, object value, bool isPinned);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object InternalCompareExchange(IntPtr handle, object value, object oldValue, bool isPinned);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr InternalAddrOfPinnedObject(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCheckDomain(IntPtr handle);
    }
}

