namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlUserRow : SqlSimpleTypeExpression
    {
        private SqlUserQuery query;
        private MetaType rowType;

        internal SqlUserRow(MetaType rowType, ProviderType sqlType, SqlUserQuery query, Expression source) : base(SqlNodeType.UserRow, rowType.Type, sqlType, source)
        {
            this.Query = query;
            this.rowType = rowType;
        }

        internal SqlUserQuery Query
        {
            get => 
                this.query;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((value.Projection != null) && (value.Projection.ClrType != base.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", base.ClrType, value.Projection.ClrType);
                }
                this.query = value;
            }
        }

        internal MetaType RowType =>
            this.rowType;
    }
}

