namespace System.Web.ApplicationServices
{
    using System;
    using System.Configuration.Provider;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Web.Management;
    using System.Web.Resources;
    using System.Web.Security;

    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required), ServiceContract(Namespace="http://asp.net/ApplicationServices/v200"), ServiceBehavior(Namespace="http://asp.net/ApplicationServices/v200", InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RoleService
    {
        private static EventHandler<SelectingProviderEventArgs> _selectingProvider;
        private static object _selectingProviderEventHandlerLock = new object();

        public static  event EventHandler<SelectingProviderEventArgs> SelectingProvider
        {
            add
            {
                lock (_selectingProviderEventHandlerLock)
                {
                    _selectingProvider = (EventHandler<SelectingProviderEventArgs>) Delegate.Combine(_selectingProvider, value);
                }
            }
            remove
            {
                lock (_selectingProviderEventHandlerLock)
                {
                    _selectingProvider = (EventHandler<SelectingProviderEventArgs>) Delegate.Remove(_selectingProvider, value);
                }
            }
        }

        internal RoleService()
        {
        }

        private static void EnsureProviderEnabled()
        {
            if (!Roles.Enabled)
            {
                throw new ProviderException(AtlasWeb.RoleService_RolesFeatureNotEnabled);
            }
        }

        private RoleProvider GetRoleProvider(IPrincipal user)
        {
            string name = Roles.Provider.Name;
            SelectingProviderEventArgs e = new SelectingProviderEventArgs(user, name);
            this.OnSelectingProvider(e);
            name = e.ProviderName;
            RoleProvider provider = Roles.Providers[name];
            if (provider == null)
            {
                throw new ProviderException(AtlasWeb.RoleService_RoleProviderNotFound);
            }
            return provider;
        }

        [OperationContract]
        public string[] GetRolesForCurrentUser()
        {
            string[] rolesForUser;
            try
            {
                ApplicationServiceHelper.EnsureRoleServiceEnabled();
                EnsureProviderEnabled();
                IPrincipal currentUser = ApplicationServiceHelper.GetCurrentUser(HttpContext.Current);
                string userName = ApplicationServiceHelper.GetUserName(currentUser);
                rolesForUser = this.GetRoleProvider(currentUser).GetRolesForUser(userName);
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return rolesForUser;
        }

        [OperationContract]
        public bool IsCurrentUserInRole(string role)
        {
            bool flag;
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            try
            {
                ApplicationServiceHelper.EnsureRoleServiceEnabled();
                EnsureProviderEnabled();
                IPrincipal currentUser = ApplicationServiceHelper.GetCurrentUser(HttpContext.Current);
                string userName = ApplicationServiceHelper.GetUserName(currentUser);
                flag = this.GetRoleProvider(currentUser).IsUserInRole(userName, role);
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return flag;
        }

        private void LogException(Exception e)
        {
            new WebServiceErrorEvent(AtlasWeb.UnhandledExceptionEventLogMessage, this, e).Raise();
        }

        private void OnSelectingProvider(SelectingProviderEventArgs e)
        {
            EventHandler<SelectingProviderEventArgs> handler = _selectingProvider;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

