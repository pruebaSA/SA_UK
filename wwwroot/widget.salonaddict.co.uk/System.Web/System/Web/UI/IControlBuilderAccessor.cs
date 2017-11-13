﻿namespace System.Web.UI
{
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IControlBuilderAccessor
    {
        System.Web.UI.ControlBuilder ControlBuilder { get; }
    }
}
