namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlClientCase : SqlExpression
    {
        private SqlExpression expression;
        private List<SqlClientWhen> whens;

        internal SqlClientCase(Type clrType, SqlExpression expr, IEnumerable<SqlClientWhen> whens, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.ClientCase, clrType, sourceExpression)
        {
            this.whens = new List<SqlClientWhen>();
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

        internal List<SqlClientWhen> Whens =>
            this.whens;
    }
}

