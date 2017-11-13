namespace PdfSharp.Pdf.IO
{
    using PdfSharp;
    using System;

    public class PdfReaderException : PdfSharpException
    {
        public PdfReaderException()
        {
        }

        public PdfReaderException(string message) : base(message)
        {
        }

        public PdfReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

