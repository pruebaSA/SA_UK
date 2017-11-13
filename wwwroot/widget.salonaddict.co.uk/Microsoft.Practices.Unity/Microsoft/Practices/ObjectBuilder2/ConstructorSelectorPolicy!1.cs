namespace Microsoft.Practices.ObjectBuilder2
{
    using System.Reflection;

    public class ConstructorSelectorPolicy<TInjectionConstructorMarkerAttribute> : ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute> where TInjectionConstructorMarkerAttribute: Attribute
    {
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter) => 
            new FixedTypeResolverPolicy(parameter.ParameterType);
    }
}

