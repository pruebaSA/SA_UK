namespace PdfSharp.Pdf
{
    using System;

    [Flags]
    internal enum DocumentState
    {
        Created = 1,
        Disposed = 0x8000,
        Imported = 2
    }
}

