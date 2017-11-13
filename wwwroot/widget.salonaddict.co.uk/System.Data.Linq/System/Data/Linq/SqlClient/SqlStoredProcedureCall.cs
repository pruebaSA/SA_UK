namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlStoredProcedureCall : SqlUserQuery
    {
        private MetaFunction function;

        internal SqlStoredProcedureCall(MetaFunction function, SqlExpression projection, IEnumerable<SqlExpression> args, Expression source) : base(SqlNodeType.StoredProcedureCall, projection, args, source)
        {
            if (function == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("function");
            }
            this.function = function;
        }

        internal MetaFunction Function =>
            this.function;
    }
}

