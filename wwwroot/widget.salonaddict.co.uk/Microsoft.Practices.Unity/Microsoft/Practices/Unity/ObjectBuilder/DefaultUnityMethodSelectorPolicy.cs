namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DefaultUnityMethodSelectorPolicy : MethodSelectorPolicyBase<InjectionMethodAttribute>
    {
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            List<DependencyResolutionAttribute> list = parameter.GetCustomAttributes(false).OfType<DependencyResolutionAttribute>().ToList<DependencyResolutionAttribute>();
            if (list.Count > 0)
            {
                return list[0].CreateResolver(parameter.ParameterType);
            }
            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }
    }
}

