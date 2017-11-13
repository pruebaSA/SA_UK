namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0C66F299-E08E-48c5-9264-7CCBEB4D5CBB")]
    internal interface IFileAssociationEntry
    {
        System.Deployment.Internal.Isolation.Manifest.FileAssociationEntry AllData { get; }
        string Extension { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Description { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string ProgID { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string DefaultIcon { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Parameter { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

