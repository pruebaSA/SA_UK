namespace System.Web
{
    using System;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IHttpAsyncHandler : IHttpHandler
    {
        IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData);
        void EndProcessRequest(IAsyncResult result);
    }
}

