namespace System.Web.Security
{
    using System;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Script.Services;
    using System.Web.Services;

    [ScriptService]
    internal sealed class AuthenticationService
    {
        [WebMethod]
        public bool IsLoggedIn()
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, false);
            return HttpContext.Current.Request.IsAuthenticated;
        }

        [WebMethod]
        public bool Login(string userName, string password, bool createPersistentCookie)
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, true);
            if (Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
                return true;
            }
            return false;
        }

        [WebMethod]
        public void Logout()
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, false);
            FormsAuthentication.SignOut();
        }
    }
}

