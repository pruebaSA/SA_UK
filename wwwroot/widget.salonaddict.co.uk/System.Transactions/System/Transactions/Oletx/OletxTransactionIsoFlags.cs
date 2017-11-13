﻿namespace System.Transactions.Oletx
{
    using System;

    [Flags]
    internal enum OletxTransactionIsoFlags
    {
        ISOFLAG_NONE = 0,
        ISOFLAG_OPTIMISTIC = 0x10,
        ISOFLAG_READONLY = 0x20,
        ISOFLAG_RETAIN_ABORT = 8,
        ISOFLAG_RETAIN_ABORT_DC = 4,
        ISOFLAG_RETAIN_ABORT_NO = 12,
        ISOFLAG_RETAIN_BOTH = 10,
        ISOFLAG_RETAIN_COMMIT = 2,
        ISOFLAG_RETAIN_COMMIT_DC = 1,
        ISOFLAG_RETAIN_COMMIT_NO = 3,
        ISOFLAG_RETAIN_DONTCARE = 5,
        ISOFLAG_RETAIN_NONE = 15
    }
}

