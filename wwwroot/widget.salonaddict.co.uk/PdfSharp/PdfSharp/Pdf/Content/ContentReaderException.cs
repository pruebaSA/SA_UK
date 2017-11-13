namespace PdfSharp.Pdf.Content
{
    using PdfSharp;
    using System;

    public class ContentReaderException : PdfSharpException
    {
        public ContentReaderException()
        {
        }

        public ContentReaderException(string message) : base(message)
        {
        }

        public ContentReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

