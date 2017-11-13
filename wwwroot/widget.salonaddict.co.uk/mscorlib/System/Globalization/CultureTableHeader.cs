namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct CultureTableHeader
    {
        internal uint version;
        internal ushort hash0;
        internal ushort hash1;
        internal ushort hash2;
        internal ushort hash3;
        internal ushort hash4;
        internal ushort hash5;
        internal ushort hash6;
        internal ushort hash7;
        internal ushort headerSize;
        internal ushort numLcidItems;
        internal ushort numCultureItems;
        internal ushort sizeCultureItem;
        internal uint offsetToCultureItemData;
        internal ushort numCultureNames;
        internal ushort numRegionNames;
        internal uint cultureIDTableOffset;
        internal uint cultureNameTableOffset;
        internal uint regionNameTableOffset;
        internal ushort numCalendarItems;
        internal ushort sizeCalendarItem;
        internal uint offsetToCalendarItemData;
        internal uint offsetToDataPool;
        internal ushort Unused_numIetfNames;
        internal ushort Unused_Padding;
        internal uint Unused_ietfNameTableOffset;
    }
}

