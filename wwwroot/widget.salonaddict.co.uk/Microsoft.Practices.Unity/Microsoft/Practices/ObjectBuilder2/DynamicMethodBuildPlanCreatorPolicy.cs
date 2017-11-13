namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class DynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy, IBuilderPolicy
    {
        private IStagedStrategyChain strategies;

        public DynamicMethodBuildPlanCreatorPolicy(IStagedStrategyChain strategies)
        {
            this.strategies = strategies;
        }

        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            IDynamicBuilderMethodCreatorPolicy builderMethodCreator = context.Policies.Get<IDynamicBuilderMethodCreatorPolicy>(context.BuildKey);
            DynamicBuildPlanGenerationContext ilContext = new DynamicBuildPlanGenerationContext(buildKey.Type, builderMethodCreator);
            IBuilderContext context3 = this.GetContext(context, buildKey, ilContext);
            context3.Strategies.ExecuteBuildUp(context3);
            return new DynamicMethodBuildPlan(ilContext.GetBuildMethod());
        }

        private IBuilderContext GetContext(IBuilderContext originalContext, NamedTypeBuildKey buildKey, DynamicBuildPlanGenerationContext ilContext) => 
            new BuilderContext(this.strategies.MakeStrategyChain(), originalContext.Lifetime, originalContext.PersistentPolicies, originalContext.Policies, buildKey, ilContext);
    }
}

