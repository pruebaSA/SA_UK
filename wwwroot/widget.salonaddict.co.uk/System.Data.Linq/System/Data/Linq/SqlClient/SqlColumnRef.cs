namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlColumnRef : SqlExpression
    {
        private SqlColumn column;

        internal SqlColumnRef(SqlColumn col) : base(SqlNodeType.ColumnRef, col.ClrType, col.SourceExpression)
        {
            this.column = col;
        }

        public override bool Equals(object obj)
        {
            SqlColumnRef ref2 = obj as SqlColumnRef;
            return ((ref2 != null) && (ref2.Column == this.column));
        }

        public override int GetHashCode() => 
            this.column.GetHashCode();

        internal SqlColumn GetRootColumn()
        {
            SqlColumn column = this.column;
            while ((column.Expression != null) && (column.Expression.NodeType == SqlNodeType.ColumnRef))
            {
                column = ((SqlColumnRef) column.Expression).Column;
            }
            return column;
        }

        internal SqlColumn Column =>
            this.column;

        internal override ProviderType SqlType =>
            this.column.SqlType;
    }
}

