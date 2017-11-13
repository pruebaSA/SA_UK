namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.IO;
    using System.Windows;

    internal class VersionedStream : Stream
    {
        private Stream _stream;
        private VersionedStreamOwner _versionOwner;

        protected VersionedStream(Stream baseStream)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            this._stream = baseStream;
            this._versionOwner = (VersionedStreamOwner) this;
        }

        internal VersionedStream(Stream baseStream, VersionedStreamOwner versionOwner)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            if (versionOwner == null)
            {
                throw new ArgumentNullException("versionOwner");
            }
            this._stream = baseStream;
            this._versionOwner = versionOwner;
        }

        protected void CheckDisposed()
        {
            if (this._stream == null)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._stream != null))
                {
                    this._stream.Close();
                }
            }
            finally
            {
                this._stream = null;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this._stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            this._versionOwner.ReadAttempt(this._stream.Length > 0L);
            return this._stream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            this.CheckDisposed();
            this._versionOwner.ReadAttempt(this._stream.Length > 0L);
            return this._stream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            return this._stream.Seek(offset, origin);
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            this._versionOwner.WriteAttempt();
            this._stream.SetLength(newLength);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            this._versionOwner.WriteAttempt();
            this._stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte b)
        {
            this.CheckDisposed();
            this._versionOwner.WriteAttempt();
            this._stream.WriteByte(b);
        }

        protected Stream BaseStream =>
            this._stream;

        public override bool CanRead =>
            (((this._stream != null) && this._stream.CanRead) && this._versionOwner.IsReadable);

        public override bool CanSeek =>
            (((this._stream != null) && this._stream.CanSeek) && this._versionOwner.IsReadable);

        public override bool CanWrite =>
            (((this._stream != null) && this._stream.CanWrite) && this._versionOwner.IsUpdatable);

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._stream.Position;
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

