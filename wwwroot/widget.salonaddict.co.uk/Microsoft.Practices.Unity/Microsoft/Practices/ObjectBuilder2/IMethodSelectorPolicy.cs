namespace Microsoft.Practices.ObjectBuilder2
{
    using System.Collections.Generic;

    public interface IMethodSelectorPolicy : IBuilderPolicy
    {
        IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination);
    }
}

