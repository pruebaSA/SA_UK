namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlOrderExpression : IEquatable<SqlOrderExpression>
    {
        private SqlExpression expression;
        private SqlOrderType orderType;

        internal SqlOrderExpression(SqlOrderType type, SqlExpression expr)
        {
            this.OrderType = type;
            this.Expression = expr;
        }

        public bool Equals(SqlOrderExpression other) => 
            (this.EqualsTo(other) || base.Equals(other));

        public override bool Equals(object obj) => 
            (this.EqualsTo(obj as SqlOrderExpression) || base.Equals(obj));

        private bool EqualsTo(SqlOrderExpression other)
        {
            if (other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (this.OrderType != other.OrderType)
            {
                return false;
            }
            if (!this.Expression.SqlType.Equals(other.Expression.SqlType))
            {
                return false;
            }
            SqlColumn column = UnwrapColumn(this.Expression);
            SqlColumn column2 = UnwrapColumn(other.Expression);
            return (((column != null) && (column2 != null)) && (column == column2));
        }

        public override int GetHashCode()
        {
            SqlColumn column = UnwrapColumn(this.Expression);
            if (column != null)
            {
                return column.GetHashCode();
            }
            return base.GetHashCode();
        }

        private static SqlColumn UnwrapColumn(SqlExpression expr)
        {
            if (expr is SqlUnary)
            {
                expr = ((SqlUnary) expr).Operand;
            }
            if (expr is SqlColumn)
            {
                return (expr as SqlColumn);
            }
            if (expr is SqlColumnRef)
            {
                return ((SqlColumnRef) expr).GetRootColumn();
            }
            return null;
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                if ((this.expression != null) && !this.expression.ClrType.IsAssignableFrom(value.ClrType))
                {
                    throw Error.ArgumentWrongType("value", this.expression.ClrType, value.ClrType);
                }
                this.expression = value;
            }
        }

        internal SqlOrderType OrderType
        {
            get => 
                this.orderType;
            set
            {
                this.orderType = value;
            }
        }
    }
}

