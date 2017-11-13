namespace System.Data.Linq.Provider
{
    using System;
    using System.Collections;
    using System.Data.Common;
    using System.Data.Linq;
    using System.IO;
    using System.Linq.Expressions;

    internal interface IProvider : IDisposable
    {
        void ClearConnection();
        ICompiledQuery Compile(Expression query);
        void CreateDatabase();
        bool DatabaseExists();
        void DeleteDatabase();
        IExecuteResult Execute(Expression query);
        DbCommand GetCommand(Expression query);
        string GetQueryText(Expression query);
        void Initialize(IDataServices dataServices, object connection);
        System.Data.Linq.IMultipleResults Translate(DbDataReader reader);
        IEnumerable Translate(Type elementType, DbDataReader reader);

        int CommandTimeout { get; set; }

        DbConnection Connection { get; }

        TextWriter Log { get; set; }

        DbTransaction Transaction { get; set; }
    }
}

