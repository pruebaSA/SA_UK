namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlUserQuery : SqlNode
    {
        private List<SqlExpression> args;
        private List<SqlUserColumn> columns;
        private SqlExpression projection;
        private string queryText;

        internal SqlUserQuery(SqlNodeType nt, SqlExpression projection, IEnumerable<SqlExpression> args, Expression source) : base(nt, source)
        {
            this.Projection = projection;
            this.args = (args != null) ? new List<SqlExpression>(args) : new List<SqlExpression>();
            this.columns = new List<SqlUserColumn>();
        }

        internal SqlUserQuery(string queryText, SqlExpression projection, IEnumerable<SqlExpression> args, Expression source) : base(SqlNodeType.UserQuery, source)
        {
            this.queryText = queryText;
            this.Projection = projection;
            this.args = (args != null) ? new List<SqlExpression>(args) : new List<SqlExpression>();
            this.columns = new List<SqlUserColumn>();
        }

        internal SqlUserColumn Find(string name)
        {
            foreach (SqlUserColumn column in this.Columns)
            {
                if (column.Name == name)
                {
                    return column;
                }
            }
            return null;
        }

        internal List<SqlExpression> Arguments =>
            this.args;

        internal List<SqlUserColumn> Columns =>
            this.columns;

        internal SqlExpression Projection
        {
            get => 
                this.projection;
            set
            {
                if ((this.projection != null) && (this.projection.ClrType != value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.projection.ClrType, value.ClrType);
                }
                this.projection = value;
            }
        }

        internal string QueryText =>
            this.queryText;
    }
}

