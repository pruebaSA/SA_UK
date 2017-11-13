﻿namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1583EFE9-832F-4d08-B041-CAC5ACEDB948")]
    internal interface IEntryPointEntry
    {
        System.Deployment.Internal.Isolation.Manifest.EntryPointEntry AllData { get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string CommandLine_File { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string CommandLine_Parameters { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        System.Deployment.Internal.Isolation.IReferenceIdentity Identity { get; }
        uint Flags { get; }
    }
}
