namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public static class IoC
    {
        private static IDependencyResolver _resolver;

        public static void InitializeWith(IDependencyResolverFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException($"InitializeWith(IDependencyResolverFactory factory): {"language"}");
            }
            _resolver = factory.CreateInstance();
        }

        public static void Inject<T>(T existing)
        {
            if (existing == null)
            {
                throw new ArgumentNullException($"Inject<T>(T existing): {"existing"}");
            }
            _resolver.Inject<T>(existing);
        }

        public static void Register<T>(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException($"Register<T>(T instance): {"instance"}");
            }
            _resolver.Register<T>(instance);
        }

        public static T Resolve<T>() => 
            _resolver.Resolve<T>();

        public static T Resolve<T>(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException($"Resolve<T>(string name): {"name"}");
            }
            return _resolver.Resolve<T>(name);
        }

        public static T Resolve<T>(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type): {"type"}");
            }
            return _resolver.Resolve<T>(type);
        }

        public static T Resolve<T>(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type, string name): {"type"}");
            }
            if (name == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type, string name): {"type"}");
            }
            return _resolver.Resolve<T>(type, name);
        }

        public static IEnumerable<T> ResolveAll<T>() => 
            _resolver.ResolveAll<T>();
    }
}

