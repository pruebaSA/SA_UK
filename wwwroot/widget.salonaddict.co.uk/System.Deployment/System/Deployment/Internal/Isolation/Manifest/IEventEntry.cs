namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8AD3FC86-AFD3-477a-8FD5-146C291195BB")]
    internal interface IEventEntry
    {
        System.Deployment.Internal.Isolation.Manifest.EventEntry AllData { get; }
        uint EventID { get; }
        uint Level { get; }
        uint Version { get; }
        System.Guid Guid { get; }
        string SubTypeName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint SubTypeValue { get; }
        string DisplayName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint EventNameMicrodomIndex { get; }
    }
}

