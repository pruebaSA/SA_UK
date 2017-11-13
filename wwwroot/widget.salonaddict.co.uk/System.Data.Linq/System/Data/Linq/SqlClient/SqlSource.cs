namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal abstract class SqlSource : SqlNode
    {
        internal SqlSource(SqlNodeType nt, Expression sourceExpression) : base(nt, sourceExpression)
        {
        }
    }
}

