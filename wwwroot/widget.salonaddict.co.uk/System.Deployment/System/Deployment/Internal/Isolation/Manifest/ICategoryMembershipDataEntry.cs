﻿namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("DA0C3B27-6B6B-4b80-A8F8-6CE14F4BC0A4")]
    internal interface ICategoryMembershipDataEntry
    {
        System.Deployment.Internal.Isolation.Manifest.CategoryMembershipDataEntry AllData { get; }
        uint index { get; }
        string Xml { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Description { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

