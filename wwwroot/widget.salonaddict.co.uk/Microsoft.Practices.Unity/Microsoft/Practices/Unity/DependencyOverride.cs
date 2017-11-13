namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public class DependencyOverride : ResolverOverride
    {
        private readonly InjectionParameterValue dependencyValue;
        private readonly Type typeToConstruct;

        public DependencyOverride(Type typeToConstruct, object dependencyValue)
        {
            this.typeToConstruct = typeToConstruct;
            this.dependencyValue = InjectionParameterValue.ToParameter(dependencyValue);
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            IDependencyResolverPolicy resolverPolicy = null;
            if (dependencyType == this.typeToConstruct)
            {
                resolverPolicy = this.dependencyValue.GetResolverPolicy(dependencyType);
            }
            return resolverPolicy;
        }
    }
}

