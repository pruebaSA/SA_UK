namespace System.EnterpriseServices.Admin
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [ComImport, SuppressUnmanagedCodeSecurity, Guid("6EB22870-8A19-11D0-81B6-00A0C9231C29")]
    internal interface IMtsCatalog
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [DispId(1)]
        object GetCollection([In, MarshalAs(UnmanagedType.BStr)] string bstrCollName);
        [return: MarshalAs(UnmanagedType.Interface)]
        [DispId(2)]
        object Connect([In, MarshalAs(UnmanagedType.BStr)] string connectStr);
        [DispId(3)]
        int MajorVersion();
        [DispId(4)]
        int MinorVersion();
    }
}

