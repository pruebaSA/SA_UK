namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    internal class NodeInfoVisitor : BasicOpVisitorOfT<NodeInfo>
    {
        private Command m_command;

        internal NodeInfoVisitor(Command command)
        {
            this.m_command = command;
        }

        internal NodeInfo ComputeNodeInfo(Node n)
        {
            NodeInfo info = base.VisitNode(n);
            info.ComputeHashValue(this.m_command, n);
            return info;
        }

        internal static Dictionary<Var, Var> ComputeVarRemappings(Node varDefListNode)
        {
            Dictionary<Var, Var> dictionary = new Dictionary<Var, Var>();
            foreach (Node node in varDefListNode.Children)
            {
                VarRefOp op = node.Child0.Op as VarRefOp;
                if (op != null)
                {
                    VarDefOp op2 = node.Op as VarDefOp;
                    dictionary[op.Var] = op2.Var;
                }
            }
            return dictionary;
        }

        private ExtendedNodeInfo GetExtendedNodeInfo(Node n) => 
            n.GetExtendedNodeInfo(this.m_command);

        private NodeInfo GetNodeInfo(Node n) => 
            n.GetNodeInfo(this.m_command);

        private ExtendedNodeInfo InitExtendedNodeInfo(Node n)
        {
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n);
            extendedNodeInfo.Clear();
            return extendedNodeInfo;
        }

        private NodeInfo InitNodeInfo(Node n)
        {
            NodeInfo nodeInfo = this.GetNodeInfo(n);
            nodeInfo.Clear();
            return nodeInfo;
        }

        public override NodeInfo Visit(CrossJoinOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            List<KeyVec> keyVecList = new List<KeyVec>();
            RowCount zero = RowCount.Zero;
            RowCount one = RowCount.One;
            foreach (Node node in n.Children)
            {
                ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(node);
                info.Definitions.Or(extendedNodeInfo.Definitions);
                info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
                keyVecList.Add(extendedNodeInfo.Keys);
                if (extendedNodeInfo.MaxRows > zero)
                {
                    zero = extendedNodeInfo.MaxRows;
                }
                if (extendedNodeInfo.MinRows < one)
                {
                    one = extendedNodeInfo.MinRows;
                }
            }
            info.Keys.InitFrom(keyVecList);
            info.SetRowCount(one, zero);
            return info;
        }

        public override NodeInfo Visit(DistinctOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            info.Definitions.InitFrom(op.Keys);
            info.Keys.InitFrom(op.Keys, true);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            info.ExternalReferences.InitFrom(extendedNodeInfo.ExternalReferences);
            info.InitRowCountFrom(extendedNodeInfo);
            return info;
        }

        public override NodeInfo Visit(FilterOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            NodeInfo nodeInfo = this.GetNodeInfo(n.Child1);
            info.Definitions.Or(extendedNodeInfo.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            info.ExternalReferences.Minus(extendedNodeInfo.Definitions);
            info.Keys.InitFrom(extendedNodeInfo.Keys);
            info.MinRows = RowCount.Zero;
            ConstantPredicateOp op2 = n.Child1.Op as ConstantPredicateOp;
            if ((op2 != null) && op2.IsFalse)
            {
                info.MaxRows = RowCount.Zero;
                return info;
            }
            info.MaxRows = extendedNodeInfo.MaxRows;
            return info;
        }

        public override NodeInfo Visit(GroupByOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            info.Definitions.InitFrom(op.Outputs);
            info.LocalDefinitions.InitFrom(info.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            foreach (Node node in n.Child1.Children)
            {
                NodeInfo nodeInfo = this.GetNodeInfo(node.Child0);
                info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            }
            foreach (Node node2 in n.Child2.Children)
            {
                NodeInfo info4 = this.GetNodeInfo(node2.Child0);
                info.ExternalReferences.Or(info4.ExternalReferences);
            }
            info.ExternalReferences.Minus(extendedNodeInfo.Definitions);
            info.Keys.InitFrom(op.Keys);
            info.MinRows = RowCount.Zero;
            info.MaxRows = op.Keys.IsEmpty ? RowCount.One : extendedNodeInfo.MaxRows;
            return info;
        }

        public override NodeInfo Visit(PhysicalProjectOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            foreach (Node node in n.Children)
            {
                NodeInfo nodeInfo = this.GetNodeInfo(node);
                info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            }
            info.Definitions.InitFrom(op.Outputs);
            info.LocalDefinitions.InitFrom(info.Definitions);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            if (!extendedNodeInfo.Keys.NoKeys)
            {
                VarVec vec = this.m_command.CreateVarVec(extendedNodeInfo.Keys.KeyVars);
                vec.Minus(info.Definitions);
                if (vec.IsEmpty)
                {
                    info.Keys.InitFrom(extendedNodeInfo.Keys);
                }
            }
            return info;
        }

        public override NodeInfo Visit(ProjectOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            foreach (Var var in op.Outputs)
            {
                if (extendedNodeInfo.Definitions.IsSet(var))
                {
                    info.Definitions.Set(var);
                }
                else
                {
                    info.ExternalReferences.Set(var);
                }
            }
            foreach (Node node in n.Child1.Children)
            {
                VarDefOp op2 = node.Op as VarDefOp;
                NodeInfo nodeInfo = this.GetNodeInfo(node.Child0);
                info.LocalDefinitions.Set(op2.Var);
                info.ExternalReferences.Clear(op2.Var);
                info.Definitions.Set(op2.Var);
                info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            }
            info.ExternalReferences.Minus(extendedNodeInfo.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            info.Keys.NoKeys = true;
            if (!extendedNodeInfo.Keys.NoKeys)
            {
                VarVec vec = this.m_command.CreateVarVec(extendedNodeInfo.Keys.KeyVars);
                Dictionary<Var, Var> varMap = ComputeVarRemappings(n.Child1);
                VarVec vec2 = vec.Remap(varMap);
                VarVec varSet = vec2.Clone();
                VarVec other = this.m_command.CreateVarVec(op.Outputs);
                vec2.Minus(other);
                if (vec2.IsEmpty)
                {
                    info.Keys.InitFrom(varSet);
                }
            }
            info.InitRowCountFrom(extendedNodeInfo);
            return info;
        }

        public override NodeInfo Visit(SingleRowOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            info.Definitions.InitFrom(extendedNodeInfo.Definitions);
            info.Keys.InitFrom(extendedNodeInfo.Keys);
            info.ExternalReferences.InitFrom(extendedNodeInfo.ExternalReferences);
            info.SetRowCount(RowCount.Zero, RowCount.One);
            return info;
        }

        public override NodeInfo Visit(SingleRowTableOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            info.Keys.NoKeys = false;
            info.SetRowCount(RowCount.One, RowCount.One);
            return info;
        }

        public override NodeInfo Visit(UnnestOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            foreach (Var var in op.Table.Columns)
            {
                info.LocalDefinitions.Set(var);
                info.Definitions.Set(var);
            }
            if (n.HasChild0)
            {
                NodeInfo nodeInfo = this.GetNodeInfo(n.Child0);
                info.ExternalReferences.Or(nodeInfo.ExternalReferences);
                return info;
            }
            info.ExternalReferences.Set(op.Var);
            return info;
        }

        public override NodeInfo Visit(VarRefOp op, Node n)
        {
            NodeInfo info = this.InitNodeInfo(n);
            info.ExternalReferences.Set(op.Var);
            return info;
        }

        protected override NodeInfo VisitApplyOp(ApplyBaseOp op, Node n)
        {
            RowCount one;
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            ExtendedNodeInfo info3 = this.GetExtendedNodeInfo(n.Child1);
            info.Definitions.Or(extendedNodeInfo.Definitions);
            info.Definitions.Or(info3.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            info.ExternalReferences.Or(info3.ExternalReferences);
            info.ExternalReferences.Minus(info.Definitions);
            info.Keys.InitFrom(extendedNodeInfo.Keys, info3.Keys);
            if ((extendedNodeInfo.MaxRows <= RowCount.One) && (info3.MaxRows <= RowCount.One))
            {
                one = RowCount.One;
            }
            else
            {
                one = RowCount.Unbounded;
            }
            RowCount minRows = (op.OpType == OpType.CrossApply) ? RowCount.Zero : extendedNodeInfo.MinRows;
            info.SetRowCount(minRows, one);
            return info;
        }

        protected override NodeInfo VisitDefault(Node n)
        {
            NodeInfo info = this.InitNodeInfo(n);
            foreach (Node node in n.Children)
            {
                NodeInfo nodeInfo = this.GetNodeInfo(node);
                info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            }
            return info;
        }

        protected override NodeInfo VisitJoinOp(JoinBaseOp op, Node n)
        {
            RowCount unbounded;
            RowCount zero;
            if (((op.OpType != OpType.InnerJoin) && (op.OpType != OpType.LeftOuterJoin)) && (op.OpType != OpType.FullOuterJoin))
            {
                return this.Unimplemented(n);
            }
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            ExtendedNodeInfo info3 = this.GetExtendedNodeInfo(n.Child1);
            NodeInfo nodeInfo = this.GetNodeInfo(n.Child2);
            info.Definitions.Or(extendedNodeInfo.Definitions);
            info.Definitions.Or(info3.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            info.ExternalReferences.Or(info3.ExternalReferences);
            info.ExternalReferences.Or(nodeInfo.ExternalReferences);
            info.ExternalReferences.Minus(info.Definitions);
            info.Keys.InitFrom(extendedNodeInfo.Keys, info3.Keys);
            if (op.OpType == OpType.FullOuterJoin)
            {
                zero = RowCount.Zero;
                unbounded = RowCount.Unbounded;
            }
            else
            {
                if ((extendedNodeInfo.MaxRows > RowCount.One) || (info3.MaxRows > RowCount.One))
                {
                    unbounded = RowCount.Unbounded;
                }
                else
                {
                    unbounded = RowCount.One;
                }
                if (op.OpType == OpType.LeftOuterJoin)
                {
                    zero = extendedNodeInfo.MinRows;
                }
                else
                {
                    zero = RowCount.Zero;
                }
            }
            info.SetRowCount(zero, unbounded);
            return info;
        }

        protected override NodeInfo VisitNestOp(NestBaseOp op, Node n)
        {
            SingleStreamNestOp op2 = op as SingleStreamNestOp;
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            foreach (CollectionInfo info2 in op.CollectionInfo)
            {
                info.LocalDefinitions.Set(info2.CollectionVar);
            }
            info.Definitions.InitFrom(op.Outputs);
            foreach (Node node in n.Children)
            {
                info.ExternalReferences.Or(this.GetExtendedNodeInfo(node).ExternalReferences);
            }
            info.ExternalReferences.Minus(info.Definitions);
            if (op2 == null)
            {
                info.Keys.InitFrom(this.GetExtendedNodeInfo(n.Child0).Keys);
                return info;
            }
            info.Keys.InitFrom(op2.Keys);
            return info;
        }

        protected override NodeInfo VisitRelOpDefault(RelOp op, Node n) => 
            this.Unimplemented(n);

        protected override NodeInfo VisitSetOp(SetOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            info.Definitions.InitFrom(op.Outputs);
            info.LocalDefinitions.InitFrom(op.Outputs);
            RowCount zero = RowCount.Zero;
            foreach (Node node in n.Children)
            {
                ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(node);
                info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
                if ((op.OpType == OpType.UnionAll) && (extendedNodeInfo.MinRows > zero))
                {
                    zero = extendedNodeInfo.MinRows;
                }
            }
            if ((op.OpType == OpType.Intersect) || (op.OpType == OpType.Except))
            {
                info.Keys.InitFrom(op.Outputs);
            }
            else
            {
                UnionAllOp op2 = (UnionAllOp) op;
                if (op2.BranchDiscriminator == null)
                {
                    info.Keys.NoKeys = true;
                }
                else
                {
                    VarVec varSet = this.m_command.CreateVarVec();
                    for (int i = 0; i < n.Children.Count; i++)
                    {
                        ExtendedNodeInfo info3 = n.Children[i].GetExtendedNodeInfo(this.m_command);
                        if (!info3.Keys.NoKeys && !info3.Keys.KeyVars.IsEmpty)
                        {
                            VarVec other = info3.Keys.KeyVars.Remap(op2.VarMap[i].GetReverseMap());
                            varSet.Or(other);
                        }
                        else
                        {
                            varSet.Clear();
                            break;
                        }
                    }
                    if (varSet.IsEmpty)
                    {
                        info.Keys.NoKeys = true;
                    }
                    else
                    {
                        info.Keys.InitFrom(varSet);
                    }
                }
            }
            info.MinRows = zero;
            return info;
        }

        protected override NodeInfo VisitSortOp(SortBaseOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n.Child0);
            info.Definitions.Or(extendedNodeInfo.Definitions);
            info.ExternalReferences.Or(extendedNodeInfo.ExternalReferences);
            info.ExternalReferences.Minus(extendedNodeInfo.Definitions);
            info.Keys.InitFrom(extendedNodeInfo.Keys);
            info.InitRowCountFrom(extendedNodeInfo);
            if (((OpType.ConstrainedSort == op.OpType) && (n.Child2.Op.OpType == OpType.Constant)) && !((ConstrainedSortOp) op).WithTies)
            {
                ConstantBaseOp op2 = (ConstantBaseOp) n.Child2.Op;
                if (TypeHelpers.IsIntegerConstant(op2.Type, op2.Value, 1L))
                {
                    info.SetRowCount(RowCount.Zero, RowCount.One);
                }
            }
            return info;
        }

        protected override NodeInfo VisitTableOp(ScanTableBaseOp op, Node n)
        {
            ExtendedNodeInfo info = this.InitExtendedNodeInfo(n);
            info.LocalDefinitions.Or(op.Table.ReferencedColumns);
            info.Definitions.Or(op.Table.ReferencedColumns);
            if (op.Table.ReferencedColumns.Subsumes(op.Table.Keys))
            {
                info.Keys.InitFrom(op.Table.Keys);
            }
            return info;
        }
    }
}

