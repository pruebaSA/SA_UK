namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public abstract class ExtensionContext
    {
        public abstract event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        public abstract event EventHandler<RegisterEventArgs> Registering;

        public abstract event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

        protected ExtensionContext()
        {
        }

        public abstract void RegisterNamedType(Type t, string name);

        public abstract StagedStrategyChain<UnityBuildStage> BuildPlanStrategies { get; }

        public abstract IUnityContainer Container { get; }

        public abstract ILifetimeContainer Lifetime { get; }

        public abstract IPolicyList Policies { get; }

        public abstract StagedStrategyChain<UnityBuildStage> Strategies { get; }
    }
}

