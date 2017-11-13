namespace System.Data.Linq.Provider
{
    using System;
    using System.Data.Linq;

    internal interface ICompiledQuery
    {
        IExecuteResult Execute(IProvider provider, object[] arguments);
    }
}

