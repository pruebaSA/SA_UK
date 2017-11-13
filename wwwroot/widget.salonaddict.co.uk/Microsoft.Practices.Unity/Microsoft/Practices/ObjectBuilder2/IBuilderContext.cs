namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections.Generic;

    public interface IBuilderContext
    {
        void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides);
        IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType);
        object NewBuildUp(NamedTypeBuildKey newBuildKey);
        object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock);

        bool BuildComplete { get; set; }

        NamedTypeBuildKey BuildKey { get; set; }

        IBuilderContext ChildContext { get; }

        object CurrentOperation { get; set; }

        object Existing { get; set; }

        ILifetimeContainer Lifetime { get; }

        NamedTypeBuildKey OriginalBuildKey { get; }

        IPolicyList PersistentPolicies { get; }

        IPolicyList Policies { get; }

        IRecoveryStack RecoveryStack { get; }

        IStrategyChain Strategies { get; }
    }
}

