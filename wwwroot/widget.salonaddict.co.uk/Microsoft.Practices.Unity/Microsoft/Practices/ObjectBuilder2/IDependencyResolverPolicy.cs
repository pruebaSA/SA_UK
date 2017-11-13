namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IDependencyResolverPolicy : IBuilderPolicy
    {
        object Resolve(IBuilderContext context);
    }
}

