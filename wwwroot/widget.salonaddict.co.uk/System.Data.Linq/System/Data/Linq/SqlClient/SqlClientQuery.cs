namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlClientQuery : SqlSimpleTypeExpression
    {
        private List<SqlExpression> arguments;
        private int ordinal;
        private List<SqlParameter> parameters;
        private SqlSubSelect query;

        internal SqlClientQuery(SqlSubSelect subquery) : base(SqlNodeType.ClientQuery, subquery.ClrType, subquery.SqlType, subquery.SourceExpression)
        {
            this.query = subquery;
            this.arguments = new List<SqlExpression>();
            this.parameters = new List<SqlParameter>();
        }

        internal List<SqlExpression> Arguments =>
            this.arguments;

        internal int Ordinal
        {
            get => 
                this.ordinal;
            set
            {
                this.ordinal = value;
            }
        }

        internal List<SqlParameter> Parameters =>
            this.parameters;

        internal SqlSubSelect Query
        {
            get => 
                this.query;
            set
            {
                if ((value == null) || ((this.query != null) && (this.query.ClrType != value.ClrType)))
                {
                    throw Error.ArgumentWrongType(value, this.query.ClrType, value.ClrType);
                }
                this.query = value;
            }
        }
    }
}

