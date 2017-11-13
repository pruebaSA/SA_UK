namespace PdfSharp.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SCColor
    {
        public float a;
        public float r;
        public float g;
        public float b;
    }
}

