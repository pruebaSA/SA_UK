namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("F79648FB-558B-4a09-88F1-1E3BCB30E34F"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IAppDomainInfoEnum
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        IAppDomainInfo GetData();
        [return: MarshalAs(UnmanagedType.I4)]
        int Count();
        [return: MarshalAs(UnmanagedType.Bool)]
        bool MoveNext();
        void Reset();
    }
}

