namespace MigraDoc.DocumentObjectModel
{
    using System;

    [Flags]
    internal enum FontProperties
    {
        Bold = 4,
        Border = 0x40,
        Color = 0x20,
        Italic = 8,
        Name = 1,
        None = 0,
        Size = 2,
        Subscript = 0x100,
        Superscript = 0x80,
        Underline = 0x10
    }
}

