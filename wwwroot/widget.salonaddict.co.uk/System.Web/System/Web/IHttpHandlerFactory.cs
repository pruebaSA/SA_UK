namespace System.Web
{
    using System;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IHttpHandlerFactory
    {
        IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated);
        void ReleaseHandler(IHttpHandler handler);
    }
}

