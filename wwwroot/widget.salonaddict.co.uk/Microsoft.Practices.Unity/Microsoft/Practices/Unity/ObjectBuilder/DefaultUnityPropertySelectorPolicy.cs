namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public class DefaultUnityPropertySelectorPolicy : PropertySelectorBase<DependencyResolutionAttribute>
    {
        protected override IDependencyResolverPolicy CreateResolver(PropertyInfo property)
        {
            List<DependencyResolutionAttribute> list = property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false).OfType<DependencyResolutionAttribute>().ToList<DependencyResolutionAttribute>();
            Debug.Assert(list.Count == 1);
            return list[0].CreateResolver(property.PropertyType);
        }
    }
}

