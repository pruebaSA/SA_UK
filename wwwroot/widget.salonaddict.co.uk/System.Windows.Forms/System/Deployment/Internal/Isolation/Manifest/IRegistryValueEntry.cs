namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("49e1fe8d-ebb8-4593-8c4e-3e14c845b142")]
    internal interface IRegistryValueEntry
    {
        System.Deployment.Internal.Isolation.Manifest.RegistryValueEntry AllData { get; }
        uint Flags { get; }
        uint OperationHint { get; }
        uint Type { get; }
        string Value { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string BuildFilter { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

