namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class BuilderAwareStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            IBuilderAware existing = context.Existing as IBuilderAware;
            if (existing != null)
            {
                existing.OnBuiltUp(context.BuildKey);
            }
        }

        public override void PreTearDown(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            IBuilderAware existing = context.Existing as IBuilderAware;
            if (existing != null)
            {
                existing.OnTearingDown();
            }
        }
    }
}

