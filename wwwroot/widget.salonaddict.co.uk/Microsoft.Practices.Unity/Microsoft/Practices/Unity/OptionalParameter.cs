namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class OptionalParameter : TypedInjectionValue
    {
        private readonly string name;

        public OptionalParameter(Type type) : this(type, null)
        {
        }

        public OptionalParameter(Type type, string name) : base(type)
        {
            this.name = name;
        }

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            ReflectionHelper helper = new ReflectionHelper(this.ParameterType);
            Type closedParameterType = helper.Type;
            if (helper.IsOpenGeneric)
            {
                closedParameterType = helper.GetClosedParameterType(typeToBuild.GetGenericArguments());
            }
            return new OptionalDependencyResolverPolicy(closedParameterType, this.name);
        }
    }
}

