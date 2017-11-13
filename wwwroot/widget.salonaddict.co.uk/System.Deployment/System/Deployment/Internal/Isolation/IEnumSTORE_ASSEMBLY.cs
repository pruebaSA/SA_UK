namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("a5c637bf-6eaa-4e5f-b535-55299657e33e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_ASSEMBLY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.STORE_ASSEMBLY[] rgelt);
        void Skip([In] uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_ASSEMBLY Clone();
    }
}

