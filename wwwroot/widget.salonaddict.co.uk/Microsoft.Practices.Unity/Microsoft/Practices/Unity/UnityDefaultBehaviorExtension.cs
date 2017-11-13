namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using System;

    public class UnityDefaultBehaviorExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            base.Context.Registering += new EventHandler<RegisterEventArgs>(this.OnRegister);
            base.Context.RegisteringInstance += new EventHandler<RegisterInstanceEventArgs>(this.OnRegisterInstance);
            base.Container.RegisterInstance<IUnityContainer>(base.Container, new ContainerLifetimeManager());
        }

        private void OnRegister(object sender, RegisterEventArgs e)
        {
            base.Context.RegisterNamedType(e.TypeFrom ?? e.TypeTo, e.Name);
            if (e.TypeFrom != null)
            {
                if (e.TypeFrom.IsGenericTypeDefinition && e.TypeTo.IsGenericTypeDefinition)
                {
                    base.Context.Policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)), new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
                else
                {
                    base.Context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)), new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
            }
            if (e.LifetimeManager != null)
            {
                this.SetLifetimeManager(e.TypeTo, e.Name, e.LifetimeManager);
            }
        }

        private void OnRegisterInstance(object sender, RegisterInstanceEventArgs e)
        {
            base.Context.RegisterNamedType(e.RegisteredType, e.Name);
            this.SetLifetimeManager(e.RegisteredType, e.Name, e.LifetimeManager);
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(e.RegisteredType, e.Name);
            base.Context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(buildKey), buildKey);
            e.LifetimeManager.SetValue(e.Instance);
        }

        public override void Remove()
        {
            base.Context.Registering -= new EventHandler<RegisterEventArgs>(this.OnRegister);
            base.Context.RegisteringInstance -= new EventHandler<RegisterInstanceEventArgs>(this.OnRegisterInstance);
        }

        private void SetLifetimeManager(Type lifetimeType, string name, LifetimeManager lifetimeManager)
        {
            if (lifetimeManager.InUse)
            {
                throw new InvalidOperationException(Resources.LifetimeManagerInUse);
            }
            if (lifetimeType.IsGenericTypeDefinition)
            {
                LifetimeManagerFactory policy = new LifetimeManagerFactory(base.Context, lifetimeManager.GetType());
                base.Context.Policies.Set<ILifetimeFactoryPolicy>(policy, new NamedTypeBuildKey(lifetimeType, name));
            }
            else
            {
                lifetimeManager.InUse = true;
                base.Context.Policies.Set<ILifetimePolicy>(lifetimeManager, new NamedTypeBuildKey(lifetimeType, name));
                if (lifetimeManager is IDisposable)
                {
                    base.Context.Lifetime.Add(lifetimeManager);
                }
            }
        }

        private class ContainerLifetimeManager : LifetimeManager
        {
            private object value;

            public override object GetValue() => 
                this.value;

            public override void RemoveValue()
            {
            }

            public override void SetValue(object newValue)
            {
                this.value = newValue;
            }
        }
    }
}

