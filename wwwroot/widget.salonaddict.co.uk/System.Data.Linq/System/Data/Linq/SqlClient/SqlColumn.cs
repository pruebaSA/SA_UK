namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlColumn : SqlExpression
    {
        private SqlAlias alias;
        private SqlExpression expression;
        private MetaDataMember member;
        private string name;
        private int ordinal;
        private ProviderType sqlType;

        internal SqlColumn(string name, SqlExpression expr) : this(expr.ClrType, expr.SqlType, name, null, expr, expr.SourceExpression)
        {
        }

        internal SqlColumn(Type clrType, ProviderType sqlType, string name, MetaDataMember member, SqlExpression expr, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.Column, clrType, sourceExpression)
        {
            if (typeof(Type).IsAssignableFrom(clrType))
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentWrongValue("clrType");
            }
            this.Name = name;
            this.member = member;
            this.Expression = expr;
            this.Ordinal = -1;
            if (sqlType == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("sqlType");
            }
            this.sqlType = sqlType;
        }

        internal SqlAlias Alias
        {
            get => 
                this.alias;
            set
            {
                this.alias = value;
            }
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if (value != null)
                {
                    if (!base.ClrType.IsAssignableFrom(value.ClrType))
                    {
                        throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", base.ClrType, value.ClrType);
                    }
                    SqlColumnRef ref2 = value as SqlColumnRef;
                    if ((ref2 != null) && (ref2.Column == this))
                    {
                        throw System.Data.Linq.SqlClient.Error.ColumnCannotReferToItself();
                    }
                }
                this.expression = value;
            }
        }

        internal MetaDataMember MetaMember =>
            this.member;

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal int Ordinal
        {
            get => 
                this.ordinal;
            set
            {
                this.ordinal = value;
            }
        }

        internal override ProviderType SqlType
        {
            get
            {
                if (this.expression != null)
                {
                    return this.expression.SqlType;
                }
                return this.sqlType;
            }
        }
    }
}

