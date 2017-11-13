namespace System.Web.UI
{
    using System;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IPostBackDataHandler
    {
        bool LoadPostData(string postDataKey, NameValueCollection postCollection);
        void RaisePostDataChangedEvent();
    }
}

