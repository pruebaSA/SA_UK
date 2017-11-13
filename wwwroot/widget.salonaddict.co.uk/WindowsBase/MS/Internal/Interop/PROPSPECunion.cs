namespace MS.Internal.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit), FriendAccessAllowed]
    internal struct PROPSPECunion
    {
        [FieldOffset(0)]
        internal IntPtr name;
        [FieldOffset(0)]
        internal uint propId;
    }
}

