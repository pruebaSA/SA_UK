namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("b840a2f5-a497-4a6d-9038-cd3ec2fbd222")]
    internal interface IEnumSTORE_CATEGORY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.STORE_CATEGORY[] rgElements);
        void Skip([In] uint ulElements);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY Clone();
    }
}

