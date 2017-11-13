namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct EndianessHeader
    {
        internal uint leOffset;
        internal uint beOffset;
    }
}

