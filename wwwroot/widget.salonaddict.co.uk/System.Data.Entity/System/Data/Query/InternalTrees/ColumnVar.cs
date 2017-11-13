namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class ColumnVar : Var
    {
        private ColumnMD m_columnMetadata;
        private System.Data.Query.InternalTrees.Table m_table;

        internal ColumnVar(int id, System.Data.Query.InternalTrees.Table table, ColumnMD columnMetadata) : base(id, VarType.Column, columnMetadata.Type)
        {
            this.m_table = table;
            this.m_columnMetadata = columnMetadata;
        }

        internal override bool TryGetName(out string name)
        {
            name = this.m_columnMetadata.Name;
            return true;
        }

        internal ColumnMD ColumnMetadata =>
            this.m_columnMetadata;

        internal System.Data.Query.InternalTrees.Table Table =>
            this.m_table;
    }
}

