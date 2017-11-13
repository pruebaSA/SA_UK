namespace SA.BAL
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class BaseUnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public BaseUnityDependencyResolver() : this(new UnityContainer())
        {
        }

        public BaseUnityDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException($"BaseUnityDependencyResolver(IUnityContainer container): {"container"}");
            }
            this._container = container;
            this.ConfigureContainer(this._container);
        }

        protected abstract void ConfigureContainer(IUnityContainer container);
        public virtual void Inject<T>(T existing)
        {
            if (existing == null)
            {
                throw new ArgumentNullException($"Inject<T>(T existing): {"existing"}");
            }
            this._container.BuildUp<T>(existing, new ResolverOverride[0]);
        }

        public virtual void Register<T>(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException($"Register<T>(T instance): {"instance"}");
            }
            this._container.RegisterInstance<T>(instance);
        }

        public virtual T Resolve<T>() => 
            this._container.Resolve<T>(new ResolverOverride[0]);

        public virtual T Resolve<T>(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException($"Resolve<T>(string name): {"name"}");
            }
            return this._container.Resolve<T>(name, new ResolverOverride[0]);
        }

        public virtual T Resolve<T>(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type): {"type"}");
            }
            return (T) this._container.Resolve(type, new ResolverOverride[0]);
        }

        public virtual T Resolve<T>(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type, string name): {"type"}");
            }
            if (name == null)
            {
                throw new ArgumentNullException($"Resolve<T>(Type type, string name): {"name"}");
            }
            return (T) this._container.Resolve(type, name, new ResolverOverride[0]);
        }

        public virtual IEnumerable<T> ResolveAll<T>()
        {
            IEnumerable<T> collection = this._container.ResolveAll<T>(new ResolverOverride[0]);
            T objA = default(T);
            try
            {
                objA = this._container.Resolve<T>(new ResolverOverride[0]);
            }
            catch (ResolutionFailedException)
            {
            }
            if (object.Equals(objA, default(T)))
            {
                return collection;
            }
            return new ReadOnlyCollection<T>(new List<T>(collection) { objA });
        }

        protected IUnityContainer Container =>
            this._container;
    }
}

