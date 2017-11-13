namespace System.Data.Query.InternalTrees
{
    using System;

    internal class ExtendedNodeInfo : NodeInfo
    {
        private VarVec m_definitions;
        private KeyVec m_keys;
        private VarVec m_localDefinitions;
        private RowCount m_maxRows;
        private RowCount m_minRows;

        internal ExtendedNodeInfo(Command cmd) : base(cmd)
        {
            this.m_localDefinitions = cmd.CreateVarVec();
            this.m_definitions = cmd.CreateVarVec();
            this.m_keys = new KeyVec(cmd);
            this.m_minRows = RowCount.Zero;
            this.m_maxRows = RowCount.Unbounded;
        }

        internal override void Clear()
        {
            base.Clear();
            this.m_definitions.Clear();
            this.m_localDefinitions.Clear();
            this.m_keys.Clear();
            this.m_minRows = RowCount.Zero;
            this.m_maxRows = RowCount.Unbounded;
        }

        internal override void ComputeHashValue(Command cmd, Node n)
        {
            base.ComputeHashValue(cmd, n);
            base.m_hashValue = (base.m_hashValue << 4) ^ NodeInfo.GetHashValue(this.Definitions);
            base.m_hashValue = (base.m_hashValue << 4) ^ NodeInfo.GetHashValue(this.Keys.KeyVars);
        }

        internal void InitRowCountFrom(ExtendedNodeInfo source)
        {
            this.m_minRows = source.m_minRows;
            this.m_maxRows = source.m_maxRows;
        }

        internal void SetRowCount(RowCount minRows, RowCount maxRows)
        {
            this.m_minRows = minRows;
            this.m_maxRows = maxRows;
            this.ValidateRowCount();
        }

        private void ValidateRowCount()
        {
        }

        internal VarVec Definitions =>
            this.m_definitions;

        internal KeyVec Keys =>
            this.m_keys;

        internal VarVec LocalDefinitions =>
            this.m_localDefinitions;

        internal RowCount MaxRows
        {
            get => 
                this.m_maxRows;
            set
            {
                this.m_maxRows = value;
                this.ValidateRowCount();
            }
        }

        internal RowCount MinRows
        {
            get => 
                this.m_minRows;
            set
            {
                this.m_minRows = value;
                this.ValidateRowCount();
            }
        }
    }
}

