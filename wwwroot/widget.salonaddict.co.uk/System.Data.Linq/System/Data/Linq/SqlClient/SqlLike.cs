namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlLike : SqlSimpleTypeExpression
    {
        private SqlExpression escape;
        private SqlExpression expression;
        private SqlExpression pattern;

        internal SqlLike(Type clrType, ProviderType sqlType, SqlExpression expr, SqlExpression pattern, SqlExpression escape, System.Linq.Expressions.Expression source) : base(SqlNodeType.Like, clrType, sqlType, source)
        {
            if (expr == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("expr");
            }
            if (pattern == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("pattern");
            }
            this.Expression = expr;
            this.Pattern = pattern;
            this.Escape = escape;
        }

        internal SqlExpression Escape
        {
            get => 
                this.escape;
            set
            {
                if ((value != null) && (value.ClrType != typeof(string)))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", "string", value.ClrType);
                }
                this.escape = value;
            }
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if (value.ClrType != typeof(string))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", "string", value.ClrType);
                }
                this.expression = value;
            }
        }

        internal SqlExpression Pattern
        {
            get => 
                this.pattern;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if (value.ClrType != typeof(string))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", "string", value.ClrType);
                }
                this.pattern = value;
            }
        }
    }
}

