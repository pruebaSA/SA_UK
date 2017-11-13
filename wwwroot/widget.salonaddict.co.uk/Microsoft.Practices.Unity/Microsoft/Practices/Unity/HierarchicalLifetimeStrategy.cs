namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public class HierarchicalLifetimeStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList list;
            if (!(!(context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out list) is HierarchicalLifetimeManager) || object.ReferenceEquals(list, context.PersistentPolicies)))
            {
                HierarchicalLifetimeManager policy = new HierarchicalLifetimeManager {
                    InUse = true
                };
                context.PersistentPolicies.Set<ILifetimePolicy>(policy, context.BuildKey);
                context.Lifetime.Add(policy);
            }
        }
    }
}

