namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;

    internal class Node
    {
        private List<Node> m_children;
        private int m_id;
        private NodeInfo m_nodeInfo;
        private System.Data.Query.InternalTrees.Op m_op;

        internal Node(System.Data.Query.InternalTrees.Op op, params Node[] children) : this(-1, op, new List<Node>(children))
        {
        }

        internal Node(int nodeId, System.Data.Query.InternalTrees.Op op, List<Node> children)
        {
            this.m_id = nodeId;
            this.m_op = op;
            this.m_children = children;
        }

        internal ExtendedNodeInfo GetExtendedNodeInfo(Command command)
        {
            if (this.m_nodeInfo == null)
            {
                this.InitializeNodeInfo(command);
            }
            return (this.m_nodeInfo as ExtendedNodeInfo);
        }

        internal NodeInfo GetNodeInfo(Command command)
        {
            if (this.m_nodeInfo == null)
            {
                this.InitializeNodeInfo(command);
            }
            return this.m_nodeInfo;
        }

        private void InitializeNodeInfo(Command command)
        {
            if (this.Op.IsRelOp || this.Op.IsPhysicalOp)
            {
                this.m_nodeInfo = new ExtendedNodeInfo(command);
            }
            else
            {
                this.m_nodeInfo = new NodeInfo(command);
            }
            command.RecomputeNodeInfo(this);
        }

        internal bool IsEquivalent(Node other)
        {
            if (this.Children.Count != other.Children.Count)
            {
                return false;
            }
            if (this.Op.IsEquivalent(other.Op) != true)
            {
                return false;
            }
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (!this.Children[i].IsEquivalent(other.Children[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal Node Child0
        {
            get => 
                this.m_children[0];
            set
            {
                this.m_children[0] = value;
            }
        }

        internal Node Child1
        {
            get => 
                this.m_children[1];
            set
            {
                this.m_children[1] = value;
            }
        }

        internal Node Child2
        {
            get => 
                this.m_children[2];
            set
            {
                this.m_children[2] = value;
            }
        }

        internal List<Node> Children =>
            this.m_children;

        internal bool HasChild0 =>
            (this.m_children.Count > 0);

        internal bool HasChild1 =>
            (this.m_children.Count > 1);

        internal bool HasChild2 =>
            (this.m_children.Count > 2);

        internal System.Data.Query.InternalTrees.Op Op
        {
            get => 
                this.m_op;
            set
            {
                this.m_op = value;
            }
        }
    }
}

