namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            IBuildKeyMappingPolicy policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.BuildKey);
            if (policy != null)
            {
                context.BuildKey = policy.Map(context.BuildKey, context);
            }
        }
    }
}

