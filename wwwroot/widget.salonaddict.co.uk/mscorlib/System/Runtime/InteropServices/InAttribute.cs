﻿namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [ComVisible(true), AttributeUsage(AttributeTargets.Parameter, Inherited=false)]
    public sealed class InAttribute : Attribute
    {
        internal static Attribute GetCustomAttribute(ParameterInfo parameter)
        {
            if (!parameter.IsIn)
            {
                return null;
            }
            return new InAttribute();
        }

        internal static bool IsDefined(ParameterInfo parameter) => 
            parameter.IsIn;
    }
}

