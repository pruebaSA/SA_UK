namespace System.Web.ApplicationServices
{
    using System;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ApplicationServicesHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            if (typeof(ProfileService).Equals(serviceType))
            {
                return new ServiceHost(new ProfileService(), baseAddresses);
            }
            if (typeof(RoleService).Equals(serviceType))
            {
                return new ServiceHost(new RoleService(), baseAddresses);
            }
            if (typeof(AuthenticationService).Equals(serviceType))
            {
                return new ServiceHost(new AuthenticationService(), baseAddresses);
            }
            return base.CreateServiceHost(serviceType, baseAddresses);
        }
    }
}

