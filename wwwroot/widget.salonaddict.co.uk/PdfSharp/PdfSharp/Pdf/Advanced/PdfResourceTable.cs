namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    internal class PdfResourceTable
    {
        protected PdfDocument owner;

        public PdfResourceTable(PdfDocument owner)
        {
            this.owner = owner;
        }
    }
}

