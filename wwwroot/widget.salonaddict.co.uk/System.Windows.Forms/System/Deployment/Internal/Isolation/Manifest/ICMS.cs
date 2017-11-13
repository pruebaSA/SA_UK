namespace System.Deployment.Internal.Isolation.Manifest
{
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, Guid("a504e5b0-8ccf-4cb4-9902-c9d1b9abd033"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICMS
    {
        System.Deployment.Internal.Isolation.IDefinitionIdentity Identity { get; }
        System.Deployment.Internal.Isolation.ISection FileSection { get; }
        System.Deployment.Internal.Isolation.ISection CategoryMembershipSection { get; }
        System.Deployment.Internal.Isolation.ISection COMRedirectionSection { get; }
        System.Deployment.Internal.Isolation.ISection ProgIdRedirectionSection { get; }
        System.Deployment.Internal.Isolation.ISection CLRSurrogateSection { get; }
        System.Deployment.Internal.Isolation.ISection AssemblyReferenceSection { get; }
        System.Deployment.Internal.Isolation.ISection WindowClassSection { get; }
        System.Deployment.Internal.Isolation.ISection StringSection { get; }
        System.Deployment.Internal.Isolation.ISection EntryPointSection { get; }
        System.Deployment.Internal.Isolation.ISection PermissionSetSection { get; }
        System.Deployment.Internal.Isolation.ISectionEntry MetadataSectionEntry { get; }
        System.Deployment.Internal.Isolation.ISection AssemblyRequestSection { get; }
        System.Deployment.Internal.Isolation.ISection RegistryKeySection { get; }
        System.Deployment.Internal.Isolation.ISection DirectorySection { get; }
        System.Deployment.Internal.Isolation.ISection FileAssociationSection { get; }
        System.Deployment.Internal.Isolation.ISection EventSection { get; }
        System.Deployment.Internal.Isolation.ISection EventMapSection { get; }
        System.Deployment.Internal.Isolation.ISection EventTagSection { get; }
        System.Deployment.Internal.Isolation.ISection CounterSetSection { get; }
        System.Deployment.Internal.Isolation.ISection CounterSection { get; }
    }
}

