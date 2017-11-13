namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct SocketAddress
    {
        private IntPtr sockAddr;
        private int sockAddrLength;
        public IntPtr SockAddr =>
            this.sockAddr;
        public int SockAddrLength =>
            this.sockAddrLength;
        public void InitializeFromCriticalAllocHandleSocketAddress(CriticalAllocHandleSocketAddress sockAddr)
        {
            this.sockAddr = (IntPtr) sockAddr;
            this.sockAddrLength = sockAddr.Size;
        }
    }
}

