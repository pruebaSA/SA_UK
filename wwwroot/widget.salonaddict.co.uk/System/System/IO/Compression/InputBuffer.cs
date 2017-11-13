﻿namespace System.IO.Compression
{
    using System;

    internal class InputBuffer
    {
        private uint bitBuffer;
        private int bitsInBuffer;
        private byte[] buffer;
        private int end;
        private int start;

        public int CopyTo(byte[] output, int offset, int length)
        {
            int num = 0;
            while ((this.bitsInBuffer > 0) && (length > 0))
            {
                output[offset++] = (byte) this.bitBuffer;
                this.bitBuffer = this.bitBuffer >> 8;
                this.bitsInBuffer -= 8;
                length--;
                num++;
            }
            if (length == 0)
            {
                return num;
            }
            int num2 = this.end - this.start;
            if (length > num2)
            {
                length = num2;
            }
            Array.Copy(this.buffer, this.start, output, offset, length);
            this.start += length;
            return (num + length);
        }

        public bool EnsureBitsAvailable(int count)
        {
            if (this.bitsInBuffer < count)
            {
                if (this.NeedsInput())
                {
                    return false;
                }
                this.bitBuffer |= (uint) (this.buffer[this.start++] << this.bitsInBuffer);
                this.bitsInBuffer += 8;
                if (this.bitsInBuffer < count)
                {
                    if (this.NeedsInput())
                    {
                        return false;
                    }
                    this.bitBuffer |= (uint) (this.buffer[this.start++] << this.bitsInBuffer);
                    this.bitsInBuffer += 8;
                }
            }
            return true;
        }

        private uint GetBitMask(int count) => 
            ((uint) ((((int) 1) << count) - 1));

        public int GetBits(int count)
        {
            if (!this.EnsureBitsAvailable(count))
            {
                return -1;
            }
            int num = (int) (this.bitBuffer & this.GetBitMask(count));
            this.bitBuffer = this.bitBuffer >> count;
            this.bitsInBuffer -= count;
            return num;
        }

        public bool NeedsInput() => 
            (this.start == this.end);

        public void SetInput(byte[] buffer, int offset, int length)
        {
            this.buffer = buffer;
            this.start = offset;
            this.end = offset + length;
        }

        public void SkipBits(int n)
        {
            this.bitBuffer = this.bitBuffer >> n;
            this.bitsInBuffer -= n;
        }

        public void SkipToByteBoundary()
        {
            this.bitBuffer = this.bitBuffer >> (this.bitsInBuffer % 8);
            this.bitsInBuffer -= this.bitsInBuffer % 8;
        }

        public uint TryLoad16Bits()
        {
            if (this.bitsInBuffer < 8)
            {
                if (this.start < this.end)
                {
                    this.bitBuffer |= (uint) (this.buffer[this.start++] << this.bitsInBuffer);
                    this.bitsInBuffer += 8;
                }
                if (this.start < this.end)
                {
                    this.bitBuffer |= (uint) (this.buffer[this.start++] << this.bitsInBuffer);
                    this.bitsInBuffer += 8;
                }
            }
            else if ((this.bitsInBuffer < 0x10) && (this.start < this.end))
            {
                this.bitBuffer |= (uint) (this.buffer[this.start++] << this.bitsInBuffer);
                this.bitsInBuffer += 8;
            }
            return this.bitBuffer;
        }

        public int AvailableBits =>
            this.bitsInBuffer;

        public int AvailableBytes =>
            ((this.end - this.start) + (this.bitsInBuffer / 8));
    }
}

