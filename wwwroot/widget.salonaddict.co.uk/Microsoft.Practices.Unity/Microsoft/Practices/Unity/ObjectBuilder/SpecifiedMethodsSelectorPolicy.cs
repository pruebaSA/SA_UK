namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class SpecifiedMethodsSelectorPolicy : IMethodSelectorPolicy, IBuilderPolicy
    {
        private readonly List<Pair<MethodInfo, IEnumerable<InjectionParameterValue>>> methods = new List<Pair<MethodInfo, IEnumerable<InjectionParameterValue>>>();

        public void AddMethodAndParameters(MethodInfo method, IEnumerable<InjectionParameterValue> parameters)
        {
            this.methods.Add(Pair.Make<MethodInfo, IEnumerable<InjectionParameterValue>>(method, parameters));
        }

        public IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            foreach (Pair<MethodInfo, IEnumerable<InjectionParameterValue>> iteratorVariable0 in this.methods)
            {
                SelectedMethod iteratorVariable2;
                Type type = context.BuildKey.Type;
                ReflectionHelper iteratorVariable3 = new ReflectionHelper(iteratorVariable0.First.DeclaringType);
                MethodReflectionHelper iteratorVariable4 = new MethodReflectionHelper(iteratorVariable0.First);
                if (!(iteratorVariable4.MethodHasOpenGenericParameters || iteratorVariable3.IsOpenGeneric))
                {
                    iteratorVariable2 = new SelectedMethod(iteratorVariable0.First);
                }
                else
                {
                    Type[] types = iteratorVariable4.GetClosedParameterTypes(type.GetGenericArguments());
                    iteratorVariable2 = new SelectedMethod(type.GetMethod(iteratorVariable0.First.Name, types));
                }
                SpecifiedMemberSelectorHelper.AddParameterResolvers(type, resolverPolicyDestination, iteratorVariable0.Second, iteratorVariable2);
                yield return iteratorVariable2;
            }
        }

    }
}

