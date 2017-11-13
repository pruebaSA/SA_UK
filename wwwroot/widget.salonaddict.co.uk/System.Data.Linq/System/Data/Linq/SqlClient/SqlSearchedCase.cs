namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlSearchedCase : SqlExpression
    {
        private SqlExpression @else;
        private List<SqlWhen> whens;

        internal SqlSearchedCase(Type clrType, IEnumerable<SqlWhen> whens, SqlExpression @else, Expression sourceExpression) : base(SqlNodeType.SearchedCase, clrType, sourceExpression)
        {
            if (whens == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("whens");
            }
            this.whens = new List<SqlWhen>(whens);
            if (this.whens.Count == 0)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("whens");
            }
            this.Else = @else;
        }

        internal SqlExpression Else
        {
            get => 
                this.@else;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((this.@else != null) && !this.@else.ClrType.IsAssignableFrom(value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.@else.ClrType, value.ClrType);
                }
                this.@else = value;
            }
        }

        internal override ProviderType SqlType =>
            this.whens[0].Value.SqlType;

        internal List<SqlWhen> Whens =>
            this.whens;
    }
}

