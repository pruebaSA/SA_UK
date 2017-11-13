namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using System;

    public class UnityDefaultStrategiesExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            base.Context.Strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);
            base.Context.Strategies.AddNew<HierarchicalLifetimeStrategy>(UnityBuildStage.Lifetime);
            base.Context.Strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);
            base.Context.Strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            base.Context.Strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);
            base.Context.BuildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(UnityBuildStage.Creation);
            base.Context.BuildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(UnityBuildStage.Initialization);
            base.Context.BuildPlanStrategies.AddNew<DynamicMethodCallStrategy>(UnityBuildStage.Initialization);
            base.Context.Policies.SetDefault<IConstructorSelectorPolicy>(new DefaultUnityConstructorSelectorPolicy());
            base.Context.Policies.SetDefault<IPropertySelectorPolicy>(new DefaultUnityPropertySelectorPolicy());
            base.Context.Policies.SetDefault<IMethodSelectorPolicy>(new DefaultUnityMethodSelectorPolicy());
            this.SetDynamicBuilderMethodCreatorPolicy();
            base.Context.Policies.SetDefault<IBuildPlanCreatorPolicy>(new DynamicMethodBuildPlanCreatorPolicy(base.Context.BuildPlanStrategies));
            base.Context.Policies.Set<IBuildPlanPolicy>(new DeferredResolveBuildPlanPolicy(), typeof(Func<>));
            base.Context.Policies.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), typeof(Func<>));
        }

        protected void SetDynamicBuilderMethodCreatorPolicy()
        {
            base.Context.Policies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(new DefaultDynamicBuilderMethodCreatorPolicy());
        }
    }
}

