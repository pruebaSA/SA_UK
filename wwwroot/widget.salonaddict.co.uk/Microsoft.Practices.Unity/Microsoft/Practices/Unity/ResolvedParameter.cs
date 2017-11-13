namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class ResolvedParameter : TypedInjectionValue
    {
        private readonly string name;

        public ResolvedParameter(Type parameterType) : this(parameterType, null)
        {
        }

        public ResolvedParameter(Type parameterType, string name) : base(parameterType)
        {
            this.name = name;
        }

        private IDependencyResolverPolicy CreateGenericArrayResolverPolicy(Type typeToBuild, ReflectionHelper parameterReflector) => 
            new NamedTypeDependencyResolverPolicy(parameterReflector.GetClosedParameterType(typeToBuild.GetGenericArguments()), this.name);

        private IDependencyResolverPolicy CreateGenericResolverPolicy(Type typeToBuild, ReflectionHelper parameterReflector) => 
            new NamedTypeDependencyResolverPolicy(parameterReflector.GetClosedParameterType(typeToBuild.GetGenericArguments()), this.name);

        private IDependencyResolverPolicy CreateResolverPolicy(Type typeToResolve) => 
            new NamedTypeDependencyResolverPolicy(typeToResolve, this.name);

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            ReflectionHelper parameterReflector = new ReflectionHelper(this.ParameterType);
            if (parameterReflector.IsGenericArray)
            {
                return this.CreateGenericArrayResolverPolicy(typeToBuild, parameterReflector);
            }
            if (parameterReflector.IsOpenGeneric)
            {
                return this.CreateGenericResolverPolicy(typeToBuild, parameterReflector);
            }
            return this.CreateResolverPolicy(parameterReflector.Type);
        }
    }
}

