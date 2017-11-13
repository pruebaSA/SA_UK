namespace System.Data.Common
{
    using System;
    using System.Data;

    internal sealed class DbSchemaTable
    {
        private bool _returnProviderSpecificTypes;
        private DataColumn[] columnCache = new DataColumn[DBCOLUMN_NAME.Length];
        private DataColumnCollection columns;
        internal DataTable dataTable;
        private static readonly string[] DBCOLUMN_NAME = new string[] { 
            SchemaTableColumn.ColumnName, SchemaTableColumn.ColumnOrdinal, SchemaTableColumn.ColumnSize, SchemaTableOptionalColumn.BaseServerName, SchemaTableOptionalColumn.BaseCatalogName, SchemaTableColumn.BaseColumnName, SchemaTableColumn.BaseSchemaName, SchemaTableColumn.BaseTableName, SchemaTableOptionalColumn.IsAutoIncrement, SchemaTableColumn.IsUnique, SchemaTableColumn.IsKey, SchemaTableOptionalColumn.IsRowVersion, SchemaTableColumn.DataType, SchemaTableOptionalColumn.ProviderSpecificDataType, SchemaTableColumn.AllowDBNull, SchemaTableColumn.ProviderType,
            SchemaTableColumn.IsExpression, SchemaTableOptionalColumn.IsHidden, SchemaTableColumn.IsLong, SchemaTableOptionalColumn.IsReadOnly, "SchemaMapping Unsorted Index"
        };

        internal DbSchemaTable(DataTable dataTable, bool returnProviderSpecificTypes)
        {
            this.dataTable = dataTable;
            this.columns = dataTable.Columns;
            this._returnProviderSpecificTypes = returnProviderSpecificTypes;
        }

        private DataColumn CachedDataColumn(ColumnEnum column) => 
            this.CachedDataColumn(column, column);

        private DataColumn CachedDataColumn(ColumnEnum column, ColumnEnum column2)
        {
            DataColumn column3 = this.columnCache[(int) column];
            if (column3 == null)
            {
                int index = this.columns.IndexOf(DBCOLUMN_NAME[(int) column]);
                if ((-1 == index) && (column != column2))
                {
                    index = this.columns.IndexOf(DBCOLUMN_NAME[(int) column2]);
                }
                if (-1 != index)
                {
                    column3 = this.columns[index];
                    this.columnCache[(int) column] = column3;
                }
            }
            return column3;
        }

        internal DataColumn AllowDBNull =>
            this.CachedDataColumn(ColumnEnum.AllowDBNull);

        internal DataColumn BaseCatalogName =>
            this.CachedDataColumn(ColumnEnum.BaseCatalogName);

        internal DataColumn BaseColumnName =>
            this.CachedDataColumn(ColumnEnum.BaseColumnName);

        internal DataColumn BaseSchemaName =>
            this.CachedDataColumn(ColumnEnum.BaseSchemaName);

        internal DataColumn BaseServerName =>
            this.CachedDataColumn(ColumnEnum.BaseServerName);

        internal DataColumn BaseTableName =>
            this.CachedDataColumn(ColumnEnum.BaseTableName);

        internal DataColumn ColumnName =>
            this.CachedDataColumn(ColumnEnum.ColumnName);

        internal DataColumn DataType
        {
            get
            {
                if (this._returnProviderSpecificTypes)
                {
                    return this.CachedDataColumn(ColumnEnum.ProviderSpecificDataType, ColumnEnum.DataType);
                }
                return this.CachedDataColumn(ColumnEnum.DataType);
            }
        }

        internal DataColumn IsAutoIncrement =>
            this.CachedDataColumn(ColumnEnum.IsAutoIncrement);

        internal DataColumn IsExpression =>
            this.CachedDataColumn(ColumnEnum.IsExpression);

        internal DataColumn IsHidden =>
            this.CachedDataColumn(ColumnEnum.IsHidden);

        internal DataColumn IsKey =>
            this.CachedDataColumn(ColumnEnum.IsKey);

        internal DataColumn IsLong =>
            this.CachedDataColumn(ColumnEnum.IsLong);

        internal DataColumn IsReadOnly =>
            this.CachedDataColumn(ColumnEnum.IsReadOnly);

        internal DataColumn IsRowVersion =>
            this.CachedDataColumn(ColumnEnum.IsRowVersion);

        internal DataColumn IsUnique =>
            this.CachedDataColumn(ColumnEnum.IsUnique);

        internal DataColumn Size =>
            this.CachedDataColumn(ColumnEnum.ColumnSize);

        internal DataColumn UnsortedIndex =>
            this.CachedDataColumn(ColumnEnum.SchemaMappingUnsortedIndex);

        private enum ColumnEnum
        {
            ColumnName,
            ColumnOrdinal,
            ColumnSize,
            BaseServerName,
            BaseCatalogName,
            BaseColumnName,
            BaseSchemaName,
            BaseTableName,
            IsAutoIncrement,
            IsUnique,
            IsKey,
            IsRowVersion,
            DataType,
            ProviderSpecificDataType,
            AllowDBNull,
            ProviderType,
            IsExpression,
            IsHidden,
            IsLong,
            IsReadOnly,
            SchemaMappingUnsortedIndex
        }
    }
}

