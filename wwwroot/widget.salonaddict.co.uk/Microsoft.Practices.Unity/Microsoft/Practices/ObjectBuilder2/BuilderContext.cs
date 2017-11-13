namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class BuilderContext : IBuilderContext
    {
        private readonly IStrategyChain chain;
        private readonly ILifetimeContainer lifetime;
        private readonly NamedTypeBuildKey originalBuildKey;
        private readonly IPolicyList persistentPolicies;
        private readonly IPolicyList policies;
        private readonly IRecoveryStack recoveryStack;
        private readonly CompositeResolverOverride resolverOverrides;

        protected BuilderContext()
        {
            this.recoveryStack = new Microsoft.Practices.ObjectBuilder2.RecoveryStack();
            this.resolverOverrides = new CompositeResolverOverride();
        }

        public BuilderContext(IStrategyChain chain, ILifetimeContainer lifetime, IPolicyList policies, NamedTypeBuildKey originalBuildKey, object existing)
        {
            this.recoveryStack = new Microsoft.Practices.ObjectBuilder2.RecoveryStack();
            this.resolverOverrides = new CompositeResolverOverride();
            this.chain = chain;
            this.lifetime = lifetime;
            this.originalBuildKey = originalBuildKey;
            this.BuildKey = originalBuildKey;
            this.persistentPolicies = policies;
            this.policies = new PolicyList(this.persistentPolicies);
            this.Existing = existing;
        }

        public BuilderContext(IStrategyChain chain, ILifetimeContainer lifetime, IPolicyList persistentPolicies, IPolicyList transientPolicies, NamedTypeBuildKey buildKey, object existing)
        {
            this.recoveryStack = new Microsoft.Practices.ObjectBuilder2.RecoveryStack();
            this.resolverOverrides = new CompositeResolverOverride();
            this.chain = chain;
            this.lifetime = lifetime;
            this.persistentPolicies = persistentPolicies;
            this.policies = transientPolicies;
            this.originalBuildKey = buildKey;
            this.BuildKey = buildKey;
            this.Existing = existing;
        }

        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            this.resolverOverrides.AddRange(newOverrides);
        }

        public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType) => 
            this.resolverOverrides.GetResolver(this, dependencyType);

        public object NewBuildUp(NamedTypeBuildKey newBuildKey)
        {
            this.ChildContext = new BuilderContext(this.chain, this.lifetime, this.persistentPolicies, this.policies, newBuildKey, null);
            this.ChildContext.AddResolverOverrides(Sequence.Collect<CompositeResolverOverride>(new CompositeResolverOverride[] { this.resolverOverrides }));
            object obj2 = this.ChildContext.Strategies.ExecuteBuildUp(this.ChildContext);
            this.ChildContext = null;
            return obj2;
        }

        public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
        {
            this.ChildContext = new BuilderContext(this.chain, this.lifetime, this.persistentPolicies, this.policies, newBuildKey, null);
            this.ChildContext.AddResolverOverrides(Sequence.Collect<CompositeResolverOverride>(new CompositeResolverOverride[] { this.resolverOverrides }));
            childCustomizationBlock(this.ChildContext);
            object obj2 = this.ChildContext.Strategies.ExecuteBuildUp(this.ChildContext);
            this.ChildContext = null;
            return obj2;
        }

        public bool BuildComplete { get; set; }

        public NamedTypeBuildKey BuildKey { get; set; }

        public IBuilderContext ChildContext { get; private set; }

        public object CurrentOperation { get; set; }

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime =>
            this.lifetime;

        public NamedTypeBuildKey OriginalBuildKey =>
            this.originalBuildKey;

        public IPolicyList PersistentPolicies =>
            this.persistentPolicies;

        public IPolicyList Policies =>
            this.policies;

        public IRecoveryStack RecoveryStack =>
            this.recoveryStack;

        public IStrategyChain Strategies =>
            this.chain;
    }
}

