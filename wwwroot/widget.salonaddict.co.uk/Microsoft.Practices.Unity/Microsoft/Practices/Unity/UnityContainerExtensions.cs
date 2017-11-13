namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class UnityContainerExtensions
    {
        public static IUnityContainer AddNewExtension<TExtension>(this IUnityContainer container) where TExtension: UnityContainerExtension
        {
            Guard.ArgumentNotNull(container, "container");
            TExtension extension = container.Resolve<TExtension>(new ResolverOverride[0]);
            return container.AddExtension(extension);
        }

        public static T BuildUp<T>(this IUnityContainer container, T existing, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T) container.BuildUp(typeof(T), existing, null, resolverOverrides);
        }

        public static object BuildUp(this IUnityContainer container, Type t, object existing, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.BuildUp(t, existing, null, resolverOverrides);
        }

        public static T BuildUp<T>(this IUnityContainer container, T existing, string name, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T) container.BuildUp(typeof(T), existing, name, resolverOverrides);
        }

        public static TConfigurator Configure<TConfigurator>(this IUnityContainer container) where TConfigurator: IUnityContainerExtensionConfigurator
        {
            Guard.ArgumentNotNull(container, "container");
            return (TConfigurator) container.Configure(typeof(TConfigurator));
        }

        private static LifetimeManager CreateDefaultInstanceLifetimeManager() => 
            new ContainerControlledLifetimeManager();

        public static bool IsRegistered<T>(this IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.IsRegistered(typeof(T));
        }

        public static bool IsRegistered<T>(this IUnityContainer container, string nameToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.IsRegistered(typeof(T), nameToCheck);
        }

        public static bool IsRegistered(this IUnityContainer container, Type typeToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            Guard.ArgumentNotNull(typeToCheck, "typeToCheck");
            return container.IsRegistered(typeToCheck, null);
        }

        public static bool IsRegistered(this IUnityContainer container, Type typeToCheck, string nameToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            Guard.ArgumentNotNull(typeToCheck, "typeToCheck");
            return ((from r in container.Registrations
                where (r.RegisteredType == typeToCheck) && (r.Name == nameToCheck)
                select r).FirstOrDefault<ContainerRegistration>() != null);
        }

        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), null, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), null, instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), name, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, object instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, null, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), name, instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, object instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, null, instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, string name, object instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, name, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static IUnityContainer RegisterType<T>(this IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), null, null, injectionMembers);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, params InjectionMember[] injectionMembers) where TTo: TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, null, injectionMembers);
        }

        public static IUnityContainer RegisterType<T>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), null, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo: TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), name, null, injectionMembers);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers) where TTo: TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, null, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, null, null, injectionMembers);
        }

        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo: TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, null, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, name, null, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, null, null, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, null, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, name, null, injectionMembers);
        }

        public static T Resolve<T>(this IUnityContainer container, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T) container.Resolve(typeof(T), null, overrides);
        }

        public static T Resolve<T>(this IUnityContainer container, string name, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T) container.Resolve(typeof(T), name, overrides);
        }

        public static object Resolve(this IUnityContainer container, Type t, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.Resolve(t, null, overrides);
        }

        public static IEnumerable<T> ResolveAll<T>(this IUnityContainer container, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.ResolveAll(typeof(T), resolverOverrides).Cast<T>();
        }
    }
}

