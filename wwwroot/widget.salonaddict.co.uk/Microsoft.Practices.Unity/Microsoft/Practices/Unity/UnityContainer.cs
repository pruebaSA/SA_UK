namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class UnityContainer : IUnityContainer, IDisposable
    {
        private StagedStrategyChain<UnityBuildStage> buildPlanStrategies;
        private IStrategyChain cachedStrategies;
        private object cachedStrategiesLock;
        private List<UnityContainerExtension> extensions;
        private LifetimeContainer lifetimeContainer;
        private readonly UnityContainer parent;
        private PolicyList policies;
        private NamedTypesRegistry registeredNames;
        private StagedStrategyChain<UnityBuildStage> strategies;

        private event EventHandler<ChildContainerCreatedEventArgs> childContainerCreated;

        private event EventHandler<RegisterEventArgs> registering;

        private event EventHandler<RegisterInstanceEventArgs> registeringInstance;

        public UnityContainer() : this(null)
        {
            this.AddExtension(new UnityDefaultStrategiesExtension());
        }

        private UnityContainer(UnityContainer parent)
        {
            this.parent = parent;
            if (parent != null)
            {
                parent.lifetimeContainer.Add(this);
            }
            this.InitializeBuilderState();
            this.registering += delegate {
            };
            this.registeringInstance += delegate {
            };
            this.childContainerCreated += delegate {
            };
            this.AddExtension(new UnityDefaultBehaviorExtension());
            this.AddExtension(new InjectedMembers());
        }

        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            this.extensions.Add(extension);
            extension.InitializeExtension(new ExtensionContextImpl(this));
            lock (this.cachedStrategiesLock)
            {
                this.cachedStrategies = null;
            }
            return this;
        }

        public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(existing, "existing");
            Guard.InstanceIsAssignable(t, existing, "existing");
            return this.DoBuildUp(t, existing, name, resolverOverrides);
        }

        private void ClearExistingBuildPlan(Type typeToInject, string name)
        {
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(typeToInject, name);
            DependencyResolverTrackerPolicy.RemoveResolvers(this.policies, buildKey);
            this.policies.Set<IBuildPlanPolicy>(new OverriddenBuildPlanMarkerPolicy(), buildKey);
        }

        public object Configure(Type configurationInterface) => 
            (from ex in this.extensions
                where configurationInterface.IsAssignableFrom(ex.GetType())
                select ex).FirstOrDefault<UnityContainerExtension>();

        public IUnityContainer CreateChildContainer()
        {
            UnityContainer container = new UnityContainer(this);
            ExtensionContextImpl childContext = new ExtensionContextImpl(container);
            this.childContainerCreated(this, new ChildContainerCreatedEventArgs(childContext));
            return container;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.lifetimeContainer != null)
                {
                    this.lifetimeContainer.Dispose();
                    this.lifetimeContainer = null;
                    if ((this.parent != null) && (this.parent.lifetimeContainer != null))
                    {
                        this.parent.lifetimeContainer.Remove(this);
                    }
                }
                this.extensions.OfType<IDisposable>().ForEach<IDisposable>(ex => ex.Dispose());
                this.extensions.Clear();
            }
        }

        private object DoBuildUp(Type t, string name, IEnumerable<ResolverOverride> resolverOverrides) => 
            this.DoBuildUp(t, null, name, resolverOverrides);

        private object DoBuildUp(Type t, object existing, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            IBuilderContext context = null;
            object obj2;
            try
            {
                context = new BuilderContext(this.GetStrategies(), this.lifetimeContainer, this.policies, new NamedTypeBuildKey(t, name), existing);
                context.AddResolverOverrides(resolverOverrides);
                if (t.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.CannotResolveOpenGenericType, new object[] { t.FullName }), "t");
                }
                obj2 = context.Strategies.ExecuteBuildUp(context);
            }
            catch (Exception exception)
            {
                throw new ResolutionFailedException(t, name, exception, context);
            }
            return obj2;
        }

        private void FillTypeRegistrationDictionary(IDictionary<Type, List<string>> typeRegistrations)
        {
            if (this.parent != null)
            {
                this.parent.FillTypeRegistrationDictionary(typeRegistrations);
            }
            foreach (Type type in this.registeredNames.RegisteredTypes)
            {
                if (!typeRegistrations.ContainsKey(type))
                {
                    typeRegistrations[type] = new List<string>();
                }
                typeRegistrations[type] = typeRegistrations[type].Concat<string>(this.registeredNames.GetKeys(type)).Distinct<string>().ToList<string>();
            }
        }

        private IEnumerable<string> GetRegisteredNames(Type t) => 
            (from s in this.registeredNames.GetKeys(t)
                where !string.IsNullOrEmpty(s)
                select s);

        private IStrategyChain GetStrategies()
        {
            IStrategyChain cachedStrategies = this.cachedStrategies;
            if (cachedStrategies != null)
            {
                return cachedStrategies;
            }
            lock (this.cachedStrategiesLock)
            {
                if (this.cachedStrategies == null)
                {
                    cachedStrategies = this.strategies.MakeStrategyChain();
                    this.cachedStrategies = cachedStrategies;
                    return cachedStrategies;
                }
                return this.cachedStrategies;
            }
        }

        private void InitializeBuilderState()
        {
            this.registeredNames = new NamedTypesRegistry(this.ParentNameRegistry);
            this.extensions = new List<UnityContainerExtension>();
            this.lifetimeContainer = new LifetimeContainer();
            this.strategies = new StagedStrategyChain<UnityBuildStage>(this.ParentStrategies);
            this.buildPlanStrategies = new StagedStrategyChain<UnityBuildStage>(this.ParentBuildPlanStrategies);
            this.policies = new PolicyList(this.ParentPolicies);
            this.cachedStrategies = null;
            this.cachedStrategiesLock = new object();
        }

        public IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            Guard.ArgumentNotNull(instance, "instance");
            Guard.ArgumentNotNull(lifetime, "lifetime");
            Guard.InstanceIsAssignable(t, instance, "instance");
            this.registeringInstance(this, new RegisterInstanceEventArgs(t, instance, name, lifetime));
            return this;
        }

        public IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(to, "to");
            if (string.IsNullOrEmpty(name))
            {
                name = null;
            }
            if (!(((from == null) || from.IsGenericType) || to.IsGenericType))
            {
                Guard.TypeIsAssignable(from, to, "from");
            }
            this.registering(this, new RegisterEventArgs(from, to, name, lifetimeManager));
            if (injectionMembers.Length > 0)
            {
                this.ClearExistingBuildPlan(to, name);
                foreach (InjectionMember member in injectionMembers)
                {
                    member.AddPolicies(from, to, name, this.policies);
                }
            }
            return this;
        }

        public IUnityContainer RemoveAllExtensions()
        {
            List<UnityContainerExtension> list = new List<UnityContainerExtension>(this.extensions);
            list.Reverse();
            foreach (UnityContainerExtension extension in list)
            {
                extension.Remove();
                IDisposable disposable = extension as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            this.extensions.Clear();
            this.strategies.Clear();
            this.policies.ClearAll();
            this.registeredNames.Clear();
            return this;
        }

        public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides) => 
            this.DoBuildUp(t, name, resolverOverrides);

        public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
        {
            IEnumerable<string> registeredNames = this.GetRegisteredNames(t);
            if (t.IsGenericType)
            {
                registeredNames = registeredNames.Concat<string>(this.GetRegisteredNames(t.GetGenericTypeDefinition()));
            }
            registeredNames = registeredNames.Distinct<string>();
            foreach (string iteratorVariable1 in registeredNames)
            {
                yield return this.Resolve(t, iteratorVariable1, resolverOverrides);
            }
        }

        public void Teardown(object o)
        {
            IBuilderContext context = null;
            try
            {
                Guard.ArgumentNotNull(o, "o");
                context = new BuilderContext(this.GetStrategies().Reverse(), this.lifetimeContainer, this.policies, null, o);
                context.Strategies.ExecuteTearDown(context);
            }
            catch (Exception exception)
            {
                throw new ResolutionFailedException(o.GetType(), null, exception, context);
            }
        }

        public IUnityContainer Parent =>
            this.parent;

        private StagedStrategyChain<UnityBuildStage> ParentBuildPlanStrategies =>
            ((this.parent == null) ? null : this.parent.buildPlanStrategies);

        private NamedTypesRegistry ParentNameRegistry =>
            ((this.parent == null) ? null : this.parent.registeredNames);

        private PolicyList ParentPolicies =>
            ((this.parent == null) ? null : this.parent.policies);

        private StagedStrategyChain<UnityBuildStage> ParentStrategies =>
            ((this.parent == null) ? null : this.parent.strategies);

        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                Dictionary<Type, List<string>> allRegisteredNames = new Dictionary<Type, List<string>>();
                this.FillTypeRegistrationDictionary(allRegisteredNames);
                return (from type in allRegisteredNames.Keys
                    from name in allRegisteredNames[type]
                    select new ContainerRegistration(type, name, this.policies));
            }
        }


        private class ExtensionContextImpl : ExtensionContext
        {
            private readonly UnityContainer container;

            public event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add
                {
                    this.container.childContainerCreated += value;
                }
                remove
                {
                    this.container.childContainerCreated -= value;
                }
            }

            public event EventHandler<RegisterEventArgs> Registering
            {
                add
                {
                    this.container.registering += value;
                }
                remove
                {
                    this.container.registering -= value;
                }
            }

            public event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add
                {
                    this.container.registeringInstance += value;
                }
                remove
                {
                    this.container.registeringInstance -= value;
                }
            }

            public ExtensionContextImpl(UnityContainer container)
            {
                this.container = container;
            }

            public override void RegisterNamedType(Type t, string name)
            {
                this.container.registeredNames.RegisterType(t, name);
            }

            public override StagedStrategyChain<UnityBuildStage> BuildPlanStrategies =>
                this.container.buildPlanStrategies;

            public override IUnityContainer Container =>
                this.container;

            public override ILifetimeContainer Lifetime =>
                this.container.lifetimeContainer;

            public override IPolicyList Policies =>
                this.container.policies;

            public override StagedStrategyChain<UnityBuildStage> Strategies =>
                this.container.strategies;
        }
    }
}

