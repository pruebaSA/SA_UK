namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5fa4f590-a416-4b22-ac79-7c3f0d31f303")]
    internal interface IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.StoreOperationMetadataProperty[] AppIds);
        void Skip([In] uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY Clone();
    }
}

