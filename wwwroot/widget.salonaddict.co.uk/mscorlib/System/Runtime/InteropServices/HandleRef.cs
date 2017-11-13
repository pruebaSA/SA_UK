namespace System.Runtime.InteropServices
{
    using System;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct HandleRef
    {
        internal object m_wrapper;
        internal IntPtr m_handle;
        public HandleRef(object wrapper, IntPtr handle)
        {
            this.m_wrapper = wrapper;
            this.m_handle = handle;
        }

        public object Wrapper =>
            this.m_wrapper;
        public IntPtr Handle =>
            this.m_handle;
        public static explicit operator IntPtr(HandleRef value) => 
            value.m_handle;

        public static IntPtr ToIntPtr(HandleRef value) => 
            value.m_handle;
    }
}

