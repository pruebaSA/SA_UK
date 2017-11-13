namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DetailsViewModeEventArgs : CancelEventArgs
    {
        private bool _cancelingEdit;
        private DetailsViewMode _mode;

        public DetailsViewModeEventArgs(DetailsViewMode mode, bool cancelingEdit) : base(false)
        {
            this._mode = mode;
            this._cancelingEdit = cancelingEdit;
        }

        public bool CancelingEdit =>
            this._cancelingEdit;

        public DetailsViewMode NewMode
        {
            get => 
                this._mode;
            set
            {
                this._mode = value;
            }
        }
    }
}

