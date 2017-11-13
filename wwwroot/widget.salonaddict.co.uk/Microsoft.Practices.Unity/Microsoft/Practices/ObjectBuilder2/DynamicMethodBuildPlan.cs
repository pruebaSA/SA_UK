namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    internal class DynamicMethodBuildPlan : IBuildPlanPolicy, IBuilderPolicy
    {
        private DynamicBuildPlanMethod planMethod;

        public DynamicMethodBuildPlan(DynamicBuildPlanMethod planMethod)
        {
            this.planMethod = planMethod;
        }

        public void BuildUp(IBuilderContext context)
        {
            this.planMethod(context);
        }
    }
}

