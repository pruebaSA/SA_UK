namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class SpecifiedPropertiesSelectorPolicy : IPropertySelectorPolicy, IBuilderPolicy
    {
        private readonly List<Pair<PropertyInfo, InjectionParameterValue>> propertiesAndValues = new List<Pair<PropertyInfo, InjectionParameterValue>>();

        public void AddPropertyAndValue(PropertyInfo property, InjectionParameterValue value)
        {
            this.propertiesAndValues.Add(Pair.Make<PropertyInfo, InjectionParameterValue>(property, value));
        }

        public IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type type = context.BuildKey.Type;
            ReflectionHelper iteratorVariable1 = new ReflectionHelper(context.BuildKey.Type);
            foreach (Pair<PropertyInfo, InjectionParameterValue> iteratorVariable2 in this.propertiesAndValues)
            {
                PropertyInfo first = iteratorVariable2.First;
                if (new ReflectionHelper(iteratorVariable2.First.DeclaringType).IsOpenGeneric)
                {
                    first = iteratorVariable1.Type.GetProperty(first.Name);
                }
                string buildKey = Guid.NewGuid().ToString();
                resolverPolicyDestination.Set<IDependencyResolverPolicy>(iteratorVariable2.Second.GetResolverPolicy(type), buildKey);
                yield return new SelectedProperty(first, buildKey);
            }
        }

    }
}

