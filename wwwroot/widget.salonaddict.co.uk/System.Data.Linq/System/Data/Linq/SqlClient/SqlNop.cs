namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlNop : SqlSimpleTypeExpression
    {
        internal SqlNop(Type clrType, ProviderType sqlType, Expression sourceExpression) : base(SqlNodeType.Nop, clrType, sqlType, sourceExpression)
        {
        }
    }
}

