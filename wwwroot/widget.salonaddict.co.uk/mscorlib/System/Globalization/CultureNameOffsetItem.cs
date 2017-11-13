namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CultureNameOffsetItem
    {
        internal ushort strOffset;
        internal ushort dataItemIndex;
        internal int actualCultureID;
    }
}

