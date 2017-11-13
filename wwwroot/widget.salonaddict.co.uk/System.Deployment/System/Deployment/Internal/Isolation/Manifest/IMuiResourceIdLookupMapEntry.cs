namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("24abe1f7-a396-4a03-9adf-1d5b86a5569f")]
    internal interface IMuiResourceIdLookupMapEntry
    {
        System.Deployment.Internal.Isolation.Manifest.MuiResourceIdLookupMapEntry AllData { get; }
        uint Count { get; }
    }
}

