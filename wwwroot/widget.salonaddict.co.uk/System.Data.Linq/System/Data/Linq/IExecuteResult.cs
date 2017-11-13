namespace System.Data.Linq
{
    using System;

    public interface IExecuteResult : IDisposable
    {
        object GetParameterValue(int parameterIndex);

        object ReturnValue { get; }
    }
}

