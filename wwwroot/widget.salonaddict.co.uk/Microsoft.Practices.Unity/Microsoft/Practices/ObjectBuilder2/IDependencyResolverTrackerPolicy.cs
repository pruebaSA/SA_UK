namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IDependencyResolverTrackerPolicy : IBuilderPolicy
    {
        void AddResolverKey(object key);
        void RemoveResolvers(IPolicyList policies);
    }
}

