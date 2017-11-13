namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("b30352cf-23da-4577-9b3f-b4e6573be53b")]
    internal interface IEnumReferenceIdentity
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IReferenceIdentity[] ReferenceIdentity);
        void Skip(uint celt);
        void Reset();
        System.Deployment.Internal.Isolation.IEnumReferenceIdentity Clone();
    }
}

