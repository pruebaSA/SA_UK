namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Provider;

    internal interface ICompiledSubQuery
    {
        IExecuteResult Execute(IProvider provider, object[] parentArgs, object[] userArgs);
    }
}

