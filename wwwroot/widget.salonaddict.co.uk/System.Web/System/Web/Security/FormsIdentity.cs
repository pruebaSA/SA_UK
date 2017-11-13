namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class FormsIdentity : IIdentity
    {
        private FormsAuthenticationTicket _Ticket;

        public FormsIdentity(FormsAuthenticationTicket ticket)
        {
            this._Ticket = ticket;
        }

        public string AuthenticationType =>
            "Forms";

        public bool IsAuthenticated =>
            true;

        public string Name =>
            this._Ticket.Name;

        public FormsAuthenticationTicket Ticket =>
            this._Ticket;
    }
}

