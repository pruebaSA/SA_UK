namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class InjectionFactory : InjectionMember
    {
        private readonly Func<IUnityContainer, Type, string, object> factoryFunc;

        public InjectionFactory(Func<IUnityContainer, object> factoryFunc) : this(func)
        {
            Func<IUnityContainer, Type, string, object> func = null;
            if (func == null)
            {
                func = (c, t, s) => factoryFunc(c);
            }
        }

        public InjectionFactory(Func<IUnityContainer, Type, string, object> factoryFunc)
        {
            Guard.ArgumentNotNull(factoryFunc, "factoryFunc");
            this.factoryFunc = factoryFunc;
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            Guard.ArgumentNotNull(policies, "policies");
            FactoryDelegateBuildPlanPolicy policy = new FactoryDelegateBuildPlanPolicy(this.factoryFunc);
            policies.Set<IBuildPlanPolicy>(policy, new NamedTypeBuildKey(implementationType, name));
        }
    }
}

