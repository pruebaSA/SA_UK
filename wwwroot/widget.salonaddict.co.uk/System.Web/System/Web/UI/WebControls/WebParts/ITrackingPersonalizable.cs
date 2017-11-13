namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface ITrackingPersonalizable
    {
        void BeginLoad();
        void BeginSave();
        void EndLoad();
        void EndSave();

        bool TracksChanges { get; }
    }
}

