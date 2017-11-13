namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Common;

    internal interface IObjectReaderFactory
    {
        IObjectReader Create(DbDataReader reader, bool disposeReader, IReaderProvider provider, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries);
        IObjectReader GetNextResult(IObjectReaderSession session, bool disposeReader);
    }
}

