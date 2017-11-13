namespace System.Data.Query.InternalTrees
{
    using System;

    internal class SubTreeId
    {
        private int m_childIndex;
        private int m_hashCode;
        private Node m_parent;
        private int m_parentHashCode;
        public Node m_subTreeRoot;

        internal SubTreeId(RuleProcessingContext context, Node node, Node parent, int childIndex)
        {
            this.m_subTreeRoot = node;
            this.m_parent = parent;
            this.m_childIndex = childIndex;
            this.m_hashCode = context.GetHashCode(node);
            this.m_parentHashCode = (parent == null) ? 0 : context.GetHashCode(parent);
        }

        public override bool Equals(object obj)
        {
            SubTreeId id = obj as SubTreeId;
            if ((id == null) || (this.m_hashCode != id.m_hashCode))
            {
                return false;
            }
            return ((id.m_subTreeRoot == this.m_subTreeRoot) || ((id.m_parent == this.m_parent) && (id.m_childIndex == this.m_childIndex)));
        }

        public override int GetHashCode() => 
            this.m_hashCode;
    }
}

