namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IStateManager
    {
        void LoadViewState(object state);
        object SaveViewState();
        void TrackViewState();

        bool IsTrackingViewState { get; }
    }
}

