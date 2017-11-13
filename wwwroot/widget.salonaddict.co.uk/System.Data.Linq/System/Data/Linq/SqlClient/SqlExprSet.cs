namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlExprSet : SqlExpression
    {
        private List<SqlExpression> expressions;

        internal SqlExprSet(Type clrType, IEnumerable<SqlExpression> exprs, Expression sourceExpression) : base(SqlNodeType.ExprSet, clrType, sourceExpression)
        {
            this.expressions = new List<SqlExpression>(exprs);
        }

        internal SqlExpression GetFirstExpression()
        {
            SqlExpression expression = this.expressions[0];
            while (expression is SqlExprSet)
            {
                expression = ((SqlExprSet) expression).Expressions[0];
            }
            return expression;
        }

        internal List<SqlExpression> Expressions =>
            this.expressions;

        internal override ProviderType SqlType =>
            this.expressions[0].SqlType;
    }
}

