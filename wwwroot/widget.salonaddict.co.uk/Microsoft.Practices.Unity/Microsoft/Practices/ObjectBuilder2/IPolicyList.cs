namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Runtime.InteropServices;

    public interface IPolicyList
    {
        void Clear(Type policyInterface, object buildKey);
        void ClearAll();
        void ClearDefault(Type policyInterface);
        IBuilderPolicy Get(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList);
        IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList);
        void Set(Type policyInterface, IBuilderPolicy policy, object buildKey);
        void SetDefault(Type policyInterface, IBuilderPolicy policy);
    }
}

