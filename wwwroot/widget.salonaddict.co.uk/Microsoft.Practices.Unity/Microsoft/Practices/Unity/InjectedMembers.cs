namespace Microsoft.Practices.Unity
{
    using System;

    [Obsolete("Use the IUnityContainer.RegisterType method instead of this interface")]
    public class InjectedMembers : UnityContainerExtension
    {
        public InjectedMembers ConfigureInjectionFor<TTypeToInject>(params InjectionMember[] injectionMembers) => 
            this.ConfigureInjectionFor(typeof(TTypeToInject), null, injectionMembers);

        public InjectedMembers ConfigureInjectionFor<TTypeToInject>(string name, params InjectionMember[] injectionMembers) => 
            this.ConfigureInjectionFor(typeof(TTypeToInject), name, injectionMembers);

        public InjectedMembers ConfigureInjectionFor(Type typeToInject, params InjectionMember[] injectionMembers) => 
            this.ConfigureInjectionFor(null, typeToInject, null, injectionMembers);

        public InjectedMembers ConfigureInjectionFor(Type typeToInject, string name, params InjectionMember[] injectionMembers) => 
            this.ConfigureInjectionFor(null, typeToInject, name, injectionMembers);

        public InjectedMembers ConfigureInjectionFor(Type serviceType, Type implementationType, string name, params InjectionMember[] injectionMembers)
        {
            base.Container.RegisterType(serviceType, implementationType, name, injectionMembers);
            return this;
        }

        protected override void Initialize()
        {
        }
    }
}

