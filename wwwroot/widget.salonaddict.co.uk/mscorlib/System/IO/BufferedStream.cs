namespace System.IO
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class BufferedStream : Stream
    {
        private byte[] _buffer;
        private int _bufferSize;
        private const int _DefaultBufferSize = 0x1000;
        private int _readLen;
        private int _readPos;
        private Stream _s;
        private int _writePos;

        private BufferedStream()
        {
        }

        public BufferedStream(Stream stream) : this(stream, 0x1000)
        {
        }

        public BufferedStream(Stream stream, int bufferSize)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_MustBePositive"), new object[] { "bufferSize" }));
            }
            this._s = stream;
            this._bufferSize = bufferSize;
            if (!this._s.CanRead && !this._s.CanWrite)
            {
                __Error.StreamIsClosed();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._s != null))
                {
                    try
                    {
                        this.Flush();
                    }
                    finally
                    {
                        this._s.Close();
                    }
                }
            }
            finally
            {
                this._s = null;
                this._buffer = null;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if (this._writePos > 0)
            {
                this.FlushWrite();
            }
            else if ((this._readPos < this._readLen) && this._s.CanSeek)
            {
                this.FlushRead();
            }
            this._readPos = 0;
            this._readLen = 0;
        }

        private void FlushRead()
        {
            if ((this._readPos - this._readLen) != 0)
            {
                this._s.Seek((long) (this._readPos - this._readLen), SeekOrigin.Current);
            }
            this._readPos = 0;
            this._readLen = 0;
        }

        private void FlushWrite()
        {
            this._s.Write(this._buffer, 0, this._writePos);
            this._writePos = 0;
            this._s.Flush();
        }

        public override int Read([In, Out] byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            int num = this._readLen - this._readPos;
            if (num == 0)
            {
                if (!this._s.CanRead)
                {
                    __Error.ReadNotSupported();
                }
                if (this._writePos > 0)
                {
                    this.FlushWrite();
                }
                if (count >= this._bufferSize)
                {
                    num = this._s.Read(array, offset, count);
                    this._readPos = 0;
                    this._readLen = 0;
                    return num;
                }
                if (this._buffer == null)
                {
                    this._buffer = new byte[this._bufferSize];
                }
                num = this._s.Read(this._buffer, 0, this._bufferSize);
                if (num == 0)
                {
                    return 0;
                }
                this._readPos = 0;
                this._readLen = num;
            }
            if (num > count)
            {
                num = count;
            }
            Buffer.InternalBlockCopy(this._buffer, this._readPos, array, offset, num);
            this._readPos += num;
            if (num < count)
            {
                int num2 = this._s.Read(array, offset + num, count - num);
                num += num2;
                this._readPos = 0;
                this._readLen = 0;
            }
            return num;
        }

        public override int ReadByte()
        {
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if ((this._readLen == 0) && !this._s.CanRead)
            {
                __Error.ReadNotSupported();
            }
            if (this._readPos == this._readLen)
            {
                if (this._writePos > 0)
                {
                    this.FlushWrite();
                }
                if (this._buffer == null)
                {
                    this._buffer = new byte[this._bufferSize];
                }
                this._readLen = this._s.Read(this._buffer, 0, this._bufferSize);
                this._readPos = 0;
            }
            if (this._readPos == this._readLen)
            {
                return -1;
            }
            return this._buffer[this._readPos++];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if (!this._s.CanSeek)
            {
                __Error.SeekNotSupported();
            }
            if (this._writePos > 0)
            {
                this.FlushWrite();
            }
            else if (origin == SeekOrigin.Current)
            {
                offset -= this._readLen - this._readPos;
            }
            long num = this._s.Position + (this._readPos - this._readLen);
            long num2 = this._s.Seek(offset, origin);
            if (this._readLen > 0)
            {
                if (num == num2)
                {
                    if (this._readPos > 0)
                    {
                        Buffer.InternalBlockCopy(this._buffer, this._readPos, this._buffer, 0, this._readLen - this._readPos);
                        this._readLen -= this._readPos;
                        this._readPos = 0;
                    }
                    if (this._readLen > 0)
                    {
                        this._s.Seek((long) this._readLen, SeekOrigin.Current);
                    }
                    return num2;
                }
                if (((num - this._readPos) < num2) && (num2 < ((num + this._readLen) - this._readPos)))
                {
                    int num3 = (int) (num2 - num);
                    Buffer.InternalBlockCopy(this._buffer, this._readPos + num3, this._buffer, 0, this._readLen - (this._readPos + num3));
                    this._readLen -= this._readPos + num3;
                    this._readPos = 0;
                    if (this._readLen > 0)
                    {
                        this._s.Seek((long) this._readLen, SeekOrigin.Current);
                    }
                    return num2;
                }
                this._readPos = 0;
                this._readLen = 0;
            }
            return num2;
        }

        public override void SetLength(long value)
        {
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NegFileSize"));
            }
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if (!this._s.CanSeek)
            {
                __Error.SeekNotSupported();
            }
            if (!this._s.CanWrite)
            {
                __Error.WriteNotSupported();
            }
            if (this._writePos > 0)
            {
                this.FlushWrite();
            }
            else if (this._readPos < this._readLen)
            {
                this.FlushRead();
            }
            this._readPos = 0;
            this._readLen = 0;
            this._s.SetLength(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if (this._writePos == 0)
            {
                if (!this._s.CanWrite)
                {
                    __Error.WriteNotSupported();
                }
                if (this._readPos < this._readLen)
                {
                    this.FlushRead();
                }
                else
                {
                    this._readPos = 0;
                    this._readLen = 0;
                }
            }
            if (this._writePos > 0)
            {
                int num = this._bufferSize - this._writePos;
                if (num > 0)
                {
                    if (num > count)
                    {
                        num = count;
                    }
                    Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, num);
                    this._writePos += num;
                    if (count == num)
                    {
                        return;
                    }
                    offset += num;
                    count -= num;
                }
                this._s.Write(this._buffer, 0, this._writePos);
                this._writePos = 0;
            }
            if (count >= this._bufferSize)
            {
                this._s.Write(array, offset, count);
            }
            else if (count != 0)
            {
                if (this._buffer == null)
                {
                    this._buffer = new byte[this._bufferSize];
                }
                Buffer.InternalBlockCopy(array, offset, this._buffer, 0, count);
                this._writePos = count;
            }
        }

        public override void WriteByte(byte value)
        {
            if (this._s == null)
            {
                __Error.StreamIsClosed();
            }
            if (this._writePos == 0)
            {
                if (!this._s.CanWrite)
                {
                    __Error.WriteNotSupported();
                }
                if (this._readPos < this._readLen)
                {
                    this.FlushRead();
                }
                else
                {
                    this._readPos = 0;
                    this._readLen = 0;
                }
                if (this._buffer == null)
                {
                    this._buffer = new byte[this._bufferSize];
                }
            }
            if (this._writePos == this._bufferSize)
            {
                this.FlushWrite();
            }
            this._buffer[this._writePos++] = value;
        }

        public override bool CanRead =>
            ((this._s != null) && this._s.CanRead);

        public override bool CanSeek =>
            ((this._s != null) && this._s.CanSeek);

        public override bool CanWrite =>
            ((this._s != null) && this._s.CanWrite);

        public override long Length
        {
            get
            {
                if (this._s == null)
                {
                    __Error.StreamIsClosed();
                }
                if (this._writePos > 0)
                {
                    this.FlushWrite();
                }
                return this._s.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (this._s == null)
                {
                    __Error.StreamIsClosed();
                }
                if (!this._s.CanSeek)
                {
                    __Error.SeekNotSupported();
                }
                return (this._s.Position + ((this._readPos - this._readLen) + this._writePos));
            }
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                }
                if (this._s == null)
                {
                    __Error.StreamIsClosed();
                }
                if (!this._s.CanSeek)
                {
                    __Error.SeekNotSupported();
                }
                if (this._writePos > 0)
                {
                    this.FlushWrite();
                }
                this._readPos = 0;
                this._readLen = 0;
                this._s.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

