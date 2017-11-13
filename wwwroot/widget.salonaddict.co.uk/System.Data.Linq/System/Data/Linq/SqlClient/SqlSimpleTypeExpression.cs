namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal abstract class SqlSimpleTypeExpression : SqlExpression
    {
        private ProviderType sqlType;

        internal SqlSimpleTypeExpression(SqlNodeType nodeType, Type clrType, ProviderType sqlType, Expression sourceExpression) : base(nodeType, clrType, sourceExpression)
        {
            this.sqlType = sqlType;
        }

        internal void SetSqlType(ProviderType type)
        {
            this.sqlType = type;
        }

        internal override ProviderType SqlType =>
            this.sqlType;
    }
}

