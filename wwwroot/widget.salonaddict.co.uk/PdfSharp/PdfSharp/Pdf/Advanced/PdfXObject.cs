namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfXObject : PdfDictionary
    {
        public PdfXObject(PdfDocument document) : base(document)
        {
        }

        public class Keys : PdfDictionary.PdfStream.Keys
        {
        }
    }
}

