namespace Microsoft.Practices.ObjectBuilder2
{
    using System.Reflection;

    public class PropertySelectorPolicy<TResolutionAttribute> : PropertySelectorBase<TResolutionAttribute> where TResolutionAttribute: Attribute
    {
        protected override IDependencyResolverPolicy CreateResolver(PropertyInfo property) => 
            new FixedTypeResolverPolicy(property.PropertyType);
    }
}

