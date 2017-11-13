namespace Microsoft.Practices.ObjectBuilder2
{
    public interface IBuildPlanCreatorPolicy : IBuilderPolicy
    {
        IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey);
    }
}

