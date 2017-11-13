namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;
    using System.Windows;

    internal class ZipIOModeEnforcingStream : Stream, IDisposable
    {
        private FileAccess _access;
        private Stream _baseStream;
        private ZipIOLocalFileBlock _block;
        private ZipIOBlockManager _blockManager;
        private long _currentStreamPosition;
        private bool _disposedFlag;

        internal ZipIOModeEnforcingStream(Stream baseStream, FileAccess access, ZipIOBlockManager blockManager, ZipIOLocalFileBlock block)
        {
            this._baseStream = baseStream;
            this._access = access;
            this._blockManager = blockManager;
            this._block = block;
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposedFlag)
                {
                    this._disposedFlag = true;
                    this._block.DeregisterExposedStream(this);
                    if ((this._access == FileAccess.ReadWrite) || (this._access == FileAccess.Write))
                    {
                        this._blockManager.SaveStream(this._block, true);
                    }
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num2;
            this.CheckDisposed();
            if (!this.CanRead)
            {
                throw new NotSupportedException(System.Windows.SR.Get("ReadNotSupported"));
            }
            long num = this._currentStreamPosition;
            try
            {
                this._baseStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
                num2 = this._baseStream.Read(buffer, offset, count);
                this._currentStreamPosition += num2;
            }
            catch
            {
                this._currentStreamPosition = num;
                throw;
            }
            return num2;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            long num = this._currentStreamPosition;
            if (origin == SeekOrigin.Begin)
            {
                num = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                num += offset;
            }
            else
            {
                if (origin != SeekOrigin.End)
                {
                    throw new ArgumentOutOfRangeException("origin");
                }
                num = this.Length + offset;
            }
            if (num < 0L)
            {
                throw new ArgumentException(System.Windows.SR.Get("SeekNegative"));
            }
            this._currentStreamPosition = num;
            return this._currentStreamPosition;
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (!this.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("SetLengthNotSupported"));
            }
            this._baseStream.SetLength(newLength);
            if (newLength < this._currentStreamPosition)
            {
                this._currentStreamPosition = newLength;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            if (!this.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("WriteNotSupported"));
            }
            if (this._baseStream.CanSeek)
            {
                this._baseStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
            }
            this._baseStream.Write(buffer, offset, count);
            this._currentStreamPosition += count;
        }

        public override bool CanRead
        {
            get
            {
                if (this._disposedFlag || !this._baseStream.CanRead)
                {
                    return false;
                }
                if (this._access != FileAccess.Read)
                {
                    return (this._access == FileAccess.ReadWrite);
                }
                return true;
            }
        }

        public override bool CanSeek =>
            (!this._disposedFlag && this._baseStream.CanSeek);

        public override bool CanWrite
        {
            get
            {
                if (this._disposedFlag || !this._baseStream.CanWrite)
                {
                    return false;
                }
                if (this._access != FileAccess.Write)
                {
                    return (this._access == FileAccess.ReadWrite);
                }
                return true;
            }
        }

        internal bool Disposed =>
            this._disposedFlag;

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._currentStreamPosition;
            }
            set
            {
                this.CheckDisposed();
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

