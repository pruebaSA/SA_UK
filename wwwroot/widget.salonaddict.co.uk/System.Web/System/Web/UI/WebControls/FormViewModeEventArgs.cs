namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class FormViewModeEventArgs : CancelEventArgs
    {
        private bool _cancelingEdit;
        private FormViewMode _mode;

        public FormViewModeEventArgs(FormViewMode mode, bool cancelingEdit) : base(false)
        {
            this._mode = mode;
            this._cancelingEdit = cancelingEdit;
        }

        public bool CancelingEdit =>
            this._cancelingEdit;

        public FormViewMode NewMode
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

