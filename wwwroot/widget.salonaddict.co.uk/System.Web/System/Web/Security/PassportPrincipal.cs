namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PassportPrincipal : GenericPrincipal
    {
        public PassportPrincipal(PassportIdentity identity, string[] roles) : base(identity, roles)
        {
        }
    }
}

