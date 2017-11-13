namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlJoinedCollection : SqlSimpleTypeExpression
    {
        private SqlExpression count;
        private SqlExpression expression;

        internal SqlJoinedCollection(Type clrType, ProviderType sqlType, SqlExpression expression, SqlExpression count, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.JoinedCollection, clrType, sqlType, sourceExpression)
        {
            this.expression = expression;
            this.count = count;
        }

        internal SqlExpression Count
        {
            get => 
                this.count;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if (value.ClrType != typeof(int))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType(value, typeof(int), value.ClrType);
                }
                this.count = value;
            }
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if ((value == null) || ((this.expression != null) && (this.expression.ClrType != value.ClrType)))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType(value, this.expression.ClrType, value.ClrType);
                }
                this.expression = value;
            }
        }
    }
}

