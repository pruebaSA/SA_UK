namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class PropertyOverride : ResolverOverride
    {
        private readonly string propertyName;
        private readonly InjectionParameterValue propertyValue;

        public PropertyOverride(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = InjectionParameterValue.ToParameter(propertyValue);
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            Guard.ArgumentNotNull(context, "context");
            ResolvingPropertyValueOperation currentOperation = context.CurrentOperation as ResolvingPropertyValueOperation;
            if ((currentOperation != null) && (currentOperation.PropertyName == this.propertyName))
            {
                return this.propertyValue.GetResolverPolicy(dependencyType);
            }
            return null;
        }
    }
}

