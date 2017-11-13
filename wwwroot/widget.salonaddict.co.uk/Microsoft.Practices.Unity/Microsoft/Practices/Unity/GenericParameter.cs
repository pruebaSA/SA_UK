namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using System;

    public class GenericParameter : GenericParameterBase
    {
        public GenericParameter(string genericParameterName) : base(genericParameterName)
        {
        }

        public GenericParameter(string genericParameterName, string resolutionKey) : base(genericParameterName, resolutionKey)
        {
        }

        protected override IDependencyResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey) => 
            new NamedTypeDependencyResolverPolicy(typeToResolve, resolutionKey);
    }
}

