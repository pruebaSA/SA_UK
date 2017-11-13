namespace MS.Internal.Interop
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum VARTYPE : short
    {
        VT_BSTR = 8,
        VT_FILETIME = 0x40,
        VT_LPSTR = 30
    }
}

