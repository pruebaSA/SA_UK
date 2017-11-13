﻿namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Parameter, Inherited=false), ComVisible(true)]
    public sealed class OutAttribute : Attribute
    {
        internal static Attribute GetCustomAttribute(ParameterInfo parameter)
        {
            if (!parameter.IsOut)
            {
                return null;
            }
            return new OutAttribute();
        }

        internal static bool IsDefined(ParameterInfo parameter) => 
            parameter.IsOut;
    }
}

