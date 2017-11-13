namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface ILifetimePolicy : IBuilderPolicy
    {
        object GetValue();
        void RemoveValue();
        void SetValue(object newValue);
    }
}

