namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("f3549d9c-fc73-4793-9c00-1cd204254c0c")]
    internal interface IEnumDefinitionIdentity
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionIdentity[] DefinitionIdentity);
        void Skip([In] uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumDefinitionIdentity Clone();
    }
}

