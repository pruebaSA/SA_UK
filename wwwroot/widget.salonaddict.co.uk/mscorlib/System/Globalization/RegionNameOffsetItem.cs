namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RegionNameOffsetItem
    {
        internal ushort strOffset;
        internal ushort dataItemIndex;
    }
}

