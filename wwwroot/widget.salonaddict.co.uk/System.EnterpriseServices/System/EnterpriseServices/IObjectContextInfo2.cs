﻿namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("594BE71A-4BC4-438b-9197-CFD176248B09")]
    internal interface IObjectContextInfo2
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsInTransaction();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetTransaction();
        Guid GetTransactionId();
        Guid GetActivityId();
        Guid GetContextId();
        Guid GetPartitionId();
        Guid GetApplicationId();
        Guid GetApplicationInstanceId();
    }
}

