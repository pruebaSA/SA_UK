namespace PdfSharp.Pdf.Advanced
{
    using System;

    internal class BitWriter
    {
        private uint bitsInBuffer;
        private uint buffer;
        private int bytesOffsetWrite;
        private readonly byte[] imageData;
        private static readonly uint[] masks = new uint[] { 0, 1, 3, 7, 15, 0x1f, 0x3f, 0x7f, 0xff };

        internal BitWriter(ref byte[] imageData)
        {
            this.imageData = imageData;
        }

        internal int BytesWritten()
        {
            this.FlushBuffer();
            return this.bytesOffsetWrite;
        }

        internal void FlushBuffer()
        {
            if (this.bitsInBuffer > 0)
            {
                uint bits = 8 - this.bitsInBuffer;
                this.WriteBits(0, bits);
            }
        }

        internal void WriteBits(uint value, uint bits)
        {
            while (true)
            {
                if ((bits + this.bitsInBuffer) <= 8)
                {
                    break;
                }
                uint num = 8 - this.bitsInBuffer;
                uint num2 = bits - num;
                this.WriteBits(value >> num2, num);
                bits = num2;
            }
            this.buffer = (this.buffer << bits) + (value & masks[bits]);
            this.bitsInBuffer += bits;
            if (this.bitsInBuffer == 8)
            {
                this.imageData[this.bytesOffsetWrite] = (byte) this.buffer;
                this.bitsInBuffer = 0;
                this.bytesOffsetWrite++;
            }
        }

        [Obsolete]
        internal void WriteEOL()
        {
            this.WriteTableLine(PdfImage.WhiteMakeUpCodes, 40);
        }

        internal void WriteTableLine(uint[] table, uint line)
        {
            uint num = table[(int) ((IntPtr) (line * 2))];
            uint bits = table[(int) ((IntPtr) ((line * 2) + 1))];
            this.WriteBits(num, bits);
        }
    }
}

