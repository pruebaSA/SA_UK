namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class SecurityCallContext
    {
        private ISecurityCallContext _ex;

        private SecurityCallContext()
        {
        }

        private SecurityCallContext(ISecurityCallContext ctx)
        {
            this._ex = ctx;
        }

        public bool IsCallerInRole(string role) => 
            this._ex.IsCallerInRole(role);

        public bool IsUserInRole(string user, string role)
        {
            object pUser = user;
            return this._ex.IsUserInRole(ref pUser, role);
        }

        public SecurityCallers Callers =>
            new SecurityCallers((ISecurityCallersColl) this._ex.GetItem("Callers"));

        public static SecurityCallContext CurrentCall
        {
            get
            {
                SecurityCallContext context2;
                Platform.Assert(Platform.W2K, "SecurityCallContext");
                try
                {
                    ISecurityCallContext context;
                    Util.CoGetCallContext(Util.IID_ISecurityCallContext, out context);
                    context2 = new SecurityCallContext(context);
                }
                catch (InvalidCastException)
                {
                    throw new COMException(Resource.FormatString("Err_NoSecurityContext"), Util.E_NOINTERFACE);
                }
                return context2;
            }
        }

        public SecurityIdentity DirectCaller =>
            new SecurityIdentity((ISecurityIdentityColl) this._ex.GetItem("DirectCaller"));

        public bool IsSecurityEnabled =>
            this._ex.IsSecurityEnabled();

        public int MinAuthenticationLevel =>
            ((int) this._ex.GetItem("MinAuthenticationLevel"));

        public int NumCallers =>
            ((int) this._ex.GetItem("NumCallers"));

        public SecurityIdentity OriginalCaller =>
            new SecurityIdentity((ISecurityIdentityColl) this._ex.GetItem("OriginalCaller"));
    }
}

