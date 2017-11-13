namespace PdfSharp.Pdf
{
    using System;

    [Flags]
    public enum PdfStringEncoding
    {
        RawEncoding,
        StandardEncoding,
        PDFDocEncoding,
        WinAnsiEncoding,
        MacRomanEncoding,
        MacExpertEncoding,
        Unicode
    }
}

