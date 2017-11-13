namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public interface IRecoveryStack
    {
        void Add(IRequiresRecovery recovery);
        void ExecuteRecovery();

        int Count { get; }
    }
}

