namespace System.Web.Management
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c84f668a-cc3f-11d7-b79e-505054503030"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IRegiisUtility
    {
        void ProtectedConfigAction(long actionToPerform, [In, MarshalAs(UnmanagedType.LPWStr)] string firstArgument, [In, MarshalAs(UnmanagedType.LPWStr)] string secondArgument, [In, MarshalAs(UnmanagedType.LPWStr)] string providerName, [In, MarshalAs(UnmanagedType.LPWStr)] string appPath, [In, MarshalAs(UnmanagedType.LPWStr)] string site, [In, MarshalAs(UnmanagedType.LPWStr)] string cspOrLocation, int keySize, out IntPtr exception);
        void RegisterSystemWebAssembly(int doReg, out IntPtr exception);
        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        void RegisterAsnetMmcAssembly(int doReg, [In, MarshalAs(UnmanagedType.LPWStr)] string assemblyName, [In, MarshalAs(UnmanagedType.LPWStr)] string binaryDirectory, out IntPtr exception);
        void RemoveBrowserCaps(out IntPtr exception);
    }
}

