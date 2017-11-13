namespace Microsoft.Practices.ObjectBuilder2
{
    public interface IBuildKeyMappingPolicy : IBuilderPolicy
    {
        NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context);
    }
}

