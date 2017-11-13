namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal sealed class AugmentedJoinNode : AugmentedNode
    {
        private List<ColumnVar> m_leftVars;
        private Node m_otherPredicate;
        private List<ColumnVar> m_rightVars;

        internal AugmentedJoinNode(int id, Node node, List<AugmentedNode> children) : base(id, node, children)
        {
            this.m_leftVars = new List<ColumnVar>();
            this.m_rightVars = new List<ColumnVar>();
        }

        internal AugmentedJoinNode(int id, Node node, AugmentedNode leftChild, AugmentedNode rightChild, List<ColumnVar> leftVars, List<ColumnVar> rightVars, Node otherPredicate) : this(id, node, new List<AugmentedNode>(new AugmentedNode[] { leftChild, rightChild }))
        {
            this.m_otherPredicate = otherPredicate;
            this.m_rightVars = rightVars;
            this.m_leftVars = leftVars;
        }

        internal List<ColumnVar> LeftVars =>
            this.m_leftVars;

        internal Node OtherPredicate =>
            this.m_otherPredicate;

        internal List<ColumnVar> RightVars =>
            this.m_rightVars;
    }
}

