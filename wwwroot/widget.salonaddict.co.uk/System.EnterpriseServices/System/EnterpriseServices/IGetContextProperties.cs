namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [ComImport, Guid("51372AF4-CAE7-11CF-BE81-00AA00A2FA25"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IGetContextProperties
    {
        int Count { get; }
        object GetProperty([In, MarshalAs(UnmanagedType.BStr)] string name);
        void GetEnumerator(out IEnumerator pEnum);
    }
}

