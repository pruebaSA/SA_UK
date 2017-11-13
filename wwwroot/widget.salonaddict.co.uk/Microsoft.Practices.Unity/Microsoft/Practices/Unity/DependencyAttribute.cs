namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public sealed class DependencyAttribute : DependencyResolutionAttribute
    {
        private readonly string name;

        public DependencyAttribute() : this(null)
        {
        }

        public DependencyAttribute(string name)
        {
            this.name = name;
        }

        public override IDependencyResolverPolicy CreateResolver(Type typeToResolve) => 
            new NamedTypeDependencyResolverPolicy(typeToResolve, this.name);

        public string Name =>
            this.name;
    }
}

