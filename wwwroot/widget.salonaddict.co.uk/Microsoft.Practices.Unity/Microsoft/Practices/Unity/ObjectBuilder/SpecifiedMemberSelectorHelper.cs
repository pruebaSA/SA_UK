namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;

    public static class SpecifiedMemberSelectorHelper
    {
        public static void AddParameterResolvers(Type typeToBuild, IPolicyList policies, IEnumerable<InjectionParameterValue> parameterValues, SelectedMemberWithParameters result)
        {
            foreach (InjectionParameterValue value2 in parameterValues)
            {
                string buildKey = Guid.NewGuid().ToString();
                policies.Set<IDependencyResolverPolicy>(value2.GetResolverPolicy(typeToBuild), buildKey);
                result.AddParameterKey(buildKey);
            }
        }
    }
}

