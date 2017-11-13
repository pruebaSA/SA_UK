namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple=false)]
    public sealed class OptionalDependencyAttribute : DependencyResolutionAttribute
    {
        private readonly string name;

        public OptionalDependencyAttribute() : this(null)
        {
        }

        public OptionalDependencyAttribute(string name)
        {
            this.name = name;
        }

        public override IDependencyResolverPolicy CreateResolver(Type typeToResolve) => 
            new OptionalDependencyResolverPolicy(typeToResolve, this.name);

        public string Name =>
            this.name;
    }
}

