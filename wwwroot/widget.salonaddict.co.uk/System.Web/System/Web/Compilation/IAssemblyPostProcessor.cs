namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.High), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.High)]
    public interface IAssemblyPostProcessor : IDisposable
    {
        void PostProcessAssembly(string path);
    }
}

