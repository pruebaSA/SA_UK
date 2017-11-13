namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public abstract class InjectionMember
    {
        protected InjectionMember()
        {
        }

        public void AddPolicies(Type typeToCreate, IPolicyList policies)
        {
            this.AddPolicies(null, typeToCreate, null, policies);
        }

        public abstract void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies);
    }
}

