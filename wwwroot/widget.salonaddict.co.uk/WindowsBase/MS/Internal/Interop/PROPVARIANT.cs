namespace MS.Internal.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct PROPVARIANT
    {
        internal VARTYPE vt;
        internal ushort wReserved1;
        internal ushort wReserved2;
        internal ushort wReserved3;
        internal PropVariantUnion union;
    }
}

