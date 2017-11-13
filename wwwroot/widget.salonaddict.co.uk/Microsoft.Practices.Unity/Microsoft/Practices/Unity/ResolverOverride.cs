namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public abstract class ResolverOverride
    {
        protected ResolverOverride()
        {
        }

        public abstract IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType);
        public ResolverOverride OnType<T>() => 
            new TypeBasedOverride<T>(this);

        public ResolverOverride OnType(Type typeToOverride) => 
            new TypeBasedOverride(typeToOverride, this);
    }
}

