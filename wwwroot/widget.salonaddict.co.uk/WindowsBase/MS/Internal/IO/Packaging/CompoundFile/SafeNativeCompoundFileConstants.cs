namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;

    internal static class SafeNativeCompoundFileConstants
    {
        internal const uint PROPSETFLAG_ANSI = 2;
        internal const int S_FALSE = 1;
        internal const int S_OK = 0;
        internal const int STATFLAG_NONAME = 1;
        internal const int STATFLAG_NOOPEN = 2;
        internal const int STG_E_ACCESSDENIED = -2147287035;
        internal const int STG_E_FILEALREADYEXISTS = -2147286960;
        internal const int STG_E_FILENOTFOUND = -2147287038;
        internal const int STG_E_INVALIDFLAG = -2147286785;
        internal const int STG_E_INVALIDNAME = -2147286788;
        internal const int STGM_CONVERT = 0x20000;
        internal const int STGM_CREATE = 0x1000;
        internal const int STGM_DELETEONRELEASE = 0x4000000;
        internal const int STGM_DIRECT = 0;
        internal const int STGM_DIRECT_SWMR = 0x400000;
        internal const int STGM_FAILIFTHERE = 0;
        internal const int STGM_NOSCRATCH = 0x100000;
        internal const int STGM_NOSNAPSHOT = 0x200000;
        internal const int STGM_PRIORITY = 0x40000;
        internal const int STGM_READ = 0;
        internal const int STGM_READWRITE = 2;
        internal const int STGM_READWRITE_Bits = 3;
        internal const int STGM_SHARE_DENY_NONE = 0x40;
        internal const int STGM_SHARE_DENY_READ = 0x30;
        internal const int STGM_SHARE_DENY_WRITE = 0x20;
        internal const int STGM_SHARE_EXCLUSIVE = 0x10;
        internal const int STGM_SIMPLE = 0x8000000;
        internal const int STGM_TRANSACTED = 0x10000;
        internal const int STGM_WRITE = 1;
        internal const int STGTY_LOCKBYTES = 3;
        internal const int STGTY_PROPERTY = 4;
        internal const int STGTY_STORAGE = 1;
        internal const int STGTY_STREAM = 2;
        internal const int STREAM_SEEK_CUR = 1;
        internal const int STREAM_SEEK_END = 2;
        internal const int STREAM_SEEK_SET = 0;
    }
}

