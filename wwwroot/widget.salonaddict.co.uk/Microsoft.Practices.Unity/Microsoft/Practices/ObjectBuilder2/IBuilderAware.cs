namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IBuilderAware
    {
        void OnBuiltUp(NamedTypeBuildKey buildKey);
        void OnTearingDown();
    }
}

