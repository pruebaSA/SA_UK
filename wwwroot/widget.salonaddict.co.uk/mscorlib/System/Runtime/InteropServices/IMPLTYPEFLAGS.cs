﻿namespace System.Runtime.InteropServices
{
    using System;

    [Serializable, Obsolete("Use System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS instead. http://go.microsoft.com/fwlink/?linkid=14202", false), Flags]
    public enum IMPLTYPEFLAGS
    {
        IMPLTYPEFLAG_FDEFAULT = 1,
        IMPLTYPEFLAG_FDEFAULTVTABLE = 8,
        IMPLTYPEFLAG_FRESTRICTED = 4,
        IMPLTYPEFLAG_FSOURCE = 2
    }
}

