namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal class Table
    {
        private VarList m_columns;
        private VarVec m_keys;
        private VarVec m_referencedColumns;
        private int m_tableId;
        private TableMD m_tableMetadata;

        internal Table(Command command, TableMD tableMetadata, int tableId)
        {
            this.m_tableMetadata = tableMetadata;
            this.m_columns = Command.CreateVarList();
            this.m_keys = command.CreateVarVec();
            this.m_tableId = tableId;
            Dictionary<string, ColumnVar> dictionary = new Dictionary<string, ColumnVar>();
            foreach (ColumnMD nmd in tableMetadata.Columns)
            {
                ColumnVar var = command.CreateColumnVar(this, nmd);
                dictionary[nmd.Name] = var;
            }
            foreach (ColumnMD nmd2 in tableMetadata.Keys)
            {
                ColumnVar v = dictionary[nmd2.Name];
                this.m_keys.Set(v);
            }
            this.m_referencedColumns = command.CreateVarVec(this.m_columns);
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}::{1}", new object[] { this.m_tableMetadata.ToString(), this.TableId });

        internal VarList Columns =>
            this.m_columns;

        internal VarVec Keys =>
            this.m_keys;

        internal VarVec ReferencedColumns =>
            this.m_referencedColumns;

        internal int TableId =>
            this.m_tableId;

        internal TableMD TableMetadata =>
            this.m_tableMetadata;
    }
}

