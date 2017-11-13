namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal abstract class SqlExpression : SqlNode
    {
        private Type clrType;

        internal SqlExpression(SqlNodeType nodeType, Type clrType, Expression sourceExpression) : base(nodeType, sourceExpression)
        {
            this.clrType = clrType;
        }

        internal void SetClrType(Type type)
        {
            this.clrType = type;
        }

        internal Type ClrType =>
            this.clrType;

        internal bool IsConstantColumn
        {
            get
            {
                if (base.NodeType == SqlNodeType.Column)
                {
                    SqlColumn column = (SqlColumn) this;
                    if (column.Expression != null)
                    {
                        return column.Expression.IsConstantColumn;
                    }
                }
                else
                {
                    if (base.NodeType == SqlNodeType.ColumnRef)
                    {
                        return ((SqlColumnRef) this).Column.IsConstantColumn;
                    }
                    if (base.NodeType == SqlNodeType.OptionalValue)
                    {
                        return ((SqlOptionalValue) this).Value.IsConstantColumn;
                    }
                    if ((base.NodeType == SqlNodeType.Value) || (base.NodeType == SqlNodeType.Parameter))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal abstract ProviderType SqlType { get; }
    }
}

