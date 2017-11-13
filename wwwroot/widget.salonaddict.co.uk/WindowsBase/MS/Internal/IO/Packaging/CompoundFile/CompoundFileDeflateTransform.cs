namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows;

    internal class CompoundFileDeflateTransform : IDeflateTransform
    {
        private const int _blockHeaderSize = 12;
        private const uint _blockHeaderToken = 0xfa0;
        private const int _compressionLevel = 9;
        private const int _defaultBlockSize = 0x1000;
        private byte[] _headerBuf = new byte[12];
        private const int _maxAllowableBlockSize = 0xfffff;
        private const int _ulongSize = 4;

        [SecurityCritical]
        private static void AllocOrRealloc(int size, ref byte[] buffer, ref GCHandle gcHandle)
        {
            Invariant.Assert(size >= 0, "Cannot allocate negative number of bytes");
            if (buffer != null)
            {
                if (buffer.Length >= size)
                {
                    return;
                }
                size = Math.Max(size, buffer.Length + (buffer.Length >> 1));
                if (gcHandle.IsAllocated)
                {
                    gcHandle.Free();
                }
            }
            buffer = new byte[size];
            gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public void Compress(Stream source, Stream sink)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (sink == null)
            {
                throw new ArgumentNullException("sink");
            }
            Invariant.Assert(source.CanRead);
            Invariant.Assert(sink.CanWrite, "Logic Error - Cannot compress into a read-only stream");
            long position = -1L;
            try
            {
                int num2;
                if (source.CanSeek)
                {
                    position = source.Position;
                    source.Position = 0L;
                    num2 = (int) Math.Min(source.Length, 0x1000L);
                }
                else
                {
                    num2 = 0x1000;
                }
                if (sink.CanSeek)
                {
                    sink.Position = 0L;
                }
                UnsafeNativeMethods.ZStream stream = new UnsafeNativeMethods.ZStream();
                ThrowIfZLibError(UnsafeNativeMethods.ZLib.ums_deflate_init(ref stream, 9, "1.1.4", Marshal.SizeOf(stream)));
                long num3 = 0L;
                byte[] buffer = null;
                byte[] buffer2 = null;
                GCHandle gcHandle = new GCHandle();
                GCHandle handle2 = new GCHandle();
                try
                {
                    int num4;
                    AllocOrRealloc(num2, ref buffer, ref gcHandle);
                    AllocOrRealloc(0x1800, ref buffer2, ref handle2);
                    BinaryWriter writer = new BinaryWriter(sink);
                    while ((num4 = PackagingUtilities.ReliableRead(source, buffer, 0, buffer.Length)) > 0)
                    {
                        Invariant.Assert(num4 <= num2);
                        stream.pInBuf = gcHandle.AddrOfPinnedObject();
                        stream.pOutBuf = handle2.AddrOfPinnedObject();
                        stream.cbIn = (uint) num4;
                        stream.cbOut = (uint) buffer2.Length;
                        ThrowIfZLibError(UnsafeNativeMethods.ZLib.ums_deflate(ref stream, 2));
                        int count = buffer2.Length - ((int) stream.cbOut);
                        Invariant.Assert(count > 0, "compressing non-zero bytes creates a non-empty block");
                        Invariant.Assert(stream.cbIn == 0, "Expecting all data to be compressed!");
                        writer.Write((uint) 0xfa0);
                        writer.Write((uint) num4);
                        writer.Write((uint) count);
                        num3 += this._headerBuf.Length;
                        sink.Write(buffer2, 0, count);
                        num3 += count;
                    }
                    if (sink.CanSeek)
                    {
                        sink.SetLength(num3);
                    }
                }
                finally
                {
                    if (gcHandle.IsAllocated)
                    {
                        gcHandle.Free();
                    }
                    if (handle2.IsAllocated)
                    {
                        handle2.Free();
                    }
                }
            }
            finally
            {
                if (sink.CanSeek)
                {
                    source.Position = position;
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public void Decompress(Stream source, Stream sink)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (sink == null)
            {
                throw new ArgumentNullException("sink");
            }
            Invariant.Assert(source.CanRead);
            Invariant.Assert(sink.CanWrite, "Logic Error - Cannot decompress into a read-only stream");
            long position = -1L;
            try
            {
                if (source.CanSeek)
                {
                    position = source.Position;
                    source.Position = 0L;
                }
                if (sink.CanSeek)
                {
                    sink.Position = 0L;
                }
                UnsafeNativeMethods.ZStream stream = new UnsafeNativeMethods.ZStream();
                ThrowIfZLibError(UnsafeNativeMethods.ZLib.ums_inflate_init(ref stream, "1.1.4", Marshal.SizeOf(stream)));
                byte[] buffer = null;
                byte[] buffer2 = null;
                GCHandle gcHandle = new GCHandle();
                GCHandle handle2 = new GCHandle();
                try
                {
                    int num2;
                    int num3;
                    long num4 = 0L;
                    while (this.ReadBlockHeader(source, out num2, out num3))
                    {
                        AllocOrRealloc(num3, ref buffer, ref gcHandle);
                        AllocOrRealloc(num2, ref buffer2, ref handle2);
                        int num5 = PackagingUtilities.ReliableRead(source, buffer, 0, num3);
                        if (num5 > 0)
                        {
                            if (num3 != num5)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                            }
                            stream.pInBuf = gcHandle.AddrOfPinnedObject();
                            stream.pOutBuf = handle2.AddrOfPinnedObject();
                            stream.cbIn = (uint) num5;
                            stream.cbOut = (uint) buffer2.Length;
                            ThrowIfZLibError(UnsafeNativeMethods.ZLib.ums_inflate(ref stream, 2));
                            int count = buffer2.Length - ((int) stream.cbOut);
                            if (count != num2)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                            }
                            num4 += count;
                            sink.Write(buffer2, 0, count);
                        }
                        else if (num3 != 0)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                        }
                    }
                    if (sink.CanSeek)
                    {
                        sink.SetLength(num4);
                    }
                }
                finally
                {
                    if (gcHandle.IsAllocated)
                    {
                        gcHandle.Free();
                    }
                    if (handle2.IsAllocated)
                    {
                        handle2.Free();
                    }
                }
            }
            finally
            {
                if (source.CanSeek)
                {
                    source.Position = position;
                }
            }
        }

        private bool ReadBlockHeader(Stream source, out int uncompressedSize, out int compressedSize)
        {
            int num = PackagingUtilities.ReliableRead(source, this._headerBuf, 0, this._headerBuf.Length);
            if (num > 0)
            {
                if (num < this._headerBuf.Length)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                }
                if (BitConverter.ToUInt32(this._headerBuf, 0) != 0xfa0)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                }
                uncompressedSize = (int) BitConverter.ToUInt32(this._headerBuf, 4);
                compressedSize = (int) BitConverter.ToUInt32(this._headerBuf, 8);
                if (((uncompressedSize < 0) || (uncompressedSize > 0xfffff)) || ((compressedSize < 0) || (compressedSize > 0xfffff)))
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
                }
            }
            else
            {
                uncompressedSize = compressedSize = 0;
            }
            return (num > 0);
        }

        private static void ThrowIfZLibError(UnsafeNativeMethods.ZLib.ErrorCode retVal)
        {
            bool flag = false;
            bool flag2 = false;
            switch (retVal)
            {
                case UnsafeNativeMethods.ZLib.ErrorCode.VersionError:
                    throw new InvalidOperationException(System.Windows.SR.Get("ZLibVersionError", new object[] { "1.1.4" }));

                case UnsafeNativeMethods.ZLib.ErrorCode.BufError:
                    flag = true;
                    break;

                case UnsafeNativeMethods.ZLib.ErrorCode.MemError:
                    throw new OutOfMemoryException();

                case UnsafeNativeMethods.ZLib.ErrorCode.DataError:
                    flag2 = true;
                    break;

                case UnsafeNativeMethods.ZLib.ErrorCode.StreamError:
                    flag2 = true;
                    break;

                case UnsafeNativeMethods.ZLib.ErrorCode.Success:
                    return;

                case UnsafeNativeMethods.ZLib.ErrorCode.StreamEnd:
                    flag = true;
                    break;

                case UnsafeNativeMethods.ZLib.ErrorCode.NeedDictionary:
                    flag2 = true;
                    break;

                default:
                    throw new IOException();
            }
            if (flag)
            {
                throw new InvalidOperationException();
            }
            if (flag2)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptStream"));
            }
        }
    }
}

