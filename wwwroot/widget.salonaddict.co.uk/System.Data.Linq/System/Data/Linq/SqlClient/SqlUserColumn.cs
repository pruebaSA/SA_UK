namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlUserColumn : SqlSimpleTypeExpression
    {
        private bool isRequired;
        private string name;
        private SqlUserQuery query;

        internal SqlUserColumn(Type clrType, ProviderType sqlType, SqlUserQuery query, string name, bool isRequired, Expression source) : base(SqlNodeType.UserColumn, clrType, sqlType, source)
        {
            this.Query = query;
            this.name = name;
            this.isRequired = isRequired;
        }

        internal bool IsRequired =>
            this.isRequired;

        internal string Name =>
            this.name;

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
                if ((this.query != null) && (this.query != value))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongValue("value");
                }
                this.query = value;
            }
        }
    }
}

