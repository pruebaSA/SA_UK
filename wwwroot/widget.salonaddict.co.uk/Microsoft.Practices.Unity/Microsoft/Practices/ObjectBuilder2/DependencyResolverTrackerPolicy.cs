namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections.Generic;

    public class DependencyResolverTrackerPolicy : IDependencyResolverTrackerPolicy, IBuilderPolicy
    {
        private List<object> keys = new List<object>();

        public void AddResolverKey(object key)
        {
            this.keys.Add(key);
        }

        public static IDependencyResolverTrackerPolicy GetTracker(IPolicyList policies, object buildKey)
        {
            IDependencyResolverTrackerPolicy policy = policies.Get<IDependencyResolverTrackerPolicy>(buildKey);
            if (policy == null)
            {
                policy = new DependencyResolverTrackerPolicy();
                policies.Set<IDependencyResolverTrackerPolicy>(policy, buildKey);
            }
            return policy;
        }

        public void RemoveResolvers(IPolicyList policies)
        {
            foreach (object obj2 in this.keys)
            {
                policies.Clear<IDependencyResolverPolicy>(obj2);
            }
            this.keys.Clear();
        }

        public static void RemoveResolvers(IPolicyList policies, object buildKey)
        {
            GetTracker(policies, buildKey).RemoveResolvers(policies);
        }

        public static void TrackKey(IPolicyList policies, object buildKey, object resolverKey)
        {
            GetTracker(policies, buildKey).AddResolverKey(resolverKey);
        }
    }
}

