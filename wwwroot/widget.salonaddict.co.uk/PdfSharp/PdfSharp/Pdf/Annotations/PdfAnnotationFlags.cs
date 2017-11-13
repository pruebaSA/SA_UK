namespace PdfSharp.Pdf.Annotations
{
    using System;

    public enum PdfAnnotationFlags
    {
        Hidden = 2,
        Invisible = 1,
        Locked = 0x80,
        NoRotate = 0x10,
        NoView = 0x20,
        NoZoom = 8,
        Print = 4,
        ReadOnly = 0x40,
        ToggleNoView = 0x100
    }
}

