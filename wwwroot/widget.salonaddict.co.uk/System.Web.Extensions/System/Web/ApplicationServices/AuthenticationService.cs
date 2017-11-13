namespace System.Web.ApplicationServices
{
    using System;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Web.Management;
    using System.Web.Resources;
    using System.Web.Security;

    [ServiceContract(Namespace="http://asp.net/ApplicationServices/v200"), AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required), ServiceBehavior(Namespace="http://asp.net/ApplicationServices/v200", InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AuthenticationService
    {
        private static EventHandler<AuthenticatingEventArgs> _authenticating;
        private static object _authenticatingEventHandlerLock = new object();
        private static EventHandler<CreatingCookieEventArgs> _creatingCookie;
        private static object _creatingCookieEventHandlerLock = new object();

        public static  event EventHandler<AuthenticatingEventArgs> Authenticating
        {
            add
            {
                lock (_authenticatingEventHandlerLock)
                {
                    _authenticating = (EventHandler<AuthenticatingEventArgs>) Delegate.Combine(_authenticating, value);
                }
            }
            remove
            {
                lock (_authenticatingEventHandlerLock)
                {
                    _authenticating = (EventHandler<AuthenticatingEventArgs>) Delegate.Remove(_authenticating, value);
                }
            }
        }

        public static  event EventHandler<CreatingCookieEventArgs> CreatingCookie
        {
            add
            {
                lock (_creatingCookieEventHandlerLock)
                {
                    _creatingCookie = (EventHandler<CreatingCookieEventArgs>) Delegate.Combine(_creatingCookie, value);
                }
            }
            remove
            {
                lock (_creatingCookieEventHandlerLock)
                {
                    _creatingCookie = (EventHandler<CreatingCookieEventArgs>) Delegate.Remove(_creatingCookie, value);
                }
            }
        }

        internal AuthenticationService()
        {
        }

        [OperationContract]
        public bool IsLoggedIn()
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, true);
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        private void LogException(Exception e)
        {
            new WebServiceErrorEvent(AtlasWeb.UnhandledExceptionEventLogMessage, this, e).Raise();
        }

        [OperationContract]
        public bool Login(string username, string password, string customCredential, bool isPersistent)
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, true);
            return this.LoginInternal(username, password, customCredential, isPersistent, true);
        }

        private bool LoginInternal(string username, string password, string customCredential, bool isPersistent, bool setCookie)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            AuthenticatingEventArgs e = new AuthenticatingEventArgs(username, password, customCredential);
            try
            {
                this.OnAuthenticating(e);
                if (!e.AuthenticationIsComplete)
                {
                    MembershipValidate(e);
                }
                if (!e.Authenticated)
                {
                    this.Logout();
                }
                if (e.Authenticated && setCookie)
                {
                    CreatingCookieEventArgs args2 = new CreatingCookieEventArgs(username, password, isPersistent, customCredential);
                    this.OnCreatingCookie(args2);
                    if (!args2.CookieIsSet)
                    {
                        SetCookie(username, isPersistent);
                    }
                }
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return e.Authenticated;
        }

        [OperationContract]
        public void Logout()
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, false);
            FormsAuthentication.SignOut();
        }

        private static void MembershipValidate(AuthenticatingEventArgs e)
        {
            e.Authenticated = Membership.ValidateUser(e.UserName, e.Password);
        }

        private void OnAuthenticating(AuthenticatingEventArgs e)
        {
            EventHandler<AuthenticatingEventArgs> handler = _authenticating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnCreatingCookie(CreatingCookieEventArgs e)
        {
            EventHandler<CreatingCookieEventArgs> handler = _creatingCookie;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static void SetCookie(string username, bool isPersistent)
        {
            FormsAuthentication.SetAuthCookie(username, isPersistent);
        }

        [OperationContract]
        public bool ValidateUser(string username, string password, string customCredential)
        {
            ApplicationServiceHelper.EnsureAuthenticationServiceEnabled(HttpContext.Current, true);
            return this.LoginInternal(username, password, customCredential, false, false);
        }
    }
}

