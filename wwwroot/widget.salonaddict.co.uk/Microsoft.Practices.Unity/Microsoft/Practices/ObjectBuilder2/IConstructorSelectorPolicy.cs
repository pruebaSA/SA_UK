namespace Microsoft.Practices.ObjectBuilder2
{
    public interface IConstructorSelectorPolicy : IBuilderPolicy
    {
        SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination);
    }
}

