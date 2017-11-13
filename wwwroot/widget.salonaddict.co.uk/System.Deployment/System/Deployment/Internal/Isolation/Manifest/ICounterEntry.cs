namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8CD3FC86-AFD3-477a-8FD5-146C291195BB")]
    internal interface ICounterEntry
    {
        System.Deployment.Internal.Isolation.Manifest.CounterEntry AllData { get; }
        Guid CounterSetGuid { get; }
        uint CounterId { get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Description { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint CounterType { get; }
        ulong Attributes { get; }
        uint BaseId { get; }
        uint DefaultScale { get; }
    }
}

