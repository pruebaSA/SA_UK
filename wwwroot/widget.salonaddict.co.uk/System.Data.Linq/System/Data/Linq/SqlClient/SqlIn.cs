namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlIn : SqlSimpleTypeExpression
    {
        private SqlExpression expression;
        private List<SqlExpression> values;

        internal SqlIn(Type clrType, ProviderType sqlType, SqlExpression expression, IEnumerable<SqlExpression> values, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.In, clrType, sqlType, sourceExpression)
        {
            this.expression = expression;
            this.values = (values != null) ? new List<SqlExpression>(values) : new List<SqlExpression>(0);
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
                this.expression = value;
            }
        }

        internal List<SqlExpression> Values =>
            this.values;
    }
}

