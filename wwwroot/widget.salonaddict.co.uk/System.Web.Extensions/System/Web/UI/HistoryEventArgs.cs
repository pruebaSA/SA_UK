namespace System.Web.UI
{
    using System;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HistoryEventArgs : EventArgs
    {
        private NameValueCollection _state;

        public HistoryEventArgs(NameValueCollection state)
        {
            this._state = state;
        }

        public NameValueCollection State =>
            this._state;
    }
}

