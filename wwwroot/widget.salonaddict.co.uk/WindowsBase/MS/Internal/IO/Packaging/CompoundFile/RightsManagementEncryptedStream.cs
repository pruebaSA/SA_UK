namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.RightsManagement;
    using System.Windows;

    internal class RightsManagementEncryptedStream : Stream
    {
        private const long _autoFlushHighWaterMark = 0x4000L;
        private Stream _baseStream;
        private MemoryStreamBlock _comparisonBlock;
        private CryptoProvider _cryptoProvider;
        private const int _prefixLengthSize = 8;
        private Random _random;
        private byte[] _randomBuffer;
        private SparseMemoryStream _readCache = new SparseMemoryStream(0x7fffffffL, 0x7fffffffffffffffL, false);
        private long _streamCachedLength = -1L;
        private long _streamOnDiskLength = -1L;
        private long _streamPosition;
        private SparseMemoryStream _writeCache = new SparseMemoryStream(0x7fffffffL, 0x7fffffffffffffffL, false);

        internal RightsManagementEncryptedStream(Stream baseStream, CryptoProvider cryptoProvider)
        {
            if (!cryptoProvider.CanDecrypt)
            {
                throw new ArgumentException(System.Windows.SR.Get("CryptoProviderCanNotDecrypt"), "cryptoProvider");
            }
            if (!cryptoProvider.CanMergeBlocks)
            {
                throw new ArgumentException(System.Windows.SR.Get("CryptoProviderCanNotMergeBlocks"), "cryptoProvider");
            }
            this._baseStream = baseStream;
            this._cryptoProvider = cryptoProvider;
            this.ParseStreamLength();
        }

        private static void CalcBlockData(long start, long size, bool canMergeBlocks, ref int blockSize, out long firstBlockOffset, out long blockCount)
        {
            long blockNo = GetBlockNo((long) blockSize, start);
            firstBlockOffset = blockNo * ((long) blockSize);
            blockCount = GetBlockSpanCount((long) blockSize, start, size);
            if (canMergeBlocks)
            {
                blockSize = (int) (((long) blockSize) * blockCount);
                blockCount = 1L;
            }
        }

        private void CheckDisposed()
        {
            if (this._baseStream == null)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._baseStream != null))
                {
                    this.FlushCache();
                    this._baseStream.Close();
                    this._readCache.Close();
                    this._writeCache.Close();
                }
            }
            finally
            {
                this._baseStream = null;
                this._readCache = null;
                this._writeCache = null;
                base.Dispose(disposing);
            }
        }

        private void FetchBlockIntoReadCache(long start, int count)
        {
            long num;
            long num2;
            int blockSize = this._cryptoProvider.BlockSize;
            CalcBlockData(start, (long) count, this._cryptoProvider.CanMergeBlocks, ref blockSize, out num, out num2);
            this._baseStream.Seek(8L + num, SeekOrigin.Begin);
            int requestedCount = (int) (num2 * blockSize);
            byte[] buffer = new byte[requestedCount];
            int num5 = PackagingUtilities.ReliableRead(this._baseStream, buffer, 0, requestedCount, this._cryptoProvider.BlockSize);
            if (num5 < this._cryptoProvider.BlockSize)
            {
                throw new FileFormatException(System.Windows.SR.Get("EncryptedDataStreamCorrupt"));
            }
            int num6 = this._cryptoProvider.BlockSize;
            int num7 = num5 / num6;
            if (this._cryptoProvider.CanMergeBlocks)
            {
                num6 *= num7;
                num7 = 1;
            }
            byte[] destinationArray = new byte[num6];
            this._readCache.Seek(num, SeekOrigin.Begin);
            for (long i = 0L; i < num7; i += 1L)
            {
                Array.Copy(buffer, i * num6, destinationArray, 0L, (long) num6);
                byte[] buffer3 = this._cryptoProvider.Decrypt(destinationArray);
                this._readCache.Write(buffer3, 0, num6);
            }
        }

        private int FindIndexOfBlockAtOffset(SparseMemoryStream cache, long start, out bool match)
        {
            if (cache.MemoryBlockCollection.Count == 0)
            {
                match = false;
                return 0;
            }
            if (this._comparisonBlock == null)
            {
                this._comparisonBlock = new MemoryStreamBlock(null, start);
            }
            else
            {
                this._comparisonBlock.Offset = start;
            }
            int num = cache.MemoryBlockCollection.BinarySearch(this._comparisonBlock);
            if (num < 0)
            {
                num = ~num;
                match = false;
                return num;
            }
            match = true;
            return num;
        }

        private long FindOffsetOfNextAvailableBlockAfter(SparseMemoryStream cache, long start)
        {
            bool flag;
            int num = this.FindIndexOfBlockAtOffset(cache, start, out flag);
            if (num >= cache.MemoryBlockCollection.Count)
            {
                return -1L;
            }
            return cache.MemoryBlockCollection[num].Offset;
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this.FlushCache();
            this._baseStream.Flush();
        }

        private void FlushCache()
        {
            this.FlushLength();
            long num = 0L;
            byte[] buffer = null;
            foreach (MemoryStreamBlock block in this._writeCache.MemoryBlockCollection)
            {
                long offset = block.Offset;
                long length = block.Stream.Length;
                if (offset < num)
                {
                    length = (offset + length) - num;
                    offset = num;
                }
                if (length > 0L)
                {
                    long num4;
                    long num5;
                    int blockSize = this._cryptoProvider.BlockSize;
                    CalcBlockData(offset, length, this._cryptoProvider.CanMergeBlocks, ref blockSize, out num4, out num5);
                    int num7 = (int) (num5 * blockSize);
                    if ((buffer == null) || (buffer.Length < num7))
                    {
                        buffer = new byte[Math.Max(0x1000, num7)];
                    }
                    int num8 = this.InternalReliableRead(num4, buffer, 0, num7);
                    if (num8 < num7)
                    {
                        this.RandomFillUp(buffer, num8, num7 - num8);
                    }
                    byte[] buffer2 = this._cryptoProvider.Encrypt(buffer);
                    this._baseStream.Seek(num4 + 8L, SeekOrigin.Begin);
                    this._baseStream.Write(buffer2, 0, num7);
                    num = num4 + num7;
                }
            }
            this._writeCache.SetLength(0L);
            this._readCache.SetLength(0L);
        }

        private void FlushCacheIfNecessary()
        {
            if ((this._readCache.MemoryConsumption + this._writeCache.MemoryConsumption) > 0x4000L)
            {
                this.FlushCache();
            }
        }

        private void FlushLength()
        {
            if ((this._streamCachedLength >= 0L) && (this._streamCachedLength != this._streamOnDiskLength))
            {
                this._baseStream.Seek(0L, SeekOrigin.Begin);
                byte[] bytes = BitConverter.GetBytes((ulong) this._streamCachedLength);
                this._baseStream.Write(bytes, 0, bytes.Length);
                int blockSize = this._cryptoProvider.BlockSize;
                long num2 = 8L + (GetBlockSpanCount((long) blockSize, 0L, this._streamCachedLength) * blockSize);
                this._baseStream.SetLength(num2);
                this._streamOnDiskLength = this._streamCachedLength;
            }
        }

        private static long GetBlockNo(long blockSize, long index) => 
            (index / blockSize);

        private static long GetBlockSpanCount(long blockSize, long index, long size)
        {
            if (size == 0L)
            {
                return 0L;
            }
            return ((GetBlockNo(blockSize, (index + size) - 1L) - GetBlockNo(blockSize, index)) + 1L);
        }

        private int InternalRead(long streamPosition, byte[] buffer, int offset, int count)
        {
            long start = streamPosition;
            int num2 = count;
            if ((start + count) > this.Length)
            {
                num2 = (int) (this.Length - start);
            }
            if (num2 <= 0)
            {
                return 0;
            }
            int num3 = this.ReadFromCache(this._writeCache, start, num2, buffer, offset);
            if (num3 > 0)
            {
                return num3;
            }
            long num4 = this.FindOffsetOfNextAvailableBlockAfter(this._writeCache, start);
            if ((num4 >= 0L) && ((start + num2) > num4))
            {
                num2 = (int) (num4 - start);
            }
            num3 = this.ReadFromCache(this._readCache, start, num2, buffer, offset);
            if (num3 > 0)
            {
                return num3;
            }
            long num5 = this.FindOffsetOfNextAvailableBlockAfter(this._readCache, start);
            if ((num5 >= 0L) && ((start + num2) > num5))
            {
                num2 = (int) (num5 - start);
            }
            this.FetchBlockIntoReadCache(start, num2);
            return this.ReadFromCache(this._readCache, start, num2, buffer, offset);
        }

        private int InternalReliableRead(long streamPosition, byte[] buffer, int offset, int count)
        {
            int num = 0;
            while (num < count)
            {
                int num2 = this.InternalRead(streamPosition + num, buffer, offset + num, count - num);
                if (num2 == 0)
                {
                    return num;
                }
                num += num2;
            }
            return num;
        }

        private void ParseStreamLength()
        {
            if (this._streamCachedLength < 0L)
            {
                this._baseStream.Seek(0L, SeekOrigin.Begin);
                byte[] buffer = new byte[8];
                int num = PackagingUtilities.ReliableRead(this._baseStream, buffer, 0, buffer.Length);
                if (num == 0)
                {
                    this._streamOnDiskLength = 0L;
                }
                else
                {
                    if (num < 8)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("EncryptedDataStreamCorrupt"));
                    }
                    this._streamOnDiskLength = (long) BitConverter.ToUInt64(buffer, 0);
                }
                this._streamCachedLength = this._streamOnDiskLength;
            }
        }

        private void RandomFillUp(byte[] buffer, int offset, int count)
        {
            if (count != 0)
            {
                if (this._random == null)
                {
                    this._random = new Random();
                }
                if ((this._randomBuffer == null) || (this._randomBuffer.Length < count))
                {
                    this._randomBuffer = new byte[Math.Max(0x10, count)];
                }
                this._random.NextBytes(this._randomBuffer);
                Array.Copy(this._randomBuffer, 0, buffer, offset, count);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            int num = this.InternalRead(this._streamPosition, buffer, offset, count);
            this.FlushCacheIfNecessary();
            this._streamPosition += num;
            return num;
        }

        private int ReadFromCache(SparseMemoryStream cache, long start, int count, byte[] buffer, int bufferOffset)
        {
            bool flag;
            IList<MemoryStreamBlock> memoryBlockCollection = cache.MemoryBlockCollection;
            int num = this.FindIndexOfBlockAtOffset(cache, start, out flag);
            int num2 = 0;
            if (flag)
            {
                long num3;
                long num4;
                MemoryStreamBlock block = memoryBlockCollection[num];
                PackagingUtilities.CalculateOverlap(block.Offset, block.Stream.Length, start, (long) count, out num3, out num4);
                if (num4 > 0L)
                {
                    block.Stream.Seek(num3 - block.Offset, SeekOrigin.Begin);
                    num2 = block.Stream.Read(buffer, bufferOffset, (int) num4);
                }
            }
            return num2;
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
                    num = this._streamPosition + offset;
                    break;

                case SeekOrigin.End:
                    num = this.Length + offset;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("origin", System.Windows.SR.Get("SeekOriginInvalid"));
            }
            if (num < 0L)
            {
                throw new ArgumentOutOfRangeException("offset", System.Windows.SR.Get("SeekNegative"));
            }
            this._streamPosition = num;
            return this._streamPosition;
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength", System.Windows.SR.Get("CannotMakeStreamLengthNegative"));
            }
            this._streamCachedLength = newLength;
            this.FlushLength();
            if (this._streamPosition > this.Length)
            {
                this._streamPosition = this.Length;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            this._writeCache.Seek(this.Position, SeekOrigin.Begin);
            this._writeCache.Write(buffer, offset, count);
            if (this._writeCache.Length > this.Length)
            {
                this.SetLength(this._writeCache.Length);
            }
            this._streamPosition += count;
            this.FlushCacheIfNecessary();
        }

        public override bool CanRead =>
            ((((this._baseStream != null) && this._baseStream.CanRead) && this._baseStream.CanSeek) && this._cryptoProvider.CanDecrypt);

        public override bool CanSeek =>
            ((this._baseStream != null) && this._baseStream.CanSeek);

        public override bool CanWrite =>
            (((((this._baseStream != null) && this._baseStream.CanWrite) && (this._baseStream.CanRead && this._baseStream.CanSeek)) && this._cryptoProvider.CanDecrypt) && this._cryptoProvider.CanEncrypt);

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._streamCachedLength;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._streamPosition;
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

