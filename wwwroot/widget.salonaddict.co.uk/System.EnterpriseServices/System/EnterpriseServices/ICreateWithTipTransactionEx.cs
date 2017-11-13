﻿namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("455ACF59-5345-11D2-99CF-00C04F797BC9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICreateWithTipTransactionEx
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object CreateInstance(string bstrTipUrl, [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);
    }
}

