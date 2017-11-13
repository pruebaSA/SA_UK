namespace System.Deployment.Application
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FRestricted), Guid("809c652e-7396-11d2-9771-00a0c9b4d50c")]
    internal interface IMetaDataDispenser
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object DefineScope([In] ref Guid rclsid, [In] uint dwCreateFlags, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.Interface)]
        object OpenScope([In, MarshalAs(UnmanagedType.LPWStr)] string szScope, [In] uint dwOpenFlags, [In] ref Guid riid);
        [return: MarshalAs(UnmanagedType.Interface)]
        object OpenScopeOnMemory([In] IntPtr pData, [In] uint cbData, [In] uint dwOpenFlags, [In] ref Guid riid);
    }
}

