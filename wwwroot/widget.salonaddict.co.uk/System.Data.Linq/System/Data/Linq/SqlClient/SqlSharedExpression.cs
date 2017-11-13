namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlSharedExpression : SqlExpression
    {
        private SqlExpression expr;

        internal SqlSharedExpression(SqlExpression expr) : base(SqlNodeType.SharedExpression, expr.ClrType, expr.SourceExpression)
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
                if (!base.ClrType.IsAssignableFrom(value.ClrType) && !value.ClrType.IsAssignableFrom(base.ClrType))
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

