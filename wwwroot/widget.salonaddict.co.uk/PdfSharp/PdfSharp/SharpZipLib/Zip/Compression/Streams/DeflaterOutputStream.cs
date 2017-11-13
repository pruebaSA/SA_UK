namespace PdfSharp.SharpZipLib.Zip.Compression.Streams
{
    using PdfSharp.SharpZipLib;
    using PdfSharp.SharpZipLib.Checksums;
    using PdfSharp.SharpZipLib.Zip.Compression;
    using System;
    using System.IO;

    internal class DeflaterOutputStream : Stream
    {
        protected Stream baseOutputStream;
        protected byte[] buf;
        protected Deflater def;
        private bool isStreamOwner;
        private uint[] keys;
        private string password;

        public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 0x200)
        {
        }

        public DeflaterOutputStream(Stream baseOutputStream, Deflater defl) : this(baseOutputStream, defl, 0x200)
        {
        }

        public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufsize)
        {
            this.isStreamOwner = true;
            if (!baseOutputStream.CanWrite)
            {
                throw new ArgumentException("baseOutputStream", "must support writing");
            }
            if (deflater == null)
            {
                throw new ArgumentNullException("deflater");
            }
            if (bufsize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufsize");
            }
            this.baseOutputStream = baseOutputStream;
            this.buf = new byte[bufsize];
            this.def = deflater;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("DeflaterOutputStream BeginWrite not currently supported");
        }

        public override void Close()
        {
            this.Finish();
            if (this.isStreamOwner)
            {
                this.baseOutputStream.Close();
            }
        }

        protected void Deflate()
        {
            while (!this.def.IsNeedingInput)
            {
                int length = this.def.Deflate(this.buf, 0, this.buf.Length);
                if (length <= 0)
                {
                    break;
                }
                if (this.Password != null)
                {
                    this.EncryptBlock(this.buf, 0, length);
                }
                this.baseOutputStream.Write(this.buf, 0, length);
            }
            if (!this.def.IsNeedingInput)
            {
                throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
            }
        }

        protected void EncryptBlock(byte[] buffer, int offset, int length)
        {
            for (int i = offset; i < (offset + length); i++)
            {
                byte ch = buffer[i];
                buffer[i] = (byte) (buffer[i] ^ this.EncryptByte());
                this.UpdateKeys(ch);
            }
        }

        protected byte EncryptByte()
        {
            uint num = (this.keys[2] & 0xffff) | 2;
            return (byte) ((num * (num ^ 1)) >> 8);
        }

        public virtual void Finish()
        {
            this.def.Finish();
            while (!this.def.IsFinished)
            {
                int length = this.def.Deflate(this.buf, 0, this.buf.Length);
                if (length <= 0)
                {
                    break;
                }
                if (this.Password != null)
                {
                    this.EncryptBlock(this.buf, 0, length);
                }
                this.baseOutputStream.Write(this.buf, 0, length);
            }
            if (!this.def.IsFinished)
            {
                throw new SharpZipBaseException("Can't deflate all input?");
            }
            this.baseOutputStream.Flush();
        }

        public override void Flush()
        {
            this.def.Flush();
            this.Deflate();
            this.baseOutputStream.Flush();
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
            throw new NotSupportedException("DeflaterOutputStream Read not supported");
        }

        public override int ReadByte()
        {
            throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("DeflaterOutputStream Seek not supported");
        }

        public override void SetLength(long val)
        {
            throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
        }

        protected void UpdateKeys(byte ch)
        {
            this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
            this.keys[1] += (byte) this.keys[0];
            this.keys[1] = (this.keys[1] * 0x8088405) + 1;
            this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte) (this.keys[1] >> 0x18));
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.def.SetInput(buf, off, len);
            this.Deflate();
        }

        public override void WriteByte(byte bval)
        {
            byte[] buffer = new byte[] { bval };
            this.Write(buffer, 0, 1);
        }

        public bool CanPatchEntries =>
            this.baseOutputStream.CanSeek;

        public override bool CanRead =>
            this.baseOutputStream.CanRead;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            this.baseOutputStream.CanWrite;

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
            this.baseOutputStream.Length;

        public string Password
        {
            get => 
                this.password;
            set
            {
                this.password = value;
            }
        }

        public override long Position
        {
            get => 
                this.baseOutputStream.Position;
            set
            {
                throw new NotSupportedException("DefalterOutputStream Position not supported");
            }
        }
    }
}

