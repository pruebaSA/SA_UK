namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class TypeBasedOverride : ResolverOverride
    {
        private readonly ResolverOverride innerOverride;
        private readonly Type targetType;

        public TypeBasedOverride(Type targetType, ResolverOverride innerOverride)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            Guard.ArgumentNotNull(innerOverride, "innerOverride");
            this.targetType = targetType;
            this.innerOverride = innerOverride;
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            Guard.ArgumentNotNull(context, "context");
            BuildOperation currentOperation = context.CurrentOperation as BuildOperation;
            if ((currentOperation != null) && (currentOperation.TypeBeingConstructed == this.targetType))
            {
                return this.innerOverride.GetResolver(context, dependencyType);
            }
            return null;
        }
    }
}

