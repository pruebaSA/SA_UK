namespace PdfSharp.Drawing.BarCodes
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Ecc200Block
    {
        public int Height;
        public int Width;
        public int CellHeight;
        public int CellWidth;
        public int Bytes;
        public int DataBlock;
        public int RSBlock;
        public Ecc200Block(int h, int w, int ch, int cw, int bytes, int datablock, int rsblock)
        {
            this.Height = h;
            this.Width = w;
            this.CellHeight = ch;
            this.CellWidth = cw;
            this.Bytes = bytes;
            this.DataBlock = datablock;
            this.RSBlock = rsblock;
        }
    }
}

