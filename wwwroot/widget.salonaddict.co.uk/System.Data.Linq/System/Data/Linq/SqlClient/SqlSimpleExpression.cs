namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlSimpleExpression : SqlExpression
    {
        private SqlExpression expr;

        internal SqlSimpleExpression(SqlExpression expr) : base(SqlNodeType.SimpleExpression, expr.ClrType, expr.SourceExpression)
        {
            this.expr = expr;
        }

        internal SqlExpression Expression
        {
            get => 
                this.expr;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                if (!base.ClrType.IsAssignableFrom(value.ClrType))
                {
                    throw Error.ArgumentWrongType("value", base.ClrType, value.ClrType);
                }
                this.expr = value;
            }
        }

        internal override ProviderType SqlType =>
            this.expr.SqlType;
    }
}

