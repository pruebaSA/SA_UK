namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlTableValuedFunctionCall : SqlFunctionCall
    {
        private List<SqlColumn> columns;
        private MetaType rowType;

        internal SqlTableValuedFunctionCall(MetaType rowType, Type clrType, ProviderType sqlType, string name, IEnumerable<SqlExpression> args, Expression source) : base(SqlNodeType.TableValuedFunctionCall, clrType, sqlType, name, args, source)
        {
            this.rowType = rowType;
            this.columns = new List<SqlColumn>();
        }

        internal SqlColumn Find(string name)
        {
            foreach (SqlColumn column in this.Columns)
            {
                if (column.Name == name)
                {
                    return column;
                }
            }
            return null;
        }

        internal List<SqlColumn> Columns =>
            this.columns;

        internal MetaType RowType =>
            this.rowType;
    }
}

