﻿namespace MS.Internal.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct BSTRBLOB
    {
        public uint cbSize;
        public IntPtr pData;
    }
}

