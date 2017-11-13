namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    internal class BufferedOutputStream : Stream
    {
        private BufferManager bufferManager;
        private int chunkCount;
        private byte[][] chunks;
        private byte[] currentChunk;
        private int currentChunkSize;
        private int maxSize;
        private int maxSizeQuota;
        private string quotaExceededString;
        private int totalSize;

        public BufferedOutputStream(string quotaExceededString)
        {
            this.chunks = new byte[4][];
            this.quotaExceededString = quotaExceededString;
        }

        public BufferedOutputStream(string quotaExceededString, int maxSize) : this(quotaExceededString)
        {
            this.Init(0, maxSize, BufferManager.CreateBufferManager(0L, 0x7fffffff));
        }

        public BufferedOutputStream(string quotaExceededString, int initialSize, int maxSize, BufferManager bufferManager) : this(quotaExceededString)
        {
            this.Init(initialSize, maxSize, bufferManager);
        }

        private void AllocNextChunk(int minimumChunkSize)
        {
            int num;
            if (this.currentChunk.Length > 0x3fffffff)
            {
                num = 0x7fffffff;
            }
            else
            {
                num = this.currentChunk.Length * 2;
            }
            if (minimumChunkSize > num)
            {
                num = minimumChunkSize;
            }
            byte[] buffer = this.bufferManager.TakeBuffer(num);
            if (this.chunkCount == this.chunks.Length)
            {
                byte[][] destinationArray = new byte[this.chunks.Length * 2][];
                Array.Copy(this.chunks, destinationArray, this.chunks.Length);
                this.chunks = destinationArray;
            }
            this.chunks[this.chunkCount++] = buffer;
            this.currentChunk = buffer;
            this.currentChunkSize = 0;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ReadNotSupported")));
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            this.Write(buffer, offset, size);
            return new CompletedAsyncResult(callback, state);
        }

        public void Clear()
        {
            for (int i = 0; i < this.chunkCount; i++)
            {
                this.chunks[i] = null;
            }
            this.chunkCount = 0;
            this.currentChunk = null;
        }

        public override void Close()
        {
        }

        public override int EndRead(IAsyncResult result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ReadNotSupported")));
        }

        public override void EndWrite(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        public override void Flush()
        {
        }

        public void Init(int initialSize, int maxSizeQuota, BufferManager bufferManager)
        {
            this.Init(initialSize, maxSizeQuota, maxSizeQuota, bufferManager);
        }

        public void Init(int initialSize, int maxSizeQuota, int effectiveMaxSize, BufferManager bufferManager)
        {
            this.maxSizeQuota = maxSizeQuota;
            this.maxSize = effectiveMaxSize;
            this.bufferManager = bufferManager;
            this.currentChunk = bufferManager.TakeBuffer(initialSize);
            this.currentChunkSize = 0;
            this.totalSize = 0;
            this.chunkCount = 1;
            this.chunks[0] = this.currentChunk;
        }

        public override int Read(byte[] buffer, int offset, int size)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ReadNotSupported")));
        }

        public override int ReadByte()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ReadNotSupported")));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SeekNotSupported")));
        }

        public override void SetLength(long value)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SeekNotSupported")));
        }

        public void Skip(int size)
        {
            this.WriteCore(null, 0, size);
        }

        public byte[] ToArray(out int bufferSize)
        {
            byte[] currentChunk;
            if (this.chunkCount == 1)
            {
                currentChunk = this.currentChunk;
                bufferSize = this.currentChunkSize;
                return currentChunk;
            }
            currentChunk = this.bufferManager.TakeBuffer(this.totalSize);
            int dstOffset = 0;
            int num2 = this.chunkCount - 1;
            for (int i = 0; i < num2; i++)
            {
                byte[] src = this.chunks[i];
                Buffer.BlockCopy(src, 0, currentChunk, dstOffset, src.Length);
                dstOffset += src.Length;
            }
            Buffer.BlockCopy(this.currentChunk, 0, currentChunk, dstOffset, this.currentChunkSize);
            bufferSize = this.totalSize;
            return currentChunk;
        }

        public MemoryStream ToMemoryStream()
        {
            int num;
            return new MemoryStream(this.ToArray(out num), 0, num);
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            this.WriteCore(buffer, offset, size);
        }

        public override void WriteByte(byte value)
        {
            if (this.totalSize == this.maxSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(System.ServiceModel.SR.GetString(this.quotaExceededString, new object[] { this.maxSize })));
            }
            if (this.currentChunkSize == this.currentChunk.Length)
            {
                this.AllocNextChunk(1);
            }
            this.currentChunk[this.currentChunkSize++] = value;
        }

        private void WriteCore(byte[] buffer, int offset, int size)
        {
            if (size < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
            }
            if ((0x7fffffff - size) < this.totalSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(System.ServiceModel.SR.GetString(this.quotaExceededString, new object[] { this.maxSizeQuota.ToString(NumberFormatInfo.CurrentInfo) })));
            }
            int num = this.totalSize + size;
            if (num > this.maxSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(System.ServiceModel.SR.GetString(this.quotaExceededString, new object[] { this.maxSizeQuota.ToString(NumberFormatInfo.CurrentInfo) })));
            }
            int count = this.currentChunk.Length - this.currentChunkSize;
            if (size > count)
            {
                if (count > 0)
                {
                    if (buffer != null)
                    {
                        Buffer.BlockCopy(buffer, offset, this.currentChunk, this.currentChunkSize, count);
                    }
                    this.currentChunkSize = this.currentChunk.Length;
                    offset += count;
                    size -= count;
                }
                this.AllocNextChunk(size);
            }
            if (buffer != null)
            {
                Buffer.BlockCopy(buffer, offset, this.currentChunk, this.currentChunkSize, size);
            }
            this.totalSize = num;
            this.currentChunkSize += size;
        }

        public override bool CanRead =>
            false;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            true;

        public override long Length =>
            ((long) this.totalSize);

        public override long Position
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SeekNotSupported")));
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SeekNotSupported")));
            }
        }
    }
}

