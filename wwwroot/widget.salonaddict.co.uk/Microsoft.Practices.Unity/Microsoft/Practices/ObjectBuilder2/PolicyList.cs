namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class PolicyList : IPolicyList
    {
        private readonly IPolicyList innerPolicyList;
        private readonly object lockObject;
        private Dictionary<PolicyKey, IBuilderPolicy> policies;

        public PolicyList() : this(null)
        {
        }

        public PolicyList(IPolicyList innerPolicyList)
        {
            this.lockObject = new object();
            this.policies = new Dictionary<PolicyKey, IBuilderPolicy>();
            this.innerPolicyList = innerPolicyList ?? new NullPolicyList();
        }

        public void Clear(Type policyInterface, object buildKey)
        {
            lock (this.lockObject)
            {
                Dictionary<PolicyKey, IBuilderPolicy> newPolicies = this.ClonePolicies();
                newPolicies.Remove(new PolicyKey(policyInterface, buildKey));
                this.SwapPolicies(newPolicies);
            }
        }

        public void ClearAll()
        {
            lock (this.lockObject)
            {
                this.SwapPolicies(new Dictionary<PolicyKey, IBuilderPolicy>());
            }
        }

        public void ClearDefault(Type policyInterface)
        {
            this.Clear(policyInterface, null);
        }

        private Dictionary<PolicyKey, IBuilderPolicy> ClonePolicies() => 
            new Dictionary<PolicyKey, IBuilderPolicy>(this.policies);

        public IBuilderPolicy Get(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            Type type;
            TryGetType(buildKey, out type);
            return (this.GetPolicyForKey(policyInterface, buildKey, localOnly, out containingPolicyList) ?? (this.GetPolicyForOpenGenericKey(policyInterface, buildKey, type, localOnly, out containingPolicyList) ?? (this.GetPolicyForType(policyInterface, type, localOnly, out containingPolicyList) ?? (this.GetPolicyForOpenGenericType(policyInterface, type, localOnly, out containingPolicyList) ?? this.GetDefaultForPolicy(policyInterface, localOnly, out containingPolicyList)))));
        }

        private IBuilderPolicy GetDefaultForPolicy(Type policyInterface, bool localOnly, out IPolicyList containingPolicyList) => 
            this.GetNoDefault(policyInterface, null, localOnly, out containingPolicyList);

        public IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            IBuilderPolicy policy;
            containingPolicyList = null;
            if (this.policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out policy))
            {
                containingPolicyList = this;
                return policy;
            }
            if (localOnly)
            {
                return null;
            }
            return this.innerPolicyList.GetNoDefault(policyInterface, buildKey, false, out containingPolicyList);
        }

        private IBuilderPolicy GetPolicyForKey(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildKey != null)
            {
                return this.GetNoDefault(policyInterface, buildKey, localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericKey(Type policyInterface, object buildKey, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if ((buildType != null) && buildType.IsGenericType)
            {
                return this.GetNoDefault(policyInterface, ReplaceType(buildKey, buildType.GetGenericTypeDefinition()), localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericType(Type policyInterface, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if ((buildType != null) && buildType.IsGenericType)
            {
                return this.GetNoDefault(policyInterface, buildType.GetGenericTypeDefinition(), localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForType(Type policyInterface, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildType != null)
            {
                return this.GetNoDefault(policyInterface, buildType, localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private static object ReplaceType(object buildKey, Type newType)
        {
            Type type = buildKey as Type;
            if (type != null)
            {
                return newType;
            }
            NamedTypeBuildKey key = buildKey as NamedTypeBuildKey;
            if (key == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.CannotExtractTypeFromBuildKey, new object[] { buildKey }), "buildKey");
            }
            return new NamedTypeBuildKey(newType, key.Name);
        }

        public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
        {
            lock (this.lockObject)
            {
                Dictionary<PolicyKey, IBuilderPolicy> newPolicies = this.ClonePolicies();
                newPolicies[new PolicyKey(policyInterface, buildKey)] = policy;
                this.SwapPolicies(newPolicies);
            }
        }

        public void SetDefault(Type policyInterface, IBuilderPolicy policy)
        {
            this.Set(policyInterface, policy, null);
        }

        private void SwapPolicies(Dictionary<PolicyKey, IBuilderPolicy> newPolicies)
        {
            this.policies = newPolicies;
            Thread.MemoryBarrier();
        }

        private static bool TryGetType(object buildKey, out Type type)
        {
            type = buildKey as Type;
            if (type == null)
            {
                NamedTypeBuildKey key = buildKey as NamedTypeBuildKey;
                if (key != null)
                {
                    type = key.Type;
                }
            }
            return (type != null);
        }

        public int Count =>
            this.policies.Count;

        private class NullPolicyList : IPolicyList
        {
            public void Clear(Type policyInterface, object buildKey)
            {
                throw new NotImplementedException();
            }

            public void ClearAll()
            {
                throw new NotImplementedException();
            }

            public void ClearDefault(Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public IBuilderPolicy Get(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            {
                containingPolicyList = null;
                return null;
            }

            public IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            {
                containingPolicyList = null;
                return null;
            }

            public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
            {
                throw new NotImplementedException();
            }

            public void SetDefault(Type policyInterface, IBuilderPolicy policy)
            {
                throw new NotImplementedException();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PolicyKey
        {
            public readonly object BuildKey;
            public readonly Type PolicyType;
            public PolicyKey(Type policyType, object buildKey)
            {
                this.PolicyType = policyType;
                this.BuildKey = buildKey;
            }

            public override bool Equals(object obj) => 
                (((obj != null) && (obj.GetType() == typeof(PolicyList.PolicyKey))) && (this == ((PolicyList.PolicyKey) obj)));

            public override int GetHashCode() => 
                ((SafeGetHashCode(this.PolicyType) * 0x25) + SafeGetHashCode(this.BuildKey));

            public static bool operator ==(PolicyList.PolicyKey left, PolicyList.PolicyKey right) => 
                ((left.PolicyType == right.PolicyType) && object.Equals(left.BuildKey, right.BuildKey));

            public static bool operator !=(PolicyList.PolicyKey left, PolicyList.PolicyKey right) => 
                !(left == right);

            private static int SafeGetHashCode(object obj) => 
                ((obj != null) ? obj.GetHashCode() : 0);
        }
    }
}

