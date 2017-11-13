namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.Unity.Utility;
    using System;

    public abstract class TypedInjectionValue : InjectionParameterValue
    {
        private readonly ReflectionHelper parameterReflector;

        protected TypedInjectionValue(Type parameterType)
        {
            this.parameterReflector = new ReflectionHelper(parameterType);
        }

        public override bool MatchesType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            ReflectionHelper helper = new ReflectionHelper(t);
            if (helper.IsOpenGeneric && this.parameterReflector.IsOpenGeneric)
            {
                return (helper.Type.GetGenericTypeDefinition() == this.parameterReflector.Type.GetGenericTypeDefinition());
            }
            return t.IsAssignableFrom(this.parameterReflector.Type);
        }

        public virtual Type ParameterType =>
            this.parameterReflector.Type;

        public override string ParameterTypeName =>
            this.parameterReflector.Type.Name;
    }
}

