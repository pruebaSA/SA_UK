namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5ba7cb30-8508-4114-8c77-262fcda4fadb")]
    internal interface IEnumSTORE_CATEGORY_INSTANCE
    {
        uint Next([In] uint ulElements, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE[] rgInstances);
        void Skip([In] uint ulElements);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY_INSTANCE Clone();
    }
}

