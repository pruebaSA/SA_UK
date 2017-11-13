namespace System.Web.UI.WebControls
{
    using System;
    using System.Net.Mail;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MailMessageEventArgs : LoginCancelEventArgs
    {
        private MailMessage _message;

        public MailMessageEventArgs(MailMessage message)
        {
            this._message = message;
        }

        public MailMessage Message =>
            this._message;
    }
}

