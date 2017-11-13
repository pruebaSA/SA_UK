namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;
    using System.Drawing;

    internal class DataMatrixImage
    {
        private int columns;
        private static Ecc200Block[] ecc200Sizes = new Ecc200Block[] { 
            new Ecc200Block(10, 10, 10, 10, 3, 3, 5), new Ecc200Block(12, 12, 12, 12, 5, 5, 7), new Ecc200Block(8, 0x12, 8, 0x12, 5, 5, 7), new Ecc200Block(14, 14, 14, 14, 8, 8, 10), new Ecc200Block(8, 0x20, 8, 0x10, 10, 10, 11), new Ecc200Block(0x10, 0x10, 0x10, 0x10, 12, 12, 12), new Ecc200Block(12, 0x1a, 12, 0x1a, 0x10, 0x10, 14), new Ecc200Block(0x12, 0x12, 0x12, 0x12, 0x12, 0x12, 14), new Ecc200Block(20, 20, 20, 20, 0x16, 0x16, 0x12), new Ecc200Block(12, 0x24, 12, 0x12, 0x16, 0x16, 0x12), new Ecc200Block(0x16, 0x16, 0x16, 0x16, 30, 30, 20), new Ecc200Block(0x10, 0x24, 0x10, 0x12, 0x20, 0x20, 0x18), new Ecc200Block(0x18, 0x18, 0x18, 0x18, 0x24, 0x24, 0x18), new Ecc200Block(0x1a, 0x1a, 0x1a, 0x1a, 0x2c, 0x2c, 0x1c), new Ecc200Block(0x10, 0x30, 0x10, 0x18, 0x31, 0x31, 0x1c), new Ecc200Block(0x20, 0x20, 0x10, 0x10, 0x3e, 0x3e, 0x24),
            new Ecc200Block(0x24, 0x24, 0x12, 0x12, 0x56, 0x56, 0x2a), new Ecc200Block(40, 40, 20, 20, 0x72, 0x72, 0x30), new Ecc200Block(0x2c, 0x2c, 0x16, 0x16, 0x90, 0x90, 0x38), new Ecc200Block(0x30, 0x30, 0x18, 0x18, 0xae, 0xae, 0x44), new Ecc200Block(0x34, 0x34, 0x1a, 0x1a, 0xcc, 0x66, 0x2a), new Ecc200Block(0x40, 0x40, 0x10, 0x10, 280, 140, 0x38), new Ecc200Block(0x48, 0x48, 0x12, 0x12, 0x170, 0x5c, 0x24), new Ecc200Block(80, 80, 20, 20, 0x1c8, 0x72, 0x30), new Ecc200Block(0x58, 0x58, 0x16, 0x16, 0x240, 0x90, 0x38), new Ecc200Block(0x60, 0x60, 0x18, 0x18, 0x2b8, 0xae, 0x44), new Ecc200Block(0x68, 0x68, 0x1a, 0x1a, 0x330, 0x88, 0x38), new Ecc200Block(120, 120, 20, 20, 0x41a, 0xaf, 0x44), new Ecc200Block(0x84, 0x84, 0x16, 0x16, 0x518, 0xa3, 0x3e), new Ecc200Block(0x90, 0x90, 0x18, 0x18, 0x616, 0x9c, 0x3e), new Ecc200Block(0, 0, 0, 0, 0, 0, 0)
        };
        private string encoding;
        private int rows;
        private string text;

        public DataMatrixImage(string text, string encoding, int rows, int columns)
        {
            this.text = text;
            this.encoding = encoding;
            this.rows = rows;
            this.columns = columns;
        }

        public XImage CreateImage(char[] code, int size) => 
            this.CreateImage(code, size, size, 10);

        public XImage CreateImage(char[] code, int rows, int columns) => 
            this.CreateImage(code, rows, columns, 10);

        public XImage CreateImage(char[] code, int rows, int columns, int pixelsize)
        {
            Bitmap bitmap = new Bitmap(columns * pixelsize, rows * pixelsize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, columns * pixelsize, rows * pixelsize));
                for (int i = rows - 1; i >= 0; i--)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (code[(((rows - 1) - i) * columns) + j] == '\x0001')
                        {
                            graphics.FillRectangle(Brushes.Black, j * pixelsize, i * pixelsize, pixelsize, pixelsize);
                        }
                    }
                }
                Pen pen = new Pen(Color.Firebrick, (float) pixelsize);
                graphics.DrawLine(pen, 0, 0, rows * pixelsize, columns * pixelsize);
                graphics.DrawLine(pen, columns * pixelsize, 0, 0, rows * pixelsize);
            }
            XImage image = XImage.FromGdiPlusImage(bitmap);
            image.Interpolate = false;
            return image;
        }

        internal char[] DataMatrix()
        {
            int columns = this.columns;
            int rows = this.rows;
            Ecc200Block block = new Ecc200Block(0, 0, 0, 0, 0, 0, 0);
            foreach (Ecc200Block block2 in ecc200Sizes)
            {
                block = block2;
                if ((block.Width == this.columns) && (block.Height == this.rows))
                {
                    break;
                }
            }
            char[] chArray = new char[columns * rows];
            Random random = new Random();
            for (int i = 0; i < columns; i++)
            {
                chArray[i] = '\x0001';
            }
            for (int j = 1; j < rows; j++)
            {
                chArray[j * rows] = '\x0001';
                for (int k = 1; k < columns; k++)
                {
                    chArray[(j * rows) + k] = (char) random.Next(2);
                }
            }
            if ((chArray != null) && (columns != 0))
            {
                return chArray;
            }
            return null;
        }

        public XImage DrawMatrix() => 
            this.CreateImage(this.DataMatrix(), this.rows, this.columns);

        public static XImage GenerateMatrixImage(string text, string encoding, int rows, int columns)
        {
            DataMatrixImage image = new DataMatrixImage(text, encoding, rows, columns);
            return image.DrawMatrix();
        }

        internal char[] Iec16022Ecc200(int columns, int rows, string encoding, int barcodelen, string barcode, int len, int max, int ecc) => 
            null;
    }
}

