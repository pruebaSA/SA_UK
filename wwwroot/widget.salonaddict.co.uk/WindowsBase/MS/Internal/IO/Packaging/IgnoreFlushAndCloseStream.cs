namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;
    using System.Windows;

    internal sealed class IgnoreFlushAndCloseStream : Stream
    {
        private bool _disposed;
        private Stream _stream;

        internal IgnoreFlushAndCloseStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this._stream = stream;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!this._disposed)
                {
                    this._stream = null;
                    this._disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.ThrowIfStreamDisposed();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.ThrowIfStreamDisposed();
            return this._stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.ThrowIfStreamDisposed();
            return this._stream.Seek(offset, origin);
        }

        public override void SetLength(long newLength)
        {
            this.ThrowIfStreamDisposed();
            this._stream.SetLength(newLength);
        }

        private void ThrowIfStreamDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        public override void Write(byte[] buf, int offset, int count)
        {
            this.ThrowIfStreamDisposed();
            this._stream.Write(buf, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                if (this._disposed)
                {
                    return false;
                }
                return this._stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (this._disposed)
                {
                    return false;
                }
                return this._stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (this._disposed)
                {
                    return false;
                }
                return this._stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                this.ThrowIfStreamDisposed();
                return this._stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.ThrowIfStreamDisposed();
                return this._stream.Position;
            }
            set
            {
                this.ThrowIfStreamDisposed();
                this._stream.Position = value;
            }
        }
    }
}

