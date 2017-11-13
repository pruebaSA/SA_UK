namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlInsert : SqlStatement
    {
        private SqlExpression expression;
        private SqlColumn outputKey;
        private bool outputToLocal;
        private SqlRow row;
        private SqlTable table;

        internal SqlInsert(SqlTable table, SqlExpression expr, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.Insert, sourceExpression)
        {
            this.Table = table;
            this.Expression = expr;
            this.Row = new SqlRow(sourceExpression);
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("null");
                }
                if (!this.table.RowType.Type.IsAssignableFrom(value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.table.RowType, value.ClrType);
                }
                this.expression = value;
            }
        }

        internal SqlColumn OutputKey
        {
            get => 
                this.outputKey;
            set
            {
                this.outputKey = value;
            }
        }

        internal bool OutputToLocal
        {
            get => 
                this.outputToLocal;
            set
            {
                this.outputToLocal = value;
            }
        }

        internal SqlRow Row
        {
            get => 
                this.row;
            set
            {
                this.row = value;
            }
        }

        internal SqlTable Table
        {
            get => 
                this.table;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("null");
                }
                this.table = value;
            }
        }
    }
}

