namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class LifetimeStrategy : BuilderStrategy
    {
        private static ILifetimePolicy GetLifetimePolicy(IBuilderContext context)
        {
            ILifetimePolicy lifetimePolicyForGenericType = context.Policies.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
            if ((lifetimePolicyForGenericType == null) && context.BuildKey.Type.IsGenericType)
            {
                lifetimePolicyForGenericType = GetLifetimePolicyForGenericType(context);
            }
            if (lifetimePolicyForGenericType == null)
            {
                lifetimePolicyForGenericType = new TransientLifetimeManager();
                context.PersistentPolicies.Set<ILifetimePolicy>(lifetimePolicyForGenericType, context.BuildKey);
            }
            return lifetimePolicyForGenericType;
        }

        private static ILifetimePolicy GetLifetimePolicyForGenericType(IBuilderContext context)
        {
            IPolicyList list;
            object buildKey = new NamedTypeBuildKey(context.BuildKey.Type.GetGenericTypeDefinition(), context.BuildKey.Name);
            ILifetimeFactoryPolicy policy = context.Policies.Get<ILifetimeFactoryPolicy>(buildKey, out list);
            if (policy != null)
            {
                ILifetimePolicy policy2 = policy.CreateLifetimePolicy();
                list.Set<ILifetimePolicy>(policy2, context.BuildKey);
                return policy2;
            }
            return null;
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            GetLifetimePolicy(context).SetValue(context.Existing);
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            if (context.Existing == null)
            {
                ILifetimePolicy lifetimePolicy = GetLifetimePolicy(context);
                IRequiresRecovery recovery = lifetimePolicy as IRequiresRecovery;
                if (recovery != null)
                {
                    context.RecoveryStack.Add(recovery);
                }
                object obj2 = lifetimePolicy.GetValue();
                if (obj2 != null)
                {
                    context.Existing = obj2;
                    context.BuildComplete = true;
                }
            }
        }
    }
}

