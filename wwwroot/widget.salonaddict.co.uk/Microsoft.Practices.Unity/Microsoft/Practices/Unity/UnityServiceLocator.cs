namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.Collections.Generic;

    public class UnityServiceLocator : ServiceLocatorImplBase, IDisposable
    {
        private IUnityContainer container;

        public UnityServiceLocator(IUnityContainer container)
        {
            this.container = container;
            container.RegisterInstance<IServiceLocator>(this, new ExternallyControlledLifetimeManager());
        }

        public void Dispose()
        {
            if (this.container != null)
            {
                this.container.Dispose();
                this.container = null;
            }
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) => 
            this.container?.ResolveAll(serviceType, new ResolverOverride[0]);

        protected override object DoGetInstance(Type serviceType, string key) => 
            this.container?.Resolve(serviceType, key, new ResolverOverride[0]);
    }
}

