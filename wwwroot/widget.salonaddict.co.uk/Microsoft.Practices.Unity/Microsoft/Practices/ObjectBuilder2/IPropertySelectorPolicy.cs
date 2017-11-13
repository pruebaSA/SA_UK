namespace Microsoft.Practices.ObjectBuilder2
{
    using System.Collections.Generic;

    public interface IPropertySelectorPolicy : IBuilderPolicy
    {
        IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination);
    }
}

