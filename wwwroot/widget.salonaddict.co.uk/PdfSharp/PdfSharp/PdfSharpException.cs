namespace PdfSharp
{
    using System;

    public class PdfSharpException : Exception
    {
        public PdfSharpException()
        {
        }

        public PdfSharpException(string message) : base(message)
        {
        }

        public PdfSharpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

