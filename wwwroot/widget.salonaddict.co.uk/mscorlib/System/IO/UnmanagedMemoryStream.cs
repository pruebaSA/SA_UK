namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [CLSCompliant(false)]
    public class UnmanagedMemoryStream : Stream
    {
        private FileAccess _access;
        private long _capacity;
        internal bool _isOpen;
        private long _length;
        private unsafe byte* _mem;
        private long _position;
        private const long UnmanagedMemStreamMaxLength = 0x7fffffffffffffffL;

        protected unsafe UnmanagedMemoryStream()
        {
            this._mem = null;
            this._isOpen = false;
        }

        public unsafe UnmanagedMemoryStream(byte* pointer, long length)
        {
            this.Initialize(pointer, length, length, FileAccess.Read, false);
        }

        public unsafe UnmanagedMemoryStream(byte* pointer, long length, long capacity, FileAccess access)
        {
            this.Initialize(pointer, length, capacity, access, false);
        }

        internal unsafe UnmanagedMemoryStream(byte* pointer, long length, long capacity, FileAccess access, bool skipSecurityCheck)
        {
            this.Initialize(pointer, length, capacity, access, skipSecurityCheck);
        }

        protected override void Dispose(bool disposing)
        {
            this._isOpen = false;
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
        }

        protected unsafe void Initialize(byte* pointer, long length, long capacity, FileAccess access)
        {
            this.Initialize(pointer, length, capacity, access, false);
        }

        internal unsafe void Initialize(byte* pointer, long length, long capacity, FileAccess access, bool skipSecurityCheck)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }
            if ((length < 0L) || (capacity < 0L))
            {
                throw new ArgumentOutOfRangeException((length < 0L) ? "length" : "capacity", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (length > capacity)
            {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_LengthGreaterThanCapacity"));
            }
            if (((IntPtr) (((ulong) pointer) + capacity)) < pointer)
            {
                throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_UnmanagedMemStreamWrapAround"));
            }
            if ((access < FileAccess.Read) || (access > FileAccess.ReadWrite))
            {
                throw new ArgumentOutOfRangeException("access", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
            }
            if (this._isOpen)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CalledTwice"));
            }
            if (!skipSecurityCheck)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            this._mem = pointer;
            this._length = length;
            this._capacity = capacity;
            this._access = access;
            this._isOpen = true;
        }

        public override unsafe int Read([In, Out] byte[] buffer, int offset, int count)
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if ((this._access & FileAccess.Read) == 0)
            {
                __Error.ReadNotSupported();
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            long num = this._position;
            long num2 = this._length - num;
            if (num2 > count)
            {
                num2 = count;
            }
            if (num2 <= 0L)
            {
                return 0;
            }
            int len = (int) num2;
            if (len < 0)
            {
                len = 0;
            }
            Buffer.memcpy(this._mem + ((byte*) num), 0, buffer, offset, len);
            this._position = num + num2;
            return len;
        }

        public override unsafe int ReadByte()
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if ((this._access & FileAccess.Read) == 0)
            {
                __Error.ReadNotSupported();
            }
            long num = this._position;
            if (num >= this._length)
            {
                return -1;
            }
            this._position = num + 1L;
            return *(((int*) (this._mem + num)));
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if (offset > 0x7fffffffffffffffL)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_UnmanagedMemStreamLength"));
            }
            switch (loc)
            {
                case SeekOrigin.Begin:
                    if (offset < 0L)
                    {
                        throw new IOException(Environment.GetResourceString("IO.IO_SeekBeforeBegin"));
                    }
                    this._position = offset;
                    break;

                case SeekOrigin.Current:
                    if ((offset + this._position) < 0L)
                    {
                        throw new IOException(Environment.GetResourceString("IO.IO_SeekBeforeBegin"));
                    }
                    this._position += offset;
                    break;

                case SeekOrigin.End:
                    if ((this._length + offset) < 0L)
                    {
                        throw new IOException(Environment.GetResourceString("IO.IO_SeekBeforeBegin"));
                    }
                    this._position = this._length + offset;
                    break;

                default:
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSeekOrigin"));
            }
            return this._position;
        }

        public override unsafe void SetLength(long value)
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if ((this._access & FileAccess.Write) == 0)
            {
                __Error.WriteNotSupported();
            }
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (value > this._capacity)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_FixedCapacity"));
            }
            long num = this._length;
            if (value > num)
            {
                Buffer.ZeroMemory(this._mem + ((byte*) num), value - num);
            }
            this._length = value;
            if (this._position > value)
            {
                this._position = value;
            }
        }

        public override unsafe void Write(byte[] buffer, int offset, int count)
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if ((this._access & FileAccess.Write) == 0)
            {
                __Error.WriteNotSupported();
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            long num = this._position;
            long num2 = this._length;
            long num3 = num + count;
            if (num3 < 0L)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_StreamTooLong"));
            }
            if (num3 > num2)
            {
                if (num3 > this._capacity)
                {
                    throw new NotSupportedException(Environment.GetResourceString("IO.IO_FixedCapacity"));
                }
                this._length = num3;
            }
            if (num > num2)
            {
                Buffer.ZeroMemory(this._mem + ((byte*) num2), num - num2);
            }
            Buffer.memcpy(buffer, offset, this._mem + ((byte*) num), 0, count);
            this._position = num3;
        }

        public override unsafe void WriteByte(byte value)
        {
            if (!this._isOpen)
            {
                __Error.StreamIsClosed();
            }
            if ((this._access & FileAccess.Write) == 0)
            {
                __Error.WriteNotSupported();
            }
            long num = this._position;
            long num2 = this._length;
            long num3 = num + 1L;
            if (num >= num2)
            {
                if (num3 < 0L)
                {
                    throw new IOException(Environment.GetResourceString("IO.IO_StreamTooLong"));
                }
                if (num3 > this._capacity)
                {
                    throw new NotSupportedException(Environment.GetResourceString("IO.IO_FixedCapacity"));
                }
                this._length = num3;
                if (num > num2)
                {
                    Buffer.ZeroMemory(this._mem + ((byte*) num2), num - num2);
                }
            }
            this._mem[(int) num] = value;
            this._position = num3;
        }

        public override bool CanRead =>
            (this._isOpen && ((this._access & FileAccess.Read) != 0));

        public override bool CanSeek =>
            this._isOpen;

        public override bool CanWrite =>
            (this._isOpen && ((this._access & FileAccess.Write) != 0));

        public long Capacity
        {
            get
            {
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                return this._capacity;
            }
        }

        public override long Length
        {
            get
            {
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                return this._length;
            }
        }

        internal byte* Pointer =>
            this._mem;

        public override long Position
        {
            get
            {
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                return this._position;
            }
            set
            {
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                }
                if ((value > 0x7fffffffL) || ((this._mem + value) < this._mem))
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_MemStreamLength"));
                }
                this._position = value;
            }
        }

        public byte* PositionPointer
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                long num = this._position;
                if (num > this._capacity)
                {
                    throw new IndexOutOfRangeException(Environment.GetResourceString("IndexOutOfRange_UMSPosition"));
                }
                byte* numPtr = this._mem + ((byte*) num);
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                return numPtr;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                if (!this._isOpen)
                {
                    __Error.StreamIsClosed();
                }
                IntPtr ptr = new IntPtr((long) ((value - this._mem) / 1));
                if (ptr.ToInt64() > 0x7fffffffffffffffL)
                {
                    throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_UnmanagedMemStreamLength"));
                }
                if (value < this._mem)
                {
                    throw new IOException(Environment.GetResourceString("IO.IO_SeekBeforeBegin"));
                }
                this._position = (long) ((value - this._mem) / 1);
            }
        }
    }
}

