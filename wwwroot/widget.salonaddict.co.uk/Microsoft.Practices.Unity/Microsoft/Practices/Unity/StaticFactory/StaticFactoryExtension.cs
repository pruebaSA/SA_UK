namespace Microsoft.Practices.Unity.StaticFactory
{
    using Microsoft.Practices.Unity;
    using System;

    [Obsolete("Use RegisterType<TInterface, TImpl>(new InjectionFactory(...)) instead of the extension's methods.")]
    public class StaticFactoryExtension : UnityContainerExtension, IStaticFactoryConfiguration, IUnityContainerExtensionConfigurator
    {
        protected override void Initialize()
        {
        }

        public IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(Func<IUnityContainer, object> factoryMethod) => 
            this.RegisterFactory<TTypeToBuild>(null, factoryMethod);

        public IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(string name, Func<IUnityContainer, object> factoryMethod)
        {
            base.Container.RegisterType<TTypeToBuild>(name, new InjectionMember[] { new InjectionFactory(factoryMethod) });
            return this;
        }
    }
}

