namespace Microsoft.Practices.Unity.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class MethodReflectionHelper
    {
        private readonly MethodBase method;

        public MethodReflectionHelper(MethodBase method)
        {
            this.method = method;
        }

        public Type[] GetClosedParameterTypes(Type[] genericTypeArguments) => 
            this.GetClosedParameterTypesSequence(genericTypeArguments).ToArray<Type>();

        private IEnumerable<Type> GetClosedParameterTypesSequence(Type[] genericTypeArguments)
        {
            foreach (ParameterReflectionHelper iteratorVariable0 in this.GetParameterReflectors())
            {
                yield return iteratorVariable0.GetClosedParameterType(genericTypeArguments);
            }
        }

        private IEnumerable<ParameterReflectionHelper> GetParameterReflectors()
        {
            foreach (ParameterInfo iteratorVariable0 in this.method.GetParameters())
            {
                yield return new ParameterReflectionHelper(iteratorVariable0);
            }
        }

        public bool MethodHasOpenGenericParameters =>
            this.GetParameterReflectors().Any<ParameterReflectionHelper>(r => r.IsOpenGeneric);

        public IEnumerable<Type> ParameterTypes
        {
            get
            {
                foreach (ParameterInfo iteratorVariable0 in this.method.GetParameters())
                {
                    yield return iteratorVariable0.ParameterType;
                }
            }
        }



    }
}

