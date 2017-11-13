namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public abstract class MatrixCode : CodeBase
    {
        internal int columns;
        internal string encoding;
        internal XImage matrixImage;
        internal int rows;

        public MatrixCode(string text, string encoding, int rows, int columns, XSize size) : base(text, size, CodeDirection.LeftToRight)
        {
            this.encoding = encoding;
            if (string.IsNullOrEmpty(this.encoding))
            {
                this.encoding = new string('a', base.text.Length);
            }
            if (columns < rows)
            {
                this.rows = columns;
                this.columns = rows;
            }
            else
            {
                this.columns = columns;
                this.rows = rows;
            }
            this.Text = text;
        }

        protected override void CheckCode(string text)
        {
        }

        protected internal abstract void Render(XGraphics gfx, XBrush brush, XPoint center);

        public int Columns
        {
            get => 
                this.columns;
            set
            {
                this.columns = value;
                this.matrixImage = null;
            }
        }

        public string Encoding
        {
            get => 
                this.encoding;
            set
            {
                this.encoding = value;
                this.matrixImage = null;
            }
        }

        public int Rows
        {
            get => 
                this.rows;
            set
            {
                this.rows = value;
                this.matrixImage = null;
            }
        }

        public string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
                this.matrixImage = null;
            }
        }
    }
}

