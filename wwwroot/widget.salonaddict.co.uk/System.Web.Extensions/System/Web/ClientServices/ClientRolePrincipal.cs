namespace System.Web.ClientServices
{
    using System;
    using System.Security.Principal;
    using System.Web.Security;

    public class ClientRolePrincipal : IPrincipal
    {
        private IIdentity _Identity;

        public ClientRolePrincipal(IIdentity identity)
        {
            this._Identity = identity;
        }

        public bool IsInRole(string role) => 
            Roles.IsUserInRole(this._Identity.Name, role);

        public IIdentity Identity =>
            this._Identity;
    }
}

