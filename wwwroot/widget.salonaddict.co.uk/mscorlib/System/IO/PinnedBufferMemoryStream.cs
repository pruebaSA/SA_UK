namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class PinnedBufferMemoryStream : UnmanagedMemoryStream
    {
        private byte[] _array;
        private GCHandle _pinningHandle;

        internal unsafe PinnedBufferMemoryStream(byte[] array)
        {
            int length = array.Length;
            if (length == 0)
            {
                array = new byte[1];
                length = 0;
            }
            this._array = array;
            this._pinningHandle = new GCHandle(array, GCHandleType.Pinned);
            fixed (byte* numRef = this._array)
            {
                base.Initialize(numRef, (long) length, (long) length, FileAccess.Read, true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (base._isOpen)
            {
                this._pinningHandle.Free();
                base._isOpen = false;
            }
            base.Dispose(disposing);
        }

        ~PinnedBufferMemoryStream()
        {
            this.Dispose(false);
        }
    }
}

