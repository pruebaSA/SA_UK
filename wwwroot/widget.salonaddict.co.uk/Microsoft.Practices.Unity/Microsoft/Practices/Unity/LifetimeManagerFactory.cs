namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Runtime.CompilerServices;

    public class LifetimeManagerFactory : ILifetimeFactoryPolicy, IBuilderPolicy
    {
        private readonly ExtensionContext containerContext;

        public LifetimeManagerFactory(ExtensionContext containerContext, Type lifetimeType)
        {
            this.containerContext = containerContext;
            this.LifetimeType = lifetimeType;
        }

        public ILifetimePolicy CreateLifetimePolicy()
        {
            LifetimeManager item = (LifetimeManager) this.containerContext.Container.Resolve(this.LifetimeType, new ResolverOverride[0]);
            if (item is IDisposable)
            {
                this.containerContext.Lifetime.Add(item);
            }
            item.InUse = true;
            return item;
        }

        public Type LifetimeType { get; private set; }
    }
}

