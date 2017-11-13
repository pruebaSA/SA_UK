namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct IDOffsetItem
    {
        internal int actualCultureID;
        internal ushort dataItemIndex;
        internal ushort strOffset;
    }
}

