﻿namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("8CD3FC85-AFD3-477a-8FD5-146C291195BB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICounterSetEntry
    {
        System.Deployment.Internal.Isolation.Manifest.CounterSetEntry AllData { get; }
        Guid CounterSetGuid { get; }
        Guid ProviderGuid { get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Description { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        bool InstanceType { get; }
    }
}

