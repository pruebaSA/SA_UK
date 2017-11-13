namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public abstract class DependencyResolutionAttribute : Attribute
    {
        protected DependencyResolutionAttribute()
        {
        }

        public abstract IDependencyResolverPolicy CreateResolver(Type typeToResolve);
    }
}

