namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlRow : SqlNode
    {
        private List<SqlColumn> columns;

        internal SqlRow(Expression sourceExpression) : base(SqlNodeType.Row, sourceExpression)
        {
            this.columns = new List<SqlColumn>();
        }

        internal SqlColumn Find(string name)
        {
            foreach (SqlColumn column in this.columns)
            {
                if (name == column.Name)
                {
                    return column;
                }
            }
            return null;
        }

        internal List<SqlColumn> Columns =>
            this.columns;
    }
}

