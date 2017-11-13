namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using System;

    public class InjectionParameter : TypedInjectionValue
    {
        private readonly object parameterValue;

        public InjectionParameter(object parameterValue) : this(GetParameterType(parameterValue), parameterValue)
        {
        }

        public InjectionParameter(Type parameterType, object parameterValue) : base(parameterType)
        {
            this.parameterValue = parameterValue;
        }

        private static Type GetParameterType(object parameterValue) => 
            parameterValue?.GetType();

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild) => 
            new LiteralValueDependencyResolverPolicy(this.parameterValue);
    }
}

