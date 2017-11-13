namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;
    using System.Windows;

    internal class WriteTimeStream : Stream
    {
        private Stream _baseStream;
        private long _position;

        internal WriteTimeStream(Stream baseStream)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            this._baseStream = baseStream;
            if (!this._baseStream.CanWrite)
            {
                throw new ArgumentException(System.Windows.SR.Get("WriteNotSupported"), "baseStream");
            }
        }

        private void CheckDisposed()
        {
            if (this._baseStream == null)
            {
                throw new ObjectDisposedException("Stream");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._baseStream != null))
                {
                    this._baseStream.Close();
                }
            }
            finally
            {
                this._baseStream = null;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this._baseStream.Flush();
        }

        private static void IllegalAccess()
        {
            throw new NotSupportedException(System.Windows.SR.Get("WriteOnlyStream"));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            IllegalAccess();
            return -1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            IllegalAccess();
            return -1L;
        }

        public override void SetLength(long newLength)
        {
            IllegalAccess();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            this._baseStream.Write(buffer, offset, count);
            this._position += count;
        }

        public override bool CanRead =>
            false;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            (this._baseStream != null);

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._position;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._position;
            }
            set
            {
                this.CheckDisposed();
                IllegalAccess();
            }
        }
    }
}

