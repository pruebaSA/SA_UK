namespace MS.Internal.FontRasterization
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct Transform
    {
        public int a00;
        public int a10;
        public int a01;
        public int a11;
    }
}

