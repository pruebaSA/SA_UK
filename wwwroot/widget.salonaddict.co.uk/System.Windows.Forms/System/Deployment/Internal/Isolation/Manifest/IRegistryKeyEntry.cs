namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("186685d1-6673-48c3-bc83-95859bb591df")]
    internal interface IRegistryKeyEntry
    {
        System.Deployment.Internal.Isolation.Manifest.RegistryKeyEntry AllData { get; }
        uint Flags { get; }
        uint Protection { get; }
        string BuildFilter { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        object SecurityDescriptor { [return: MarshalAs(UnmanagedType.Interface)] get; }
        object Values { [return: MarshalAs(UnmanagedType.Interface)] get; }
        object Keys { [return: MarshalAs(UnmanagedType.Interface)] get; }
    }
}

