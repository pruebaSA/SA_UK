namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlDoNotVisitExpression : SqlExpression
    {
        private SqlExpression expression;

        internal SqlDoNotVisitExpression(SqlExpression expr) : base(SqlNodeType.DoNotVisit, expr.ClrType, expr.SourceExpression)
        {
            if (expr == null)
            {
                throw Error.ArgumentNull("expr");
            }
            this.expression = expr;
        }

        internal SqlExpression Expression =>
            this.expression;

        internal override ProviderType SqlType =>
            this.expression.SqlType;
    }
}

