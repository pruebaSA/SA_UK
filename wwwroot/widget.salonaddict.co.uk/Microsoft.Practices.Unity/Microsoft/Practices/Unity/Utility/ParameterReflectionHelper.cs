namespace Microsoft.Practices.Unity.Utility
{
    using System;
    using System.Reflection;

    public class ParameterReflectionHelper : ReflectionHelper
    {
        public ParameterReflectionHelper(ParameterInfo parameter) : base(TypeFromParameterInfo(parameter))
        {
        }

        private static Type TypeFromParameterInfo(ParameterInfo parameter)
        {
            Guard.ArgumentNotNull(parameter, "parameter");
            return parameter.ParameterType;
        }
    }
}

