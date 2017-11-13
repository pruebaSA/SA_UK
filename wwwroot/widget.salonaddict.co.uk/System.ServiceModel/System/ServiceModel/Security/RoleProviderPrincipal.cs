namespace System.ServiceModel.Security
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Web.Security;

    internal sealed class RoleProviderPrincipal : IPrincipal
    {
        private static object defaultRoleProvider;
        private static bool defaultRoleProviderSet = false;
        private object roleProvider;
        private ServiceSecurityContext securityContext;

        public RoleProviderPrincipal(object roleProvider, ServiceSecurityContext securityContext)
        {
            this.roleProvider = roleProvider;
            this.securityContext = securityContext;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private object GetRoleProvider()
        {
            if (!defaultRoleProviderSet)
            {
                defaultRoleProvider = Roles.Enabled ? Roles.Provider : null;
                defaultRoleProviderSet = true;
            }
            return defaultRoleProvider;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool IsInRole(string role)
        {
            object obj2 = this.roleProvider ?? this.GetRoleProvider();
            RoleProvider provider = obj2 as RoleProvider;
            return ((provider != null) && provider.IsUserInRole(this.securityContext.PrimaryIdentity.Name, role));
        }

        public IIdentity Identity =>
            this.securityContext.PrimaryIdentity;
    }
}

