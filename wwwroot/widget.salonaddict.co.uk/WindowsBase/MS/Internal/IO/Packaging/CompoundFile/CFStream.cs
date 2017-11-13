namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.IO.Packaging;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows;

    internal class CFStream : Stream
    {
        private MS.Internal.IO.Packaging.CompoundFile.IStream _safeIStream;
        private FileAccess access;
        private StreamInfo backReference;

        internal CFStream(MS.Internal.IO.Packaging.CompoundFile.IStream underlyingStream, FileAccess openAccess, StreamInfo creator)
        {
            this._safeIStream = underlyingStream;
            this.access = openAccess;
            this.backReference = creator;
        }

        internal void CheckDisposedStatus()
        {
            if (this.StreamDisposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._safeIStream != null))
                {
                    this._safeIStream = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposedStatus();
            this._safeIStream.Commit(0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposedStatus();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            int pcbRead = 0;
            if (offset == 0)
            {
                this._safeIStream.Read(buffer, count, out pcbRead);
                return pcbRead;
            }
            byte[] pv = new byte[count];
            this._safeIStream.Read(pv, count, out pcbRead);
            if (pcbRead > 0)
            {
                Array.Copy(pv, 0, buffer, offset, pcbRead);
            }
            return pcbRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposedStatus();
            if (!this.CanSeek)
            {
                throw new NotSupportedException(System.Windows.SR.Get("SeekNotSupported"));
            }
            long plibNewPosition = 0L;
            int dwOrigin = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    dwOrigin = 0;
                    if (0L > offset)
                    {
                        throw new ArgumentOutOfRangeException("offset", System.Windows.SR.Get("SeekNegative"));
                    }
                    break;

                case SeekOrigin.Current:
                    dwOrigin = 1;
                    break;

                case SeekOrigin.End:
                    dwOrigin = 2;
                    break;

                default:
                    throw new InvalidEnumArgumentException("origin", (int) origin, typeof(SeekOrigin));
            }
            this._safeIStream.Seek(offset, dwOrigin, out plibNewPosition);
            return plibNewPosition;
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposedStatus();
            if (!this.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("SetLengthNotSupported"));
            }
            if (0L > newLength)
            {
                throw new ArgumentOutOfRangeException("newLength", System.Windows.SR.Get("StreamLengthNegative"));
            }
            this._safeIStream.SetSize(newLength);
            if (newLength < this.Position)
            {
                this.Position = newLength;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposedStatus();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            int pcbWritten = 0;
            if (offset == 0)
            {
                this._safeIStream.Write(buffer, count, out pcbWritten);
            }
            else
            {
                byte[] destinationArray = new byte[count];
                Array.Copy(buffer, offset, destinationArray, 0, count);
                this._safeIStream.Write(destinationArray, count, out pcbWritten);
            }
            if (count != pcbWritten)
            {
                throw new IOException(System.Windows.SR.Get("WriteFailure"));
            }
        }

        public override bool CanRead
        {
            get
            {
                if (this.StreamDisposed)
                {
                    return false;
                }
                if (FileAccess.Read != (this.access & FileAccess.Read))
                {
                    return (FileAccess.ReadWrite == (this.access & FileAccess.ReadWrite));
                }
                return true;
            }
        }

        public override bool CanSeek =>
            !this.StreamDisposed;

        public override bool CanWrite
        {
            get
            {
                if (this.StreamDisposed)
                {
                    return false;
                }
                if (FileAccess.Write != (this.access & FileAccess.Write))
                {
                    return (FileAccess.ReadWrite == (this.access & FileAccess.ReadWrite));
                }
                return true;
            }
        }

        public override long Length
        {
            get
            {
                STATSTG statstg;
                this.CheckDisposedStatus();
                this._safeIStream.Stat(out statstg, 1);
                return statstg.cbSize;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposedStatus();
                long plibNewPosition = 0L;
                this._safeIStream.Seek(0L, 1, out plibNewPosition);
                return plibNewPosition;
            }
            set
            {
                this.CheckDisposedStatus();
                if (!this.CanSeek)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("SetPositionNotSupported"));
                }
                long plibNewPosition = 0L;
                this._safeIStream.Seek(value, 0, out plibNewPosition);
                if (value != plibNewPosition)
                {
                    throw new IOException(System.Windows.SR.Get("SeekFailed"));
                }
            }
        }

        internal bool StreamDisposed
        {
            get
            {
                if (!this.backReference.StreamInfoDisposed)
                {
                    return (null == this._safeIStream);
                }
                return true;
            }
        }
    }
}

