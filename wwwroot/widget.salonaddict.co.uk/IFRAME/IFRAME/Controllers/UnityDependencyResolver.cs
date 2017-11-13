namespace IFRAME.Controllers
{
    using Microsoft.Practices.Unity;
    using SA.BAL;
    using System;
    using System.Configuration;

    internal sealed class UnityDependencyResolver : BaseUnityDependencyResolver, IDependencyResolver
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SalonAddict"].ConnectionString;
            base.Container.RegisterType<ILogManager, AuditManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new NullCache() }) });
            base.Container.RegisterType<ILocationManager, LocationManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<IBillingManager, BillingManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new NullCache() }) });
            base.Container.RegisterType<IEmployeeManager, EmployeeManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<ITicketManager, TicketManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<ISchedulingManager, SchedulingManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<IReportManager, ReportManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<ISalonManager, SalonManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<ISecurityManager, SecurityManager>(new InjectionMember[0]);
            base.Container.RegisterType<IServiceManager, ServiceManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<IUserManager, UserManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new StaticCache() }) });
            base.Container.RegisterType<IMessageManager, MessageManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new NullCache() }) });
            base.Container.RegisterType<IMediaManager, MediaManagerSQL>(new TransientLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { connectionString, new NullCache() }) });
        }
    }
}

