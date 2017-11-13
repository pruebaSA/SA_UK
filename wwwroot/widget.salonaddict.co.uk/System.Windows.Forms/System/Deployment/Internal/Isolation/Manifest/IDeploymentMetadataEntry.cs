namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("CFA3F59F-334D-46bf-A5A5-5D11BB2D7EBC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDeploymentMetadataEntry
    {
        System.Deployment.Internal.Isolation.Manifest.DeploymentMetadataEntry AllData { get; }
        string DeploymentProviderCodebase { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string MinimumRequiredVersion { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        ushort MaximumAge { get; }
        byte MaximumAge_Unit { get; }
        uint DeploymentFlags { get; }
    }
}

