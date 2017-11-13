namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class FixedTypeResolverPolicy : IDependencyResolverPolicy, IBuilderPolicy
    {
        private readonly NamedTypeBuildKey keyToBuild;

        public FixedTypeResolverPolicy(Type typeToBuild)
        {
            this.keyToBuild = new NamedTypeBuildKey(typeToBuild);
        }

        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return context.NewBuildUp(this.keyToBuild);
        }
    }
}

