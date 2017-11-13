namespace Microsoft.Practices.Unity
{
    using System;
    using System.Collections.Generic;

    public interface IUnityContainer : IDisposable
    {
        IUnityContainer AddExtension(UnityContainerExtension extension);
        object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides);
        object Configure(Type configurationInterface);
        IUnityContainer CreateChildContainer();
        IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime);
        IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers);
        IUnityContainer RemoveAllExtensions();
        object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides);
        IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides);
        void Teardown(object o);

        IUnityContainer Parent { get; }

        IEnumerable<ContainerRegistration> Registrations { get; }
    }
}

