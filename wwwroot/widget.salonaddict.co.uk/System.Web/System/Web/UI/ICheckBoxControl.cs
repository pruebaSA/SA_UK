namespace System.Web.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface ICheckBoxControl
    {
        event EventHandler CheckedChanged;

        bool Checked { get; set; }
    }
}

