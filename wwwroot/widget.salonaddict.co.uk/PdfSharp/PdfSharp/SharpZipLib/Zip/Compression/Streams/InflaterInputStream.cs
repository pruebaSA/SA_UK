namespace PdfSharp.SharpZipLib.Zip.Compression.Streams
{
    using PdfSharp.SharpZipLib;
    using PdfSharp.SharpZipLib.Checksums;
    using PdfSharp.SharpZipLib.Zip.Compression;
    using System;
    using System.IO;

    internal class InflaterInputStream : Stream
    {
        protected Stream baseInputStream;
        protected byte[] buf;
        protected byte[] cryptbuffer;
        protected Inflater inf;
        private bool isStreamOwner;
        private uint[] keys;
        protected int len;
        private byte[] onebytebuffer;
        private int readChunkSize;

        public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 0x1000)
        {
        }

        public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 0x1000)
        {
        }

        public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
        {
            this.onebytebuffer = new byte[1];
            this.isStreamOwner = true;
            if (baseInputStream == null)
            {
                throw new ArgumentNullException("InflaterInputStream baseInputStream is null");
            }
            if (inflater == null)
            {
                throw new ArgumentNullException("InflaterInputStream Inflater is null");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }
            this.baseInputStream = baseInputStream;
            this.inf = inflater;
            this.buf = new byte[bufferSize];
            if (baseInputStream.CanSeek)
            {
                this.len = (int) baseInputStream.Length;
            }
            else
            {
                this.len = 0;
            }
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
        }

        public override void Close()
        {
            if (this.isStreamOwner)
            {
                this.baseInputStream.Close();
            }
        }

        protected void DecryptBlock(byte[] buf, int off, int len)
        {
            for (int i = off; i < (off + len); i++)
            {
                buf[i] = (byte) (buf[i] ^ this.DecryptByte());
                this.UpdateKeys(buf[i]);
            }
        }

        protected byte DecryptByte()
        {
            uint num = (this.keys[2] & 0xffff) | 2;
            return (byte) ((num * (num ^ 1)) >> 8);
        }

        protected void Fill()
        {
            this.FillInputBuffer();
            if (this.keys != null)
            {
                this.DecryptBlock(this.buf, 0, this.len);
            }
            if (this.len <= 0)
            {
                throw new SharpZipBaseException("Deflated stream ends early.");
            }
            this.inf.SetInput(this.buf, 0, this.len);
        }

        protected void FillInputBuffer()
        {
            if (this.readChunkSize <= 0)
            {
                this.len = this.baseInputStream.Read(this.buf, 0, this.buf.Length);
            }
            else
            {
                this.len = this.baseInputStream.Read(this.buf, 0, this.readChunkSize);
            }
        }

        public override void Flush()
        {
            this.baseInputStream.Flush();
        }

        protected void InitializePassword(string password)
        {
            this.keys = new uint[] { 0x12345678, 0x23456789, 0x34567890 };
            for (int i = 0; i < password.Length; i++)
            {
                this.UpdateKeys((byte) password[i]);
            }
        }

        public override int Read(byte[] b, int off, int len)
        {
            while (true)
            {
                int num;
                try
                {
                    num = this.inf.Inflate(b, off, len);
                }
                catch (Exception exception)
                {
                    throw new SharpZipBaseException(exception.ToString());
                }
                if (num > 0)
                {
                    return num;
                }
                if (this.inf.IsNeedingDictionary)
                {
                    throw new SharpZipBaseException("Need a dictionary");
                }
                if (this.inf.IsFinished)
                {
                    return 0;
                }
                if (!this.inf.IsNeedingInput)
                {
                    throw new InvalidOperationException("Don't know what to do");
                }
                this.Fill();
            }
        }

        public override int ReadByte()
        {
            if (this.Read(this.onebytebuffer, 0, 1) > 0)
            {
                return (this.onebytebuffer[0] & 0xff);
            }
            return -1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek not supported");
        }

        public override void SetLength(long val)
        {
            throw new NotSupportedException("InflaterInputStream SetLength not supported");
        }

        public long Skip(long n)
        {
            if (n <= 0L)
            {
                throw new ArgumentOutOfRangeException("n");
            }
            if (this.baseInputStream.CanSeek)
            {
                this.baseInputStream.Seek(n, SeekOrigin.Current);
                return n;
            }
            int num = 0x800;
            if (n < num)
            {
                num = (int) n;
            }
            byte[] buffer = new byte[num];
            return (long) this.baseInputStream.Read(buffer, 0, buffer.Length);
        }

        protected void StopDecrypting()
        {
            this.keys = null;
            this.cryptbuffer = null;
        }

        protected void UpdateKeys(byte ch)
        {
            this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
            this.keys[1] += (byte) this.keys[0];
            this.keys[1] = (this.keys[1] * 0x8088405) + 1;
            this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte) (this.keys[1] >> 0x18));
        }

        public override void Write(byte[] array, int offset, int count)
        {
            throw new NotSupportedException("InflaterInputStream Write not supported");
        }

        public override void WriteByte(byte val)
        {
            throw new NotSupportedException("InflaterInputStream WriteByte not supported");
        }

        public virtual int Available
        {
            get
            {
                if (!this.inf.IsFinished)
                {
                    return 1;
                }
                return 0;
            }
        }

        protected int BufferReadSize
        {
            get => 
                this.readChunkSize;
            set
            {
                this.readChunkSize = value;
            }
        }

        public override bool CanRead =>
            this.baseInputStream.CanRead;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            false;

        public bool IsStreamOwner
        {
            get => 
                this.isStreamOwner;
            set
            {
                this.isStreamOwner = value;
            }
        }

        public override long Length =>
            ((long) this.len);

        public override long Position
        {
            get => 
                this.baseInputStream.Position;
            set
            {
                throw new NotSupportedException("InflaterInputStream Position not supported");
            }
        }
    }
}

