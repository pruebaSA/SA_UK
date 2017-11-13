namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class ParameterOverride : ResolverOverride
    {
        private readonly string parameterName;
        private readonly InjectionParameterValue parameterValue;

        public ParameterOverride(string parameterName, object parameterValue)
        {
            this.parameterName = parameterName;
            this.parameterValue = InjectionParameterValue.ToParameter(parameterValue);
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            Guard.ArgumentNotNull(context, "context");
            ConstructorArgumentResolveOperation currentOperation = context.CurrentOperation as ConstructorArgumentResolveOperation;
            if ((currentOperation != null) && (currentOperation.ParameterName == this.parameterName))
            {
                return this.parameterValue.GetResolverPolicy(dependencyType);
            }
            return null;
        }
    }
}

