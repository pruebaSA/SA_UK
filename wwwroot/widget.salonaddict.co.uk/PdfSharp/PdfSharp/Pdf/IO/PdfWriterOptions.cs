namespace PdfSharp.Pdf.IO
{
    using System;

    [Flags]
    public enum PdfWriterOptions
    {
        Regular,
        OmitStream,
        OmitInflation
    }
}

