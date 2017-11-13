namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class InjectionParameterValue
    {
        protected InjectionParameterValue()
        {
        }

        public abstract IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild);
        public abstract bool MatchesType(Type t);
        public static InjectionParameterValue ToParameter(object value)
        {
            InjectionParameterValue value2 = value as InjectionParameterValue;
            if (value2 != null)
            {
                return value2;
            }
            Type parameterType = value as Type;
            if (parameterType != null)
            {
                return new ResolvedParameter(parameterType);
            }
            return new InjectionParameter(value);
        }

        public static IEnumerable<InjectionParameterValue> ToParameters(params object[] values)
        {
            foreach (object iteratorVariable0 in values)
            {
                yield return ToParameter(iteratorVariable0);
            }
        }

        public abstract string ParameterTypeName { get; }

    }
}

