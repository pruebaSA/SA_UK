namespace MigraDoc.DocumentObjectModel
{
    using System;

    [Flags]
    public enum TextFormat
    {
        Bold = 1,
        Italic = 4,
        NotBold = 3,
        NotItalic = 12,
        NoUnderline = 0x30,
        Underline = 0x10
    }
}

