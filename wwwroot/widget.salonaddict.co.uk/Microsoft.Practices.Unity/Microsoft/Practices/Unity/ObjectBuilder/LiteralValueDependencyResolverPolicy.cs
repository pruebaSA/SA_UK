namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public class LiteralValueDependencyResolverPolicy : IDependencyResolverPolicy, IBuilderPolicy
    {
        private object dependencyValue;

        public LiteralValueDependencyResolverPolicy(object dependencyValue)
        {
            this.dependencyValue = dependencyValue;
        }

        public object Resolve(IBuilderContext context) => 
            this.dependencyValue;
    }
}

