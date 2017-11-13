namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class JoinEdge
    {
        private System.Data.Query.PlanCompiler.JoinKind m_joinKind;
        private AugmentedJoinNode m_joinNode;
        private AugmentedTableNode m_left;
        private List<ColumnVar> m_leftVars;
        private AugmentedTableNode m_right;
        private List<ColumnVar> m_rightVars;

        private JoinEdge(AugmentedTableNode left, AugmentedTableNode right, AugmentedJoinNode joinNode, System.Data.Query.PlanCompiler.JoinKind joinKind, List<ColumnVar> leftVars, List<ColumnVar> rightVars)
        {
            this.m_left = left;
            this.m_right = right;
            this.m_joinKind = joinKind;
            this.m_joinNode = joinNode;
            this.m_leftVars = leftVars;
            this.m_rightVars = rightVars;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_leftVars.Count == this.m_rightVars.Count, string.Concat(new object[] { "Count mismatch: ", this.m_leftVars.Count, ",", this.m_rightVars.Count }));
        }

        internal bool AddCondition(AugmentedJoinNode joinNode, ColumnVar leftVar, ColumnVar rightVar)
        {
            if (joinNode != this.m_joinNode)
            {
                return false;
            }
            this.m_leftVars.Add(leftVar);
            this.m_rightVars.Add(rightVar);
            return true;
        }

        internal static JoinEdge CreateJoinEdge(AugmentedTableNode left, AugmentedTableNode right, AugmentedJoinNode joinNode, ColumnVar leftVar, ColumnVar rightVar)
        {
            List<ColumnVar> leftVars = new List<ColumnVar>();
            List<ColumnVar> rightVars = new List<ColumnVar>();
            leftVars.Add(leftVar);
            rightVars.Add(rightVar);
            OpType opType = joinNode.Node.Op.OpType;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((opType == OpType.LeftOuterJoin) || (opType == OpType.InnerJoin), "Unexpected join type for join edge: " + opType);
            return new JoinEdge(left, right, joinNode, (opType == OpType.LeftOuterJoin) ? System.Data.Query.PlanCompiler.JoinKind.LeftOuter : System.Data.Query.PlanCompiler.JoinKind.Inner, leftVars, rightVars);
        }

        internal static JoinEdge CreateTransitiveJoinEdge(AugmentedTableNode left, AugmentedTableNode right, System.Data.Query.PlanCompiler.JoinKind joinKind, List<ColumnVar> leftVars, List<ColumnVar> rightVars) => 
            new JoinEdge(left, right, null, joinKind, leftVars, rightVars);

        internal bool IsEliminated
        {
            get
            {
                if (!this.Left.IsEliminated)
                {
                    return this.Right.IsEliminated;
                }
                return true;
            }
        }

        internal System.Data.Query.PlanCompiler.JoinKind JoinKind =>
            this.m_joinKind;

        internal AugmentedJoinNode JoinNode =>
            this.m_joinNode;

        internal AugmentedTableNode Left =>
            this.m_left;

        internal List<ColumnVar> LeftVars =>
            this.m_leftVars;

        internal AugmentedTableNode Right =>
            this.m_right;

        internal List<ColumnVar> RightVars =>
            this.m_rightVars;
    }
}

