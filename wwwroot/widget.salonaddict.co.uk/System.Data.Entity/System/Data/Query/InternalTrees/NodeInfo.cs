namespace System.Data.Query.InternalTrees
{
    using System;

    internal class NodeInfo
    {
        private VarVec m_externalReferences;
        protected int m_hashValue;

        internal NodeInfo(Command cmd)
        {
            this.m_externalReferences = cmd.CreateVarVec();
        }

        internal virtual void Clear()
        {
            this.m_externalReferences.Clear();
            this.m_hashValue = 0;
        }

        internal virtual void ComputeHashValue(Command cmd, Node n)
        {
            this.m_hashValue = 0;
            foreach (Node node in n.Children)
            {
                NodeInfo nodeInfo = cmd.GetNodeInfo(node);
                this.m_hashValue ^= nodeInfo.HashValue;
            }
            this.m_hashValue = (this.m_hashValue << 4) ^ n.Op.OpType;
            this.m_hashValue = (this.m_hashValue << 4) ^ GetHashValue(this.m_externalReferences);
        }

        internal static int GetHashValue(VarVec vec)
        {
            int num = 0;
            foreach (Var var in vec)
            {
                num ^= var.GetHashCode();
            }
            return num;
        }

        internal VarVec ExternalReferences =>
            this.m_externalReferences;

        internal int HashValue =>
            this.m_hashValue;
    }
}

