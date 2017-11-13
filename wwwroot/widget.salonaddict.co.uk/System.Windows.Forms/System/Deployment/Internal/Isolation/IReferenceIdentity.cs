namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("6eaf5ace-7917-4f3c-b129-e046a9704766")]
    internal interface IReferenceIdentity
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAttribute([In, MarshalAs(UnmanagedType.LPWStr)] string Namespace, [In, MarshalAs(UnmanagedType.LPWStr)] string Name);
        void SetAttribute([In, MarshalAs(UnmanagedType.LPWStr)] string Namespace, [In, MarshalAs(UnmanagedType.LPWStr)] string Name, [In, MarshalAs(UnmanagedType.LPWStr)] string Value);
        System.Deployment.Internal.Isolation.IEnumIDENTITY_ATTRIBUTE EnumAttributes();
        System.Deployment.Internal.Isolation.IReferenceIdentity Clone([In] IntPtr cDeltas, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDENTITY_ATTRIBUTE[] Deltas);
    }
}

