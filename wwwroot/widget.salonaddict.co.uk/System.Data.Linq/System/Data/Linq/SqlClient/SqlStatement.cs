namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal abstract class SqlStatement : SqlNode
    {
        internal SqlStatement(SqlNodeType nodeType, Expression sourceExpression) : base(nodeType, sourceExpression)
        {
        }
    }
}

