namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, Guid("FD47B733-AFBC-45e4-B7C2-BBEB1D9F766C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyReferenceEntry
    {
        System.Deployment.Internal.Isolation.Manifest.AssemblyReferenceEntry AllData { get; }
        System.Deployment.Internal.Isolation.IReferenceIdentity ReferenceIdentity { get; }
        uint Flags { get; }
        System.Deployment.Internal.Isolation.Manifest.IAssemblyReferenceDependentAssemblyEntry DependentAssembly { get; }
    }
}

