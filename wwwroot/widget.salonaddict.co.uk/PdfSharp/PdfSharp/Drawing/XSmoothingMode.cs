namespace PdfSharp.Drawing
{
    using System;

    [Flags]
    public enum XSmoothingMode
    {
        AntiAlias = 4,
        Default = 0,
        HighQuality = 2,
        HighSpeed = 1,
        Invalid = -1,
        None = 3
    }
}

