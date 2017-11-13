﻿namespace System.EnterpriseServices.CompensatingResourceManager
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("9C51D821-C98B-11D1-82FB-00A0C91EEDE9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface _IFormatLogRecords
    {
        int GetColumnCount();
        object GetColumnHeaders();
        object GetColumn([In] _LogRecord crmLogRec);
        object GetColumnVariants([In] object logRecord);
    }
}

