namespace Microsoft.Practices.Unity.StaticFactory
{
    using Microsoft.Practices.Unity;
    using System;

    public interface IStaticFactoryConfiguration : IUnityContainerExtensionConfigurator
    {
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(Func<IUnityContainer, object> factoryMethod);
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(string name, Func<IUnityContainer, object> factoryMethod);
    }
}

