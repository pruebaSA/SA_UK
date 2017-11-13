namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Runtime.CompilerServices;

    public class ContainerRegistration
    {
        private readonly NamedTypeBuildKey buildKey;

        internal ContainerRegistration(Type registeredType, string name, IPolicyList policies)
        {
            this.buildKey = new NamedTypeBuildKey(registeredType, name);
            this.MappedToType = this.GetMappedType(policies);
            this.LifetimeManagerType = this.GetLifetimeManagerType(policies);
            this.LifetimeManager = this.GetLifetimeManager(policies);
        }

        private Microsoft.Practices.Unity.LifetimeManager GetLifetimeManager(IPolicyList policies)
        {
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(this.MappedToType, this.Name);
            return (Microsoft.Practices.Unity.LifetimeManager) policies.Get<ILifetimePolicy>(buildKey);
        }

        private Type GetLifetimeManagerType(IPolicyList policies)
        {
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(this.MappedToType, this.Name);
            ILifetimePolicy policy = policies.Get<ILifetimePolicy>(buildKey);
            if (policy != null)
            {
                return policy.GetType();
            }
            if (this.MappedToType.IsGenericType)
            {
                NamedTypeBuildKey key2 = new NamedTypeBuildKey(this.MappedToType.GetGenericTypeDefinition(), this.Name);
                ILifetimeFactoryPolicy policy2 = policies.Get<ILifetimeFactoryPolicy>(key2);
                if (policy2 != null)
                {
                    return policy2.LifetimeType;
                }
            }
            return typeof(TransientLifetimeManager);
        }

        private Type GetMappedType(IPolicyList policies)
        {
            IBuildKeyMappingPolicy policy = policies.Get<IBuildKeyMappingPolicy>(this.buildKey);
            if (policy != null)
            {
                return policy.Map(this.buildKey, null).Type;
            }
            return this.buildKey.Type;
        }

        public Microsoft.Practices.Unity.LifetimeManager LifetimeManager { get; private set; }

        public Type LifetimeManagerType { get; private set; }

        public Type MappedToType { get; private set; }

        public string Name =>
            this.buildKey.Name;

        public Type RegisteredType =>
            this.buildKey.Type;
    }
}

