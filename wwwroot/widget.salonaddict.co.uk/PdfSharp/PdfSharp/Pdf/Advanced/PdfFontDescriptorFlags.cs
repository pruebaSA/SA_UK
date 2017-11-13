namespace PdfSharp.Pdf.Advanced
{
    using System;

    [Flags]
    internal enum PdfFontDescriptorFlags
    {
        AllCap = 0x10000,
        FixedPitch = 1,
        ForceBold = 0x40000,
        Italic = 0x40,
        Nonsymbolic = 0x20,
        Script = 8,
        Serif = 2,
        SmallCap = 0x20000,
        Symbolic = 4
    }
}

