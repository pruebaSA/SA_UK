namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IVersioningPersonalizable
    {
        void Load(IDictionary unknownProperties);
    }
}

