﻿namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, Flags, ComVisible(true)]
    public enum FileShare
    {
        Delete = 4,
        Inheritable = 0x10,
        None = 0,
        Read = 1,
        ReadWrite = 3,
        Write = 2
    }
}

