namespace PdfSharp.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SColor
    {
        public byte a;
        public byte r;
        public byte g;
        public byte b;
    }
}

