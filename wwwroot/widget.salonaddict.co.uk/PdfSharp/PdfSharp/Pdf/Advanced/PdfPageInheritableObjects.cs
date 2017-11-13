namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    internal class PdfPageInheritableObjects : PdfDictionary
    {
        private PdfRectangle cropBox;
        private PdfRectangle mediaBox;
        private int rotate;

        public PdfRectangle CropBox
        {
            get => 
                this.cropBox;
            set
            {
                this.cropBox = value;
            }
        }

        public PdfRectangle MediaBox
        {
            get => 
                this.mediaBox;
            set
            {
                this.mediaBox = value;
            }
        }

        public int Rotate
        {
            get => 
                this.rotate;
            set
            {
                if ((value % 90) != 0)
                {
                    throw new ArgumentException("Rotate", "The value must be a multiple of 90.");
                }
                this.rotate = value;
            }
        }
    }
}

