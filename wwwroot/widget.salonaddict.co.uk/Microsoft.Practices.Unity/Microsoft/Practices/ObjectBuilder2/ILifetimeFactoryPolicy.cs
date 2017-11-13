namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface ILifetimeFactoryPolicy : IBuilderPolicy
    {
        ILifetimePolicy CreateLifetimePolicy();

        Type LifetimeType { get; }
    }
}

