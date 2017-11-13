namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal sealed class AugmentedTableNode : AugmentedNode
    {
        private List<JoinEdge> m_joinEdges;
        private int m_lastVisibleId;
        private int m_newLocationId;
        private VarVec m_nullableColumns;
        private AugmentedTableNode m_replacementTable;
        private System.Data.Query.InternalTrees.Table m_table;

        internal AugmentedTableNode(int id, Node node) : base(id, node)
        {
            ScanTableOp op = (ScanTableOp) node.Op;
            this.m_table = op.Table;
            this.m_joinEdges = new List<JoinEdge>();
            this.m_lastVisibleId = id;
            this.m_replacementTable = this;
            this.m_newLocationId = id;
        }

        internal bool IsEliminated =>
            (this.m_replacementTable != this);

        internal bool IsMoved =>
            (this.m_newLocationId != base.Id);

        internal List<JoinEdge> JoinEdges =>
            this.m_joinEdges;

        internal int LastVisibleId
        {
            get => 
                this.m_lastVisibleId;
            set
            {
                this.m_lastVisibleId = value;
            }
        }

        internal int NewLocationId
        {
            get => 
                this.m_newLocationId;
            set
            {
                this.m_newLocationId = value;
            }
        }

        internal VarVec NullableColumns
        {
            get => 
                this.m_nullableColumns;
            set
            {
                this.m_nullableColumns = value;
            }
        }

        internal AugmentedTableNode ReplacementTable
        {
            get => 
                this.m_replacementTable;
            set
            {
                this.m_replacementTable = value;
            }
        }

        internal System.Data.Query.InternalTrees.Table Table =>
            this.m_table;
    }
}

