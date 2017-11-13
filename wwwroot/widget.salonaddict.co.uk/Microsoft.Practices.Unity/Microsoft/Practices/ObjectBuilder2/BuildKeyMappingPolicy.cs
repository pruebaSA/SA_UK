namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class BuildKeyMappingPolicy : IBuildKeyMappingPolicy, IBuilderPolicy
    {
        private readonly NamedTypeBuildKey newBuildKey;

        public BuildKeyMappingPolicy(NamedTypeBuildKey newBuildKey)
        {
            this.newBuildKey = newBuildKey;
        }

        public NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context) => 
            this.newBuildKey;
    }
}

