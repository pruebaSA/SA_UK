namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class PolicyListExtensions
    {
        public static void Clear<TPolicyInterface>(this IPolicyList policies, object buildKey) where TPolicyInterface: IBuilderPolicy
        {
            policies.Clear(typeof(TPolicyInterface), buildKey);
        }

        public static void ClearDefault<TPolicyInterface>(this IPolicyList policies) where TPolicyInterface: IBuilderPolicy
        {
            policies.ClearDefault(typeof(TPolicyInterface));
        }

        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.Get(typeof(TPolicyInterface), buildKey, false));

        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, out IPolicyList containingPolicyList) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.Get(typeof(TPolicyInterface), buildKey, false, out containingPolicyList));

        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.Get(typeof(TPolicyInterface), buildKey, localOnly));

        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface, object buildKey) => 
            policies.Get(policyInterface, buildKey, false);

        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.Get(typeof(TPolicyInterface), buildKey, localOnly, out containingPolicyList));

        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface, object buildKey, out IPolicyList containingPolicyList) => 
            policies.Get(policyInterface, buildKey, false, out containingPolicyList);

        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
        {
            IPolicyList list;
            return policies.Get(policyInterface, buildKey, localOnly, out list);
        }

        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly));

        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList) where TPolicyInterface: IBuilderPolicy => 
            ((TPolicyInterface) policies.GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly, out containingPolicyList));

        public static IBuilderPolicy GetNoDefault(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
        {
            IPolicyList list;
            return policies.GetNoDefault(policyInterface, buildKey, localOnly, out list);
        }

        public static void Set<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy, object buildKey) where TPolicyInterface: IBuilderPolicy
        {
            policies.Set(typeof(TPolicyInterface), policy, buildKey);
        }

        public static void SetDefault<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy) where TPolicyInterface: IBuilderPolicy
        {
            policies.SetDefault(typeof(TPolicyInterface), policy);
        }
    }
}

