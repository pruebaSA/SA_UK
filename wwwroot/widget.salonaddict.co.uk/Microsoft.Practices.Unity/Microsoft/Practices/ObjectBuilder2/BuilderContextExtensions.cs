namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Runtime.CompilerServices;

    public static class BuilderContextExtensions
    {
        public static void AddResolverOverrides(this IBuilderContext context, params ResolverOverride[] overrides)
        {
            context.AddResolverOverrides(overrides);
        }

        public static TResult NewBuildUp<TResult>(this IBuilderContext context) => 
            context.NewBuildUp<TResult>(null);

        public static TResult NewBuildUp<TResult>(this IBuilderContext context, string name) => 
            ((TResult) context.NewBuildUp(NamedTypeBuildKey.Make<TResult>(name)));
    }
}

