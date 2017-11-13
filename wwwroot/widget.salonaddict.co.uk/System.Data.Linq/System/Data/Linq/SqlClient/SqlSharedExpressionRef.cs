namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlSharedExpressionRef : SqlExpression
    {
        private SqlSharedExpression expr;

        internal SqlSharedExpressionRef(SqlSharedExpression expr) : base(SqlNodeType.SharedExpressionRef, expr.ClrType, expr.SourceExpression)
        {
            this.expr = expr;
        }

        internal SqlSharedExpression SharedExpression =>
            this.expr;

        internal override ProviderType SqlType =>
            this.expr.SqlType;
    }
}

