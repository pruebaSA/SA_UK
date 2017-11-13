namespace Microsoft.Practices.ObjectBuilder2
{
    using System.Reflection;

    public class MethodSelectorPolicy<TMarkerAttribute> : MethodSelectorPolicyBase<TMarkerAttribute> where TMarkerAttribute: Attribute
    {
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter) => 
            new FixedTypeResolverPolicy(parameter.ParameterType);
    }
}

