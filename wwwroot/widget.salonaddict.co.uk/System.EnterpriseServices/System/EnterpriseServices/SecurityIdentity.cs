namespace System.EnterpriseServices
{
    using System;

    public sealed class SecurityIdentity
    {
        private ISecurityIdentityColl _ex;

        private SecurityIdentity()
        {
        }

        internal SecurityIdentity(ISecurityIdentityColl ifc)
        {
            this._ex = ifc;
        }

        public string AccountName =>
            ((string) this._ex.GetItem("AccountName"));

        public AuthenticationOption AuthenticationLevel =>
            ((AuthenticationOption) this._ex.GetItem("AuthenticationLevel"));

        public int AuthenticationService =>
            ((int) this._ex.GetItem("AuthenticationService"));

        public ImpersonationLevelOption ImpersonationLevel =>
            ((ImpersonationLevelOption) this._ex.GetItem("ImpersonationLevel"));
    }
}

