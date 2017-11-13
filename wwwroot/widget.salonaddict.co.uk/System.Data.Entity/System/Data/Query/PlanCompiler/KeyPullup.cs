namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class KeyPullup : BasicOpVisitor
    {
        private Command m_command;

        internal KeyPullup(Command command)
        {
            this.m_command = command;
        }

        internal KeyVec GetKeys(Node node)
        {
            ExtendedNodeInfo extendedNodeInfo = node.GetExtendedNodeInfo(this.m_command);
            if (extendedNodeInfo.Keys.NoKeys)
            {
                this.VisitNode(node);
            }
            return extendedNodeInfo.Keys;
        }

        public override void Visit(PhysicalProjectOp op, Node n)
        {
            this.VisitChildren(n);
            ExtendedNodeInfo extendedNodeInfo = this.m_command.GetExtendedNodeInfo(n.Child0);
            ExtendedNodeInfo info2 = this.m_command.GetExtendedNodeInfo(n);
            VarVec keyVars = extendedNodeInfo.Keys.KeyVars;
            keyVars.Minus(info2.Definitions);
            op.Outputs.AddRange(keyVars);
            this.m_command.RecomputeNodeInfo(n);
        }

        public override void Visit(ProjectOp op, Node n)
        {
            this.VisitChildren(n);
            ExtendedNodeInfo extendedNodeInfo = n.Child0.GetExtendedNodeInfo(this.m_command);
            if (!extendedNodeInfo.Keys.NoKeys)
            {
                VarVec other = this.m_command.CreateVarVec(op.Outputs);
                Dictionary<Var, Var> varMap = NodeInfoVisitor.ComputeVarRemappings(n.Child1);
                VarVec vec2 = extendedNodeInfo.Keys.KeyVars.Remap(varMap);
                other.Or(vec2);
                op.Outputs.InitFrom(other);
            }
            this.m_command.RecomputeNodeInfo(n);
        }

        public override void Visit(ScanTableOp op, Node n)
        {
            op.Table.ReferencedColumns.Or(op.Table.Keys);
            this.m_command.RecomputeNodeInfo(n);
        }

        public override void Visit(UnionAllOp op, Node n)
        {
            this.VisitChildren(n);
            Var key = this.m_command.CreateSetOpVar(this.m_command.IntegerType);
            VarList list = Command.CreateVarList();
            VarVec[] vecArray = new VarVec[n.Children.Count];
            for (int i = 0; i < n.Children.Count; i++)
            {
                Node node = n.Children[i];
                VarVec v = this.m_command.GetExtendedNodeInfo(node).Keys.KeyVars.Remap(op.VarMap[i]);
                vecArray[i] = this.m_command.CreateVarVec(v);
                vecArray[i].Minus(op.Outputs);
                if (OpType.UnionAll == node.Op.OpType)
                {
                    UnionAllOp op2 = (UnionAllOp) node.Op;
                    vecArray[i].Clear(op2.BranchDiscriminator);
                }
                list.AddRange(vecArray[i]);
            }
            VarList list2 = Command.CreateVarList();
            foreach (Var var2 in list)
            {
                Var item = this.m_command.CreateSetOpVar(var2.Type);
                list2.Add(item);
            }
            for (int j = 0; j < n.Children.Count; j++)
            {
                Var branchDiscriminator;
                Node node2 = n.Children[j];
                ExtendedNodeInfo extendedNodeInfo = this.m_command.GetExtendedNodeInfo(node2);
                VarVec vars = this.m_command.CreateVarVec();
                List<Node> args = new List<Node>();
                if ((OpType.UnionAll == node2.Op.OpType) && (((UnionAllOp) node2.Op).BranchDiscriminator != null))
                {
                    branchDiscriminator = ((UnionAllOp) node2.Op).BranchDiscriminator;
                    if (!op.VarMap[j].ContainsValue(branchDiscriminator))
                    {
                        op.VarMap[j].Add(key, branchDiscriminator);
                    }
                    else
                    {
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(0 == j, "right branch has a discriminator var that the left branch doesn't have?");
                        key = op.VarMap[j].GetReverseMap()[branchDiscriminator];
                    }
                }
                else
                {
                    args.Add(this.m_command.CreateVarDefNode(this.m_command.CreateNode(this.m_command.CreateConstantOp(this.m_command.IntegerType, this.m_command.NextBranchDiscriminatorValue)), out branchDiscriminator));
                    vars.Set(branchDiscriminator);
                    op.VarMap[j].Add(key, branchDiscriminator);
                }
                for (int k = 0; k < list.Count; k++)
                {
                    Var var5 = list[k];
                    if (!vecArray[j].IsSet(var5))
                    {
                        Node definingExpr = this.m_command.CreateNode(this.m_command.CreateNullOp(var5.Type));
                        args.Add(this.m_command.CreateVarDefNode(definingExpr, out var5));
                        vars.Set(var5);
                    }
                    op.VarMap[j].Add(list2[k], var5);
                }
                if (vars.IsEmpty)
                {
                    extendedNodeInfo.Keys.KeyVars.Set(branchDiscriminator);
                }
                else
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(args.Count != 0, "no new nodes?");
                    foreach (Var var6 in op.VarMap[j].Values)
                    {
                        vars.Set(var6);
                    }
                    n.Children[j] = this.m_command.CreateNode(this.m_command.CreateProjectOp(vars), node2, this.m_command.CreateNode(this.m_command.CreateVarDefListOp(), args));
                    ExtendedNodeInfo info3 = (ExtendedNodeInfo) this.m_command.RecomputeNodeInfo(n.Children[j]);
                    info3.Keys.KeyVars.InitFrom(extendedNodeInfo.Keys.KeyVars);
                    info3.Keys.KeyVars.Set(branchDiscriminator);
                }
            }
            n.Op = this.m_command.CreateUnionAllOp(op.VarMap[0], op.VarMap[1], key);
            this.m_command.RecomputeNodeInfo(n);
        }

        protected override void VisitChildren(Node n)
        {
            foreach (Node node in n.Children)
            {
                if (node.Op.IsRelOp || node.Op.IsPhysicalOp)
                {
                    this.GetKeys(node);
                }
            }
        }

        protected override void VisitPhysicalOpDefault(PhysicalOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_command.RecomputeNodeInfo(n);
        }

        protected override void VisitRelOpDefault(RelOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_command.RecomputeNodeInfo(n);
        }
    }
}

