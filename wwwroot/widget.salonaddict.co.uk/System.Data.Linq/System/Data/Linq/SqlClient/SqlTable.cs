namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlTable : SqlNode
    {
        private List<SqlColumn> columns;
        private MetaType rowType;
        private ProviderType sqlRowType;
        private System.Data.Linq.Mapping.MetaTable table;

        internal SqlTable(System.Data.Linq.Mapping.MetaTable table, MetaType rowType, ProviderType sqlRowType, Expression sourceExpression) : base(SqlNodeType.Table, sourceExpression)
        {
            this.table = table;
            this.rowType = rowType;
            this.sqlRowType = sqlRowType;
            this.columns = new List<SqlColumn>();
        }

        internal SqlColumn Find(string columnName)
        {
            foreach (SqlColumn column in this.Columns)
            {
                if (column.Name == columnName)
                {
                    return column;
                }
            }
            return null;
        }

        internal List<SqlColumn> Columns =>
            this.columns;

        internal System.Data.Linq.Mapping.MetaTable MetaTable =>
            this.table;

        internal string Name =>
            this.table.TableName;

        internal MetaType RowType =>
            this.rowType;

        internal ProviderType SqlRowType =>
            this.sqlRowType;
    }
}

