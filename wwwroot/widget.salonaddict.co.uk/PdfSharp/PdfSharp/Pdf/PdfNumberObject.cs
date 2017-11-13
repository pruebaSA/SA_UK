namespace PdfSharp.Pdf
{
    using System;

    public abstract class PdfNumberObject : PdfObject
    {
        protected PdfNumberObject()
        {
        }

        protected PdfNumberObject(PdfDocument document) : base(document)
        {
        }
    }
}

