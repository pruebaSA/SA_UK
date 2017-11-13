namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Common;

    internal interface IObjectReaderCompiler
    {
        IObjectReaderFactory Compile(SqlExpression expression, Type elementType);
        IObjectReaderSession CreateSession(DbDataReader reader, IReaderProvider provider, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries);
    }
}

