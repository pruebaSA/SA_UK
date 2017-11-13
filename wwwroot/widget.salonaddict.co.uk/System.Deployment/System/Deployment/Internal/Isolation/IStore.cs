namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a5c62f6d-5e3e-4cd9-b345-6b281d7a1d1e")]
    internal interface IStore
    {
        void Transact([In] IntPtr cOperation, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.StoreTransactionOperation[] rgOperations, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] rgDispositions, [Out, MarshalAs(UnmanagedType.LPArray)] int[] rgResults);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object BindReferenceToAssembly([In] uint Flags, [In] System.Deployment.Internal.Isolation.IReferenceIdentity ReferenceIdentity, [In] uint cDeploymentsToIgnore, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionIdentity[] DefinitionIdentity_DeploymentsToIgnore, [In] ref Guid riid);
        void CalculateDelimiterOfDeploymentsBasedOnQuota([In] uint dwFlags, [In] IntPtr cDeployments, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionAppId[] rgpIDefinitionAppId_Deployments, [In] ref System.Deployment.Internal.Isolation.StoreApplicationReference InstallerReference, [In] ulong ulonglongQuota, [In, Out] ref IntPtr Delimiter, [In, Out] ref ulong SizeSharedWithExternalDeployment, [In, Out] ref ulong SizeConsumedByInputDeploymentArray);
        IntPtr BindDefinitions([In] uint Flags, [In, MarshalAs(UnmanagedType.SysInt)] IntPtr Count, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionIdentity[] DefsToBind, [In] uint DeploymentsToIgnore, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionIdentity[] DefsToIgnore);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetAssemblyInformation([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumAssemblies([In] uint Flags, [In] System.Deployment.Internal.Isolation.IReferenceIdentity ReferenceIdentity_ToMatch, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumFiles([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumInstallationReferences([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LockAssemblyPath([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity, out IntPtr Cookie);
        void ReleaseAssemblyPath([In] IntPtr Cookie);
        ulong QueryChangeID([In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumCategories([In] uint Flags, [In] System.Deployment.Internal.Isolation.IReferenceIdentity ReferenceIdentity_ToMatch, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumSubcategories([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity CategoryId, [In, MarshalAs(UnmanagedType.LPWStr)] string SubcategoryPathPattern, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumCategoryInstances([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity CategoryId, [In, MarshalAs(UnmanagedType.LPWStr)] string SubcategoryPath, [In] ref Guid riid);
        void GetDeploymentProperty([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionAppId DeploymentInPackage, [In] ref System.Deployment.Internal.Isolation.StoreApplicationReference Reference, [In] ref Guid PropertySet, [In, MarshalAs(UnmanagedType.LPWStr)] string pcwszPropertyName, out System.Deployment.Internal.Isolation.BLOB blob);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LockApplicationPath([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionAppId ApId, out IntPtr Cookie);
        void ReleaseApplicationPath([In] IntPtr Cookie);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumPrivateFiles([In] uint Flags, [In] System.Deployment.Internal.Isolation.IDefinitionAppId Application, [In] System.Deployment.Internal.Isolation.IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumInstallerDeploymentMetadata([In] uint Flags, [In] ref System.Deployment.Internal.Isolation.StoreApplicationReference Reference, [In] System.Deployment.Internal.Isolation.IReferenceAppId Filter, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object EnumInstallerDeploymentMetadataProperties([In] uint Flags, [In] ref System.Deployment.Internal.Isolation.StoreApplicationReference Reference, [In] System.Deployment.Internal.Isolation.IDefinitionAppId Filter, [In] ref Guid riid);
    }
}

