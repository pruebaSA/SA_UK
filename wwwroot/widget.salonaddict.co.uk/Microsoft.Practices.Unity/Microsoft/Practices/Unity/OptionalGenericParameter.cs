namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public class OptionalGenericParameter : GenericParameterBase
    {
        public OptionalGenericParameter(string genericParameterName) : base(genericParameterName)
        {
        }

        public OptionalGenericParameter(string genericParameterName, string resolutionKey) : base(genericParameterName, resolutionKey)
        {
        }

        protected override IDependencyResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey) => 
            new OptionalDependencyResolverPolicy(typeToResolve, resolutionKey);
    }
}

