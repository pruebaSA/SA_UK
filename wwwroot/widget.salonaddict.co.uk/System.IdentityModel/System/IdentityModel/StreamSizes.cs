namespace System.IdentityModel
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class StreamSizes
    {
        public int header;
        public int trailer;
        public int maximumMessage;
        public int buffersCount;
        public int blockSize;
        public static readonly int SizeOf = Marshal.SizeOf(typeof(StreamSizes));
        internal unsafe StreamSizes(byte[] memory)
        {
            byte[] buffer;
            if (((buffer = memory) != null) && (buffer.Length != 0))
            {
                goto Label_0015;
            }
            fixed (void* voidRef = null)
            {
                IntPtr ptr;
                goto Label_001D;
            Label_0015:
                voidRef = buffer;
            Label_001D:
                ptr = new IntPtr(voidRef);
                this.header = Marshal.ReadInt32(ptr);
                this.trailer = Marshal.ReadInt32(ptr, 4);
                this.maximumMessage = Marshal.ReadInt32(ptr, 8);
                this.buffersCount = Marshal.ReadInt32(ptr, 12);
                this.blockSize = Marshal.ReadInt32(ptr, 0x10);
            }
        }
    }
}

