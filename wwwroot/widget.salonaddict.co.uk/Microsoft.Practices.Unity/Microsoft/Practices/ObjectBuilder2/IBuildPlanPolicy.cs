namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IBuildPlanPolicy : IBuilderPolicy
    {
        void BuildUp(IBuilderContext context);
    }
}

