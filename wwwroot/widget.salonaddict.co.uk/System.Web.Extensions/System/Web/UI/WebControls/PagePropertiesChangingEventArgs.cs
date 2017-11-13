namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class PagePropertiesChangingEventArgs : EventArgs
    {
        private int _maximumRows;
        private int _startRowIndex;

        public PagePropertiesChangingEventArgs(int startRowIndex, int maximumRows)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
        }

        public int MaximumRows =>
            this._maximumRows;

        public int StartRowIndex =>
            this._startRowIndex;
    }
}

