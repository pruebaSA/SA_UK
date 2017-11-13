namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("d8b1aacb-5142-4abb-bcc1-e9dc9052a89e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_ASSEMBLY_INSTALLATION_REFERENCE
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.StoreApplicationReference[] rgelt);
        void Skip([In] uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_ASSEMBLY_INSTALLATION_REFERENCE Clone();
    }
}

