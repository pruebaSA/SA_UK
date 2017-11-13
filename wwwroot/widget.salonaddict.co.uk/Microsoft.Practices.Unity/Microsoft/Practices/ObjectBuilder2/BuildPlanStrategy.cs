namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class BuildPlanStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList list;
            IBuildPlanPolicy policy = context.Policies.Get<IBuildPlanPolicy>(context.BuildKey, out list);
            if ((policy == null) || (policy is OverriddenBuildPlanMarkerPolicy))
            {
                IPolicyList list2;
                IBuildPlanCreatorPolicy policy2 = context.Policies.Get<IBuildPlanCreatorPolicy>(context.BuildKey, out list2);
                if (policy2 != null)
                {
                    policy = policy2.CreatePlan(context, context.BuildKey);
                    (list ?? list2).Set<IBuildPlanPolicy>(policy, context.BuildKey);
                }
            }
            if (policy != null)
            {
                policy.BuildUp(context);
            }
        }
    }
}

