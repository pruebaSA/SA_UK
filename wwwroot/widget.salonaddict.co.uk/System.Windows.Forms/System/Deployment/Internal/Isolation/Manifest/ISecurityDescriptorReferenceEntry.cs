namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a75b74e9-2c00-4ebb-b3f9-62a670aaa07e")]
    internal interface ISecurityDescriptorReferenceEntry
    {
        System.Deployment.Internal.Isolation.Manifest.SecurityDescriptorReferenceEntry AllData { get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string BuildFilter { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

