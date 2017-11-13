namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlBetween : SqlSimpleTypeExpression
    {
        private SqlExpression end;
        private SqlExpression expression;
        private SqlExpression start;

        internal SqlBetween(Type clrType, ProviderType sqlType, SqlExpression expr, SqlExpression start, SqlExpression end, System.Linq.Expressions.Expression source) : base(SqlNodeType.Between, clrType, sqlType, source)
        {
            this.expression = expr;
            this.start = start;
            this.end = end;
        }

        internal SqlExpression End
        {
            get => 
                this.end;
            set
            {
                this.end = value;
            }
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }

        internal SqlExpression Start
        {
            get => 
                this.start;
            set
            {
                this.start = value;
            }
        }
    }
}

