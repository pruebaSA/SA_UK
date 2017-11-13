namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlGrouping : SqlSimpleTypeExpression
    {
        private SqlExpression group;
        private SqlExpression key;

        internal SqlGrouping(Type clrType, ProviderType sqlType, SqlExpression key, SqlExpression group, Expression sourceExpression) : base(SqlNodeType.Grouping, clrType, sqlType, sourceExpression)
        {
            if (key == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("key");
            }
            if (group == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("group");
            }
            this.key = key;
            this.group = group;
        }

        internal SqlExpression Group
        {
            get => 
                this.group;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if (value.ClrType != this.group.ClrType)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.group.ClrType, value.ClrType);
                }
                this.group = value;
            }
        }

        internal SqlExpression Key
        {
            get => 
                this.key;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if (!this.key.ClrType.IsAssignableFrom(value.ClrType) && !value.ClrType.IsAssignableFrom(this.key.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.key.ClrType, value.ClrType);
                }
                this.key = value;
            }
        }
    }
}

