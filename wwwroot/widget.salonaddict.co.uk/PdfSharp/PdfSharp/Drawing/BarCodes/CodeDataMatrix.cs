namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public class CodeDataMatrix : MatrixCode
    {
        private int quietZone;

        public CodeDataMatrix() : this("", "", 0x1a, 0x1a, 0, XSize.Empty)
        {
        }

        public CodeDataMatrix(string code, int length) : this(code, "", length, length, 0, XSize.Empty)
        {
        }

        public CodeDataMatrix(string code, int length, XSize size) : this(code, "", length, length, 0, size)
        {
        }

        public CodeDataMatrix(string code, int rows, int columns) : this(code, "", rows, columns, 0, XSize.Empty)
        {
        }

        public CodeDataMatrix(string code, DataMatrixEncoding dmEncoding, int length, XSize size) : this(code, CreateEncoding(dmEncoding, code.Length), length, length, 0, size)
        {
        }

        public CodeDataMatrix(string code, int rows, int columns, XSize size) : this(code, "", rows, columns, 0, size)
        {
        }

        public CodeDataMatrix(string code, int rows, int columns, int quietZone) : this(code, "", rows, columns, quietZone, XSize.Empty)
        {
        }

        public CodeDataMatrix(string code, DataMatrixEncoding dmEncoding, int rows, int columns, XSize size) : this(code, CreateEncoding(dmEncoding, code.Length), rows, columns, 0, size)
        {
        }

        public CodeDataMatrix(string code, string encoding, int rows, int columns, int quietZone, XSize size) : base(code, encoding, rows, columns, size)
        {
            this.QuietZone = quietZone;
        }

        protected override void CheckCode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            new DataMatrixImage(base.Text, base.Encoding, base.Rows, base.Columns).Iec16022Ecc200(base.Columns, base.Rows, base.Encoding, base.Text.Length, base.Text, 0, 0, 0);
        }

        private static string CreateEncoding(DataMatrixEncoding dmEncoding, int length)
        {
            switch (dmEncoding)
            {
                case DataMatrixEncoding.Ascii:
                    return new string('a', length);

                case DataMatrixEncoding.C40:
                    return new string('c', length);

                case DataMatrixEncoding.Text:
                    return new string('t', length);

                case DataMatrixEncoding.X12:
                    return new string('x', length);

                case DataMatrixEncoding.EDIFACT:
                    return new string('e', length);

                case DataMatrixEncoding.Base256:
                    return new string('b', length);
            }
            return "";
        }

        protected internal override void Render(XGraphics gfx, XBrush brush, XPoint position)
        {
            XGraphicsState state = gfx.Save();
            switch (base.direction)
            {
                case CodeDirection.BottomToTop:
                    gfx.RotateAtTransform(-90.0, position);
                    break;

                case CodeDirection.RightToLeft:
                    gfx.RotateAtTransform(180.0, position);
                    break;

                case CodeDirection.TopToBottom:
                    gfx.RotateAtTransform(90.0, position);
                    break;
            }
            XPoint point = position + CodeBase.CalcDistance(base.anchor, AnchorType.TopLeft, base.size);
            if (base.matrixImage == null)
            {
                base.matrixImage = DataMatrixImage.GenerateMatrixImage(base.Text, base.Encoding, base.Rows, base.Columns);
            }
            if (this.QuietZone > 0)
            {
                XSize size = new XSize(this.size.width, this.size.height);
                size.width = (size.width / ((double) (base.Columns + (2 * this.QuietZone)))) * base.Columns;
                size.height = (size.height / ((double) (base.Rows + (2 * this.QuietZone)))) * base.Rows;
                XPoint point2 = new XPoint(point.X, point.Y);
                point2.X += (this.size.width / ((double) (base.Columns + (2 * this.QuietZone)))) * this.QuietZone;
                point2.Y += (this.size.height / ((double) (base.Rows + (2 * this.QuietZone)))) * this.QuietZone;
                gfx.DrawRectangle(XBrushes.White, point.x, point.y, this.size.width, this.size.height);
                gfx.DrawImage(base.matrixImage, point2.x, point2.y, size.width, size.height);
            }
            else
            {
                gfx.DrawImage(base.matrixImage, point.x, point.y, this.size.width, this.size.height);
            }
            gfx.Restore(state);
        }

        public void SetEncoding(DataMatrixEncoding dmEncoding)
        {
            base.Encoding = CreateEncoding(dmEncoding, base.Text.Length);
        }

        public int QuietZone
        {
            get => 
                this.quietZone;
            set
            {
                this.quietZone = value;
            }
        }
    }
}

