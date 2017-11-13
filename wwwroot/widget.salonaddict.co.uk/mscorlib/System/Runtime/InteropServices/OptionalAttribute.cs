namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [ComVisible(true), AttributeUsage(AttributeTargets.Parameter, Inherited=false)]
    public sealed class OptionalAttribute : Attribute
    {
        internal static Attribute GetCustomAttribute(ParameterInfo parameter)
        {
            if (!parameter.IsOptional)
            {
                return null;
            }
            return new OptionalAttribute();
        }

        internal static bool IsDefined(ParameterInfo parameter) => 
            parameter.IsOptional;
    }
}

