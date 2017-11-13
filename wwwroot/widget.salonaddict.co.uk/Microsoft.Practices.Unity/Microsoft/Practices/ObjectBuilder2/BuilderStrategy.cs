namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public abstract class BuilderStrategy : IBuilderStrategy
    {
        protected BuilderStrategy()
        {
        }

        public virtual void PostBuildUp(IBuilderContext context)
        {
        }

        public virtual void PostTearDown(IBuilderContext context)
        {
        }

        public virtual void PreBuildUp(IBuilderContext context)
        {
        }

        public virtual void PreTearDown(IBuilderContext context)
        {
        }
    }
}

