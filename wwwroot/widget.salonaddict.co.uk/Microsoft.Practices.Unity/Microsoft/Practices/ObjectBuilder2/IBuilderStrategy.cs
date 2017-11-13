namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IBuilderStrategy
    {
        void PostBuildUp(IBuilderContext context);
        void PostTearDown(IBuilderContext context);
        void PreBuildUp(IBuilderContext context);
        void PreTearDown(IBuilderContext context);
    }
}

