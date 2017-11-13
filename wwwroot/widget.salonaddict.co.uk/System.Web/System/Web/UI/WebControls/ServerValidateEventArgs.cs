namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ServerValidateEventArgs : EventArgs
    {
        private bool isValid;
        private string value;

        public ServerValidateEventArgs(string value, bool isValid)
        {
            this.isValid = isValid;
            this.value = value;
        }

        public bool IsValid
        {
            get => 
                this.isValid;
            set
            {
                this.isValid = value;
            }
        }

        public string Value =>
            this.value;
    }
}

