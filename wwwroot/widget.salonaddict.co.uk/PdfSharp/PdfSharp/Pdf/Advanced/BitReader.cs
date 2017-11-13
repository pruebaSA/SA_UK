namespace PdfSharp.Pdf.Advanced
{
    using System;
    using System.Runtime.InteropServices;

    internal class BitReader
    {
        private uint bitsInBuffer;
        private readonly uint bitsTotal;
        private byte buffer;
        private readonly uint bytesFileOffset;
        private uint bytesOffsetRead;
        private readonly byte[] imageBits;

        internal BitReader(byte[] imageBits, uint bytesFileOffset, uint bits)
        {
            this.imageBits = imageBits;
            this.bytesFileOffset = bytesFileOffset;
            this.bitsTotal = bits;
            this.bytesOffsetRead = bytesFileOffset;
            this.buffer = imageBits[this.bytesOffsetRead];
            this.bitsInBuffer = 8;
        }

        internal bool GetBit(uint position)
        {
            uint num;
            if (position >= this.bitsTotal)
            {
                return false;
            }
            this.SetPosition(position);
            return ((this.PeekByte(out num) & 0x80) > 0);
        }

        internal void NextByte()
        {
            this.buffer = this.imageBits[(int) ((IntPtr) (++this.bytesOffsetRead))];
            this.bitsInBuffer = 8;
        }

        internal byte PeekByte(out uint bits)
        {
            if (this.bitsInBuffer == 8)
            {
                bits = 8;
                return this.buffer;
            }
            bits = this.bitsInBuffer;
            return (byte) (this.buffer << (8 - this.bitsInBuffer));
        }

        internal void SetPosition(uint position)
        {
            this.bytesOffsetRead = this.bytesFileOffset + (position >> 3);
            this.buffer = this.imageBits[this.bytesOffsetRead];
            this.bitsInBuffer = 8 - (position & 7);
        }

        internal void SkipBits(uint bits)
        {
            if (bits == this.bitsInBuffer)
            {
                this.NextByte();
            }
            else
            {
                this.bitsInBuffer -= bits;
            }
        }
    }
}

