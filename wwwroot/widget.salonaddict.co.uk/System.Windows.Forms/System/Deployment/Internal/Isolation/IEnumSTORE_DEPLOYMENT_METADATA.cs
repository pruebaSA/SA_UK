namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("f9fd4090-93db-45c0-af87-624940f19cff")]
    internal interface IEnumSTORE_DEPLOYMENT_METADATA
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionAppId[] AppIds);
        void Skip([In] uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_DEPLOYMENT_METADATA Clone();
    }
}

