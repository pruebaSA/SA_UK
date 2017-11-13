namespace System.Web.Security
{
    using System;
    using System.Configuration.Provider;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class RoleProvider : ProviderBase
    {
        protected RoleProvider()
        {
        }

        public abstract void AddUsersToRoles(string[] usernames, string[] roleNames);
        public abstract void CreateRole(string roleName);
        public abstract bool DeleteRole(string roleName, bool throwOnPopulatedRole);
        public abstract string[] FindUsersInRole(string roleName, string usernameToMatch);
        public abstract string[] GetAllRoles();
        public abstract string[] GetRolesForUser(string username);
        public abstract string[] GetUsersInRole(string roleName);
        public abstract bool IsUserInRole(string username, string roleName);
        public abstract void RemoveUsersFromRoles(string[] usernames, string[] roleNames);
        public abstract bool RoleExists(string roleName);

        public abstract string ApplicationName { get; set; }
    }
}

