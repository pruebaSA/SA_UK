namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using System;

    internal class OverriddenBuildPlanMarkerPolicy : IBuildPlanPolicy, IBuilderPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            throw new InvalidOperationException(Resources.MarkerBuildPlanInvoked);
        }
    }
}

