namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class UnmanagedMemoryStreamWrapper : MemoryStream
    {
        private UnmanagedMemoryStream _unmanagedStream;

        internal UnmanagedMemoryStreamWrapper(UnmanagedMemoryStream stream)
        {
            this._unmanagedStream = stream;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this._unmanagedStream.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this._unmanagedStream.Flush();
        }

        public override byte[] GetBuffer()
        {
            throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_MemStreamBuffer"));
        }

        public override int Read([In, Out] byte[] buffer, int offset, int count) => 
            this._unmanagedStream.Read(buffer, offset, count);

        public override int ReadByte() => 
            this._unmanagedStream.ReadByte();

        public override long Seek(long offset, SeekOrigin loc) => 
            this._unmanagedStream.Seek(offset, loc);

        public override unsafe byte[] ToArray()
        {
            if (!this._unmanagedStream._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if (!this._unmanagedStream.CanRead)
            {
                __Error.ReadNotSupported();
            }
            byte[] dest = new byte[this._unmanagedStream.Length];
            Buffer.memcpy(this._unmanagedStream.Pointer, 0, dest, 0, (int) this._unmanagedStream.Length);
            return dest;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._unmanagedStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            this._unmanagedStream.WriteByte(value);
        }

        public override void WriteTo(Stream stream)
        {
            if (!this._unmanagedStream._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if (!this._unmanagedStream.CanRead)
            {
                __Error.ReadNotSupported();
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream", Environment.GetResourceString("ArgumentNull_Stream"));
            }
            byte[] buffer = this.ToArray();
            stream.Write(buffer, 0, buffer.Length);
        }

        public override bool CanRead =>
            this._unmanagedStream.CanRead;

        public override bool CanSeek =>
            this._unmanagedStream.CanSeek;

        public override bool CanWrite =>
            this._unmanagedStream.CanWrite;

        public override int Capacity
        {
            get => 
                ((int) this._unmanagedStream.Capacity);
            set
            {
                throw new IOException(Environment.GetResourceString("IO.IO_FixedCapacity"));
            }
        }

        public override long Length =>
            this._unmanagedStream.Length;

        public override long Position
        {
            get => 
                this._unmanagedStream.Position;
            set
            {
                this._unmanagedStream.Position = value;
            }
        }
    }
}

