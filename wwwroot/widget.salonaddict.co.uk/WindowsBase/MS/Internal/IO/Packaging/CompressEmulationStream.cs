namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;
    using System.Windows;

    internal class CompressEmulationStream : Stream
    {
        protected Stream _baseStream;
        private bool _dirty;
        private bool _disposed;
        protected Stream _tempStream;
        private IDeflateTransform _transformer;

        internal CompressEmulationStream(Stream baseStream, Stream tempStream, long position, IDeflateTransform transformer)
        {
            if (position < 0L)
            {
                throw new ArgumentOutOfRangeException("position");
            }
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            if (!baseStream.CanSeek)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("SeekNotSupported"));
            }
            if (!baseStream.CanRead)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("ReadNotSupported"));
            }
            if (tempStream == null)
            {
                throw new ArgumentNullException("tempStream");
            }
            if (transformer == null)
            {
                throw new ArgumentNullException("transfomer");
            }
            this._baseStream = baseStream;
            this._tempStream = tempStream;
            this._transformer = transformer;
            this._baseStream.Position = 0L;
            this._tempStream.Position = 0L;
            this._transformer.Decompress(baseStream, tempStream);
            this._tempStream.Position = position;
        }

        protected void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposed)
                {
                    this.Flush();
                    this._tempStream.Close();
                    this._tempStream = null;
                    this._baseStream = null;
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
            this.CheckDisposed();
            if (this._dirty)
            {
                long position = this._tempStream.Position;
                this._tempStream.Position = 0L;
                this._baseStream.Position = 0L;
                this._transformer.Compress(this._tempStream, this._baseStream);
                this._tempStream.Position = position;
                this._baseStream.Flush();
                this._dirty = false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            return this._tempStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            long num = 0L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    num = offset;
                    break;

                case SeekOrigin.Current:
                    num = this._tempStream.Position + offset;
                    break;

                case SeekOrigin.End:
                    num = this._tempStream.Length + offset;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("origin", System.Windows.SR.Get("SeekOriginInvalid"));
            }
            if (num < 0L)
            {
                throw new ArgumentException(System.Windows.SR.Get("SeekNegative"));
            }
            return this._tempStream.Seek(offset, origin);
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            this._tempStream.SetLength(newLength);
            if (newLength < this._tempStream.Position)
            {
                this._tempStream.Position = newLength;
            }
            this._dirty = true;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            if (count != 0)
            {
                this._tempStream.Write(buffer, offset, count);
                this._dirty = true;
            }
        }

        public override bool CanRead =>
            (!this._disposed && this._baseStream.CanRead);

        public override bool CanSeek =>
            (!this._disposed && this._baseStream.CanSeek);

        public override bool CanWrite =>
            (!this._disposed && this._baseStream.CanWrite);

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._tempStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._tempStream.Position;
            }
            set
            {
                this.CheckDisposed();
                if (value < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("SeekNegative"));
                }
                this._tempStream.Position = value;
            }
        }
    }
}

