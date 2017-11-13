namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlTypeCase : SqlExpression
    {
        private SqlExpression discriminator;
        private MetaType rowType;
        private ProviderType sqlType;
        private List<SqlTypeCaseWhen> whens;

        internal SqlTypeCase(Type clrType, ProviderType sqlType, MetaType rowType, SqlExpression discriminator, IEnumerable<SqlTypeCaseWhen> whens, Expression sourceExpression) : base(SqlNodeType.TypeCase, clrType, sourceExpression)
        {
            this.whens = new List<SqlTypeCaseWhen>();
            this.Discriminator = discriminator;
            if (whens == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("whens");
            }
            this.whens.AddRange(whens);
            if (this.whens.Count == 0)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("whens");
            }
            this.sqlType = sqlType;
            this.rowType = rowType;
        }

        internal SqlExpression Discriminator
        {
            get => 
                this.discriminator;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((this.discriminator != null) && (this.discriminator.ClrType != value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.discriminator.ClrType, value.ClrType);
                }
                this.discriminator = value;
            }
        }

        internal MetaType RowType =>
            this.rowType;

        internal override ProviderType SqlType =>
            this.sqlType;

        internal List<SqlTypeCaseWhen> Whens =>
            this.whens;
    }
}

