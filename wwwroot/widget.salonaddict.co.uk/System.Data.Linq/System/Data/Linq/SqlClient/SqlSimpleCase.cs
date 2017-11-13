namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlSimpleCase : SqlExpression
    {
        private SqlExpression expression;
        private List<SqlWhen> whens;

        internal SqlSimpleCase(Type clrType, SqlExpression expr, IEnumerable<SqlWhen> whens, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.SimpleCase, clrType, sourceExpression)
        {
            this.whens = new List<SqlWhen>();
            this.Expression = expr;
            if (whens == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("whens");
            }
            this.whens.AddRange(whens);
            if (this.whens.Count == 0)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("whens");
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
                if ((this.expression != null) && (this.expression.ClrType != value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.expression.ClrType, value.ClrType);
                }
                this.expression = value;
            }
        }

        internal override ProviderType SqlType =>
            this.whens[0].Value.SqlType;

        internal List<SqlWhen> Whens =>
            this.whens;
    }
}

