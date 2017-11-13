namespace PdfSharp.Drawing.Pdf
{
    using System;

    [Flags]
    internal enum DirtyFlags
    {
        ClipPath = 2,
        Ctm = 1,
        LineJoin = 0x20,
        LineWidth = 0x10,
        MiterLimit = 0x40,
        StrokeFill = 0x70
    }
}

