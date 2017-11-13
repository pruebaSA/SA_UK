namespace PdfSharp.Pdf.Advanced
{
    using System;

    internal class MonochromeMask
    {
        private int bitsWritten;
        private int byteBuffer;
        private readonly byte[] maskData;
        private readonly int sizeX;
        private readonly int sizeY;
        private int writeOffset;

        public MonochromeMask(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            int num = ((sizeX + 7) / 8) * sizeY;
            this.maskData = new byte[num];
            this.StartLine(0);
        }

        public void AddPel(bool isTransparent)
        {
            if (this.bitsWritten < this.sizeX)
            {
                if (isTransparent)
                {
                    this.byteBuffer = (this.byteBuffer << 1) + 1;
                }
                else
                {
                    this.byteBuffer = this.byteBuffer << 1;
                }
                this.bitsWritten++;
                if ((this.bitsWritten & 7) == 0)
                {
                    this.maskData[this.writeOffset] = (byte) this.byteBuffer;
                    this.writeOffset++;
                    this.byteBuffer = 0;
                }
                else if (this.bitsWritten == this.sizeX)
                {
                    int num = 8 - (this.bitsWritten & 7);
                    this.byteBuffer = this.byteBuffer << num;
                    this.maskData[this.writeOffset] = (byte) this.byteBuffer;
                }
            }
        }

        public void AddPel(int shade)
        {
            this.AddPel(shade < 0x80);
        }

        public void StartLine(int newCurrentLine)
        {
            this.bitsWritten = 0;
            this.byteBuffer = 0;
            this.writeOffset = ((this.sizeX + 7) / 8) * ((this.sizeY - 1) - newCurrentLine);
        }

        public byte[] MaskData =>
            this.maskData;
    }
}

