namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("9f27c750-7dfb-46a1-a673-52e53e2337a9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDirectoryEntry
    {
        System.Deployment.Internal.Isolation.Manifest.DirectoryEntry AllData { get; }
        uint Flags { get; }
        uint Protection { get; }
        string BuildFilter { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        object SecurityDescriptor { [return: MarshalAs(UnmanagedType.Interface)] get; }
    }
}

