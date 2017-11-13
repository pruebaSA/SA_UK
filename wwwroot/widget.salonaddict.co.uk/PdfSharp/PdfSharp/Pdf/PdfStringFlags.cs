namespace PdfSharp.Pdf
{
    using System;

    [Flags]
    internal enum PdfStringFlags
    {
        EncodingMask = 15,
        HexLiteral = 0x80,
        MacExpertEncoding = 5,
        MacRomanEncoding = 4,
        PDFDocEncoding = 2,
        RawEncoding = 0,
        StandardEncoding = 1,
        Unicode = 6,
        WinAnsiEncoding = 3
    }
}

