namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal abstract class CellTreeNode : InternalBase
    {
        private System.Data.Mapping.ViewGeneration.CellNormalizer m_normalizer;

        protected CellTreeNode(System.Data.Mapping.ViewGeneration.CellNormalizer normalizer)
        {
            this.m_normalizer = normalizer;
        }

        internal abstract TOutput Accept<TInput, TOutput>(CellTreeVisitor<TInput, TOutput> visitor, TInput param);
        internal abstract TOutput Accept<TInput, TOutput>(SimpleCellTreeVisitor<TInput, TOutput> visitor, TInput param);
        internal CellTreeNode AssociativeFlatten() => 
            AssociativeOpFlatteningVisitor.Flatten(this);

        protected int BoolIndexToSlot(int boolIndex) => 
            ProjectedSlot.BoolIndexToSlot(boolIndex, this.ProjectedSlotMap, this.NumBoolSlots);

        internal CellTreeNode Flatten() => 
            FlatteningVisitor.Flatten(this);

        internal List<LeftCellWrapper> GetLeaves() => 
            LeafVisitor.GetLeaves(this);

        protected MemberPath GetMemberPath(int slotNum) => 
            ProjectedSlot.GetMemberPath(slotNum, this.ProjectedSlotMap, this.NumBoolSlots);

        internal bool[] GetProjectedSlots()
        {
            int num = this.ProjectedSlotMap.Count + this.NumBoolSlots;
            bool[] flagArray = new bool[num];
            for (int i = 0; i < num; i++)
            {
                flagArray[i] = this.IsProjectedSlot(i);
            }
            return flagArray;
        }

        internal static bool IsAssociativeOp(CellTreeOpType opType)
        {
            if ((opType != CellTreeOpType.IJ) && (opType != CellTreeOpType.Union))
            {
                return (opType == CellTreeOpType.FOJ);
            }
            return true;
        }

        protected bool IsBoolSlot(int slotNum) => 
            ProjectedSlot.IsBoolSlot(slotNum, this.ProjectedSlotMap, this.NumBoolSlots);

        protected bool IsKeySlot(int slotNum) => 
            ProjectedSlot.IsKeySlot(slotNum, this.ProjectedSlotMap, this.NumBoolSlots);

        internal abstract bool IsProjectedSlot(int slot);
        internal CellTreeNode MakeCopy()
        {
            DefaultCellTreeVisitor<bool> visitor = new DefaultCellTreeVisitor<bool>();
            return this.Accept<bool, CellTreeNode>(visitor, true);
        }

        protected int SlotToBoolIndex(int slotNum) => 
            ProjectedSlot.SlotToBoolIndex(slotNum, this.ProjectedSlotMap, this.NumBoolSlots);

        internal abstract CqlBlock ToCqlBlock(bool[] requiredSlots, CqlIdentifiers identifiers, ref int blockAliasNum, ref List<WithStatement> withStatements);
        internal override void ToFullString(StringBuilder builder)
        {
            int blockAliasNum = 0;
            bool[] projectedSlots = this.GetProjectedSlots();
            CqlIdentifiers identifiers = new CqlIdentifiers();
            List<WithStatement> withStatements = new List<WithStatement>();
            this.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withStatements).AsCql(builder, false, 1);
        }

        internal abstract Set<MemberPath> Attributes { get; }

        internal System.Data.Mapping.ViewGeneration.CellNormalizer CellNormalizer =>
            this.m_normalizer;

        internal abstract List<CellTreeNode> Children { get; }

        internal bool IsEmptyRightFragmentQuery =>
            !this.m_normalizer.RightFragmentQP.IsSatisfiable(this.RightFragmentQuery);

        protected IEnumerable<int> KeySlots
        {
            get
            {
                int count = this.ProjectedSlotMap.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this.IsKeySlot(i))
                    {
                        yield return i;
                    }
                }
            }
        }

        internal abstract FragmentQuery LeftFragmentQuery { get; }

        internal abstract int NumBoolSlots { get; }

        internal abstract int NumProjectedSlots { get; }

        internal abstract CellTreeOpType OpType { get; }

        internal MemberPathMapBase ProjectedSlotMap =>
            this.m_normalizer.MemberMaps.ProjectedSlotMap;

        internal abstract MemberDomainMap RightDomainMap { get; }

        internal abstract FragmentQuery RightFragmentQuery { get; }


        private class AssociativeOpFlatteningVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>
        {
            private AssociativeOpFlatteningVisitor()
            {
            }

            internal static CellTreeNode Flatten(CellTreeNode node)
            {
                CellTreeNode node2 = CellTreeNode.FlatteningVisitor.Flatten(node);
                CellTreeNode.AssociativeOpFlatteningVisitor visitor = new CellTreeNode.AssociativeOpFlatteningVisitor();
                return node2.Accept<bool, CellTreeNode>(visitor, true);
            }

            internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, bool dummy) => 
                node;

            internal override CellTreeNode VisitOpNode(OpCellTreeNode node, bool dummy)
            {
                List<CellTreeNode> list = new List<CellTreeNode>();
                foreach (CellTreeNode node2 in node.Children)
                {
                    CellTreeNode item = node2.Accept<bool, CellTreeNode>(this, dummy);
                    list.Add(item);
                }
                List<CellTreeNode> children = list;
                if (CellTreeNode.IsAssociativeOp(node.OpType))
                {
                    children = new List<CellTreeNode>();
                    foreach (CellTreeNode node4 in list)
                    {
                        if (node4.OpType == node.OpType)
                        {
                            children.AddRange(node4.Children);
                        }
                        else
                        {
                            children.Add(node4);
                        }
                    }
                }
                return new OpCellTreeNode(node.CellNormalizer, node.OpType, children);
            }
        }

        internal abstract class CellTreeVisitor<TInput, TOutput>
        {
            protected CellTreeVisitor()
            {
            }

            internal abstract TOutput VisitFullOuterJoin(OpCellTreeNode node, TInput param);
            internal abstract TOutput VisitInnerJoin(OpCellTreeNode node, TInput param);
            internal abstract TOutput VisitLeaf(LeafCellTreeNode node, TInput param);
            internal abstract TOutput VisitLeftAntiSemiJoin(OpCellTreeNode node, TInput param);
            internal abstract TOutput VisitLeftOuterJoin(OpCellTreeNode node, TInput param);
            internal abstract TOutput VisitUnion(OpCellTreeNode node, TInput param);
        }

        private class DefaultCellTreeVisitor<TInput> : CellTreeNode.CellTreeVisitor<TInput, CellTreeNode>
        {
            private OpCellTreeNode AcceptChildren(OpCellTreeNode node, TInput param)
            {
                List<CellTreeNode> children = new List<CellTreeNode>();
                foreach (CellTreeNode node2 in node.Children)
                {
                    children.Add(node2.Accept<TInput, CellTreeNode>(this, param));
                }
                return new OpCellTreeNode(node.CellNormalizer, node.OpType, children);
            }

            internal override CellTreeNode VisitFullOuterJoin(OpCellTreeNode node, TInput param) => 
                this.AcceptChildren(node, param);

            internal override CellTreeNode VisitInnerJoin(OpCellTreeNode node, TInput param) => 
                this.AcceptChildren(node, param);

            internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, TInput param) => 
                node;

            internal override CellTreeNode VisitLeftAntiSemiJoin(OpCellTreeNode node, TInput param) => 
                this.AcceptChildren(node, param);

            internal override CellTreeNode VisitLeftOuterJoin(OpCellTreeNode node, TInput param) => 
                this.AcceptChildren(node, param);

            internal override CellTreeNode VisitUnion(OpCellTreeNode node, TInput param) => 
                this.AcceptChildren(node, param);
        }

        private class FlatteningVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>
        {
            protected FlatteningVisitor()
            {
            }

            internal static CellTreeNode Flatten(CellTreeNode node)
            {
                CellTreeNode.FlatteningVisitor visitor = new CellTreeNode.FlatteningVisitor();
                return node.Accept<bool, CellTreeNode>(visitor, true);
            }

            internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, bool dummy) => 
                node;

            internal override CellTreeNode VisitOpNode(OpCellTreeNode node, bool dummy)
            {
                List<CellTreeNode> children = new List<CellTreeNode>();
                foreach (CellTreeNode node2 in node.Children)
                {
                    CellTreeNode item = node2.Accept<bool, CellTreeNode>(this, dummy);
                    children.Add(item);
                }
                if (children.Count == 1)
                {
                    return children[0];
                }
                return new OpCellTreeNode(node.CellNormalizer, node.OpType, children);
            }
        }

        private class LeafVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, IEnumerable<LeftCellWrapper>>
        {
            private LeafVisitor()
            {
            }

            internal static List<LeftCellWrapper> GetLeaves(CellTreeNode node)
            {
                CellTreeNode.LeafVisitor visitor = new CellTreeNode.LeafVisitor();
                return new List<LeftCellWrapper>(node.Accept<bool, IEnumerable<LeftCellWrapper>>(visitor, true));
            }

            internal override IEnumerable<LeftCellWrapper> VisitLeaf(LeafCellTreeNode node, bool dummy)
            {
                yield return node.LeftCellWrapper;
            }

            internal override IEnumerable<LeftCellWrapper> VisitOpNode(OpCellTreeNode node, bool dummy)
            {
                foreach (CellTreeNode iteratorVariable0 in node.Children)
                {
                    IEnumerable<LeftCellWrapper> iteratorVariable1 = iteratorVariable0.Accept<bool, IEnumerable<LeftCellWrapper>>(this, dummy);
                    foreach (LeftCellWrapper iteratorVariable2 in iteratorVariable1)
                    {
                        yield return iteratorVariable2;
                    }
                }
            }


        }

        internal abstract class SimpleCellTreeVisitor<TInput, TOutput>
        {
            protected SimpleCellTreeVisitor()
            {
            }

            internal abstract TOutput VisitLeaf(LeafCellTreeNode node, TInput param);
            internal abstract TOutput VisitOpNode(OpCellTreeNode node, TInput param);
        }
    }
}

