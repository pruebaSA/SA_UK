namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("8AD3FC86-AFD3-477a-8FD5-146C291195BC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEventMapEntry
    {
        System.Deployment.Internal.Isolation.Manifest.EventMapEntry AllData { get; }
        string MapName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint Value { get; }
        bool IsValueMap { get; }
    }
}

