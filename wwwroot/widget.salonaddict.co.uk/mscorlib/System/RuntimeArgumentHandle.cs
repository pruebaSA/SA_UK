namespace System
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct RuntimeArgumentHandle
    {
        private IntPtr m_ptr;
        internal IntPtr Value =>
            this.m_ptr;
    }
}

