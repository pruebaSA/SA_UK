namespace MS.Internal.FontRasterization
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct Point
    {
        public int x;
        public int y;
    }
}

