namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("75B52DDB-E8ED-11D1-93AD-00AA00BA3258"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IObjectContextInfo
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [PreserveSig]
        bool IsInTransaction();
        [return: MarshalAs(UnmanagedType.Interface)]
        [PreserveSig]
        object GetTransaction();
        void GetTransactionId(out Guid guid);
        void GetActivityId(out Guid guid);
        void GetContextId(out Guid guid);
    }
}

