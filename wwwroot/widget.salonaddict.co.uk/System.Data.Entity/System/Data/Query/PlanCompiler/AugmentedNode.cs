namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class AugmentedNode
    {
        private List<AugmentedNode> m_children;
        private int m_id;
        private System.Data.Query.InternalTrees.Node m_node;
        protected AugmentedNode m_parent;

        internal AugmentedNode(int id, System.Data.Query.InternalTrees.Node node) : this(id, node, new List<AugmentedNode>())
        {
        }

        internal AugmentedNode(int id, System.Data.Query.InternalTrees.Node node, List<AugmentedNode> children)
        {
            this.m_id = id;
            this.m_node = node;
            this.m_children = children;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(children != null, "null children (gasp!)");
            foreach (AugmentedNode node2 in this.m_children)
            {
                node2.m_parent = this;
            }
        }

        internal List<AugmentedNode> Children =>
            this.m_children;

        internal int Id =>
            this.m_id;

        internal System.Data.Query.InternalTrees.Node Node =>
            this.m_node;

        internal AugmentedNode Parent =>
            this.m_parent;
    }
}

