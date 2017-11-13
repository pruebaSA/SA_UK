namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Security;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CreateUserErrorEventArgs : EventArgs
    {
        private MembershipCreateStatus _error;

        public CreateUserErrorEventArgs(MembershipCreateStatus s)
        {
            this._error = s;
        }

        public MembershipCreateStatus CreateUserError
        {
            get => 
                this._error;
            set
            {
                this._error = value;
            }
        }
    }
}

