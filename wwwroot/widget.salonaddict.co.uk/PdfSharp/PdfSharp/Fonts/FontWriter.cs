namespace PdfSharp.Fonts
{
    using System;
    using System.IO;

    internal class FontWriter
    {
        private readonly System.IO.Stream stream;

        public FontWriter(System.IO.Stream stream)
        {
            this.stream = stream;
        }

        public void Close()
        {
            this.Close(true);
        }

        public void Close(bool closeUnderlyingStream)
        {
            if ((this.stream != null) && closeUnderlyingStream)
            {
                this.stream.Close();
            }
        }

        public void Write(byte[] buffer)
        {
            this.stream.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        public void WriteByte(int value)
        {
            this.stream.WriteByte((byte) value);
        }

        public void WriteInt(int value)
        {
            this.stream.WriteByte((byte) (value >> 0x18));
            this.stream.WriteByte((byte) (value >> 0x10));
            this.stream.WriteByte((byte) (value >> 8));
            this.stream.WriteByte((byte) value);
        }

        public void WriteShort(short value)
        {
            this.stream.WriteByte((byte) (value >> 8));
            this.stream.WriteByte((byte) value);
        }

        public void WriteShort(int value)
        {
            this.WriteShort((short) value);
        }

        public void WriteUInt(uint value)
        {
            this.stream.WriteByte((byte) (value >> 0x18));
            this.stream.WriteByte((byte) (value >> 0x10));
            this.stream.WriteByte((byte) (value >> 8));
            this.stream.WriteByte((byte) value);
        }

        public void WriteUShort(int value)
        {
            this.WriteUShort((ushort) value);
        }

        public void WriteUShort(ushort value)
        {
            this.stream.WriteByte((byte) (value >> 8));
            this.stream.WriteByte((byte) value);
        }

        public int Position
        {
            get => 
                ((int) this.stream.Position);
            set
            {
                this.stream.Position = value;
            }
        }

        internal System.IO.Stream Stream =>
            this.stream;
    }
}

