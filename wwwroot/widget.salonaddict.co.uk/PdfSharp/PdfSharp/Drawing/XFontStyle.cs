namespace PdfSharp.Drawing
{
    using System;

    [Flags]
    public enum XFontStyle
    {
        Bold = 1,
        BoldItalic = 3,
        Italic = 2,
        Regular = 0,
        Strikeout = 8,
        Underline = 4
    }
}

