namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;

    public class NamedTypeDependencyResolverPolicy : IDependencyResolverPolicy, IBuilderPolicy
    {
        private string name;
        private System.Type type;

        public NamedTypeDependencyResolverPolicy(System.Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return context.NewBuildUp(new NamedTypeBuildKey(this.type, this.name));
        }

        public string Name =>
            this.name;

        public System.Type Type =>
            this.type;
    }
}

