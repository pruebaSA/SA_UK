namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class Predicate
    {
        private Command m_command;
        private List<Node> m_parts;

        internal Predicate(Command command)
        {
            this.m_command = command;
            this.m_parts = new List<Node>();
        }

        internal Predicate(Command command, Node andTree) : this(command)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(andTree != null, "null node passed to Predicate() constructor");
            this.InitFromAndTree(andTree);
        }

        internal void AddPart(Node n)
        {
            this.m_parts.Add(n);
        }

        internal Node BuildAndTree()
        {
            Node node = null;
            foreach (Node node2 in this.m_parts)
            {
                if (node == null)
                {
                    node = node2;
                }
                else
                {
                    node = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node, node2);
                }
            }
            return node;
        }

        internal void GetEquiJoinPredicates(VarVec leftTableDefinitions, VarVec rightTableDefinitions, out List<Var> leftTableEquiJoinColumns, out List<Var> rightTableEquiJoinColumns, out Predicate otherPredicates)
        {
            otherPredicates = new Predicate(this.m_command);
            leftTableEquiJoinColumns = new List<Var>();
            rightTableEquiJoinColumns = new List<Var>();
            foreach (Node node in this.m_parts)
            {
                Var var;
                Var var2;
                if (IsEquiJoinPredicate(node, leftTableDefinitions, rightTableDefinitions, out var, out var2))
                {
                    leftTableEquiJoinColumns.Add(var);
                    rightTableEquiJoinColumns.Add(var2);
                }
                else
                {
                    otherPredicates.AddPart(node);
                }
            }
        }

        internal Predicate GetJoinPredicates(VarVec leftTableDefinitions, VarVec rightTableDefinitions, out Predicate otherPredicates)
        {
            Predicate predicate = new Predicate(this.m_command);
            otherPredicates = new Predicate(this.m_command);
            foreach (Node node in this.m_parts)
            {
                Var var;
                Var var2;
                if (IsEquiJoinPredicate(node, leftTableDefinitions, rightTableDefinitions, out var, out var2))
                {
                    predicate.AddPart(node);
                }
                else
                {
                    otherPredicates.AddPart(node);
                }
            }
            return predicate;
        }

        internal Predicate GetSingleTablePredicates(VarVec tableDefinitions, out Predicate otherPredicates)
        {
            List<Predicate> list2;
            List<VarVec> list = new List<VarVec> {
                tableDefinitions
            };
            this.GetSingleTablePredicates(list, out list2, out otherPredicates);
            return list2[0];
        }

        private void GetSingleTablePredicates(List<VarVec> tableDefinitions, out List<Predicate> singleTablePredicates, out Predicate otherPredicates)
        {
            singleTablePredicates = new List<Predicate>();
            using (List<VarVec>.Enumerator enumerator = tableDefinitions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    VarVec current = enumerator.Current;
                    singleTablePredicates.Add(new Predicate(this.m_command));
                }
            }
            otherPredicates = new Predicate(this.m_command);
            VarVec vec = this.m_command.CreateVarVec();
            foreach (Node node in this.m_parts)
            {
                NodeInfo nodeInfo = this.m_command.GetNodeInfo(node);
                bool flag = false;
                for (int i = 0; i < tableDefinitions.Count; i++)
                {
                    VarVec other = tableDefinitions[i];
                    if (other != null)
                    {
                        vec.InitFrom(nodeInfo.ExternalReferences);
                        vec.Minus(other);
                        if (vec.IsEmpty)
                        {
                            flag = true;
                            singleTablePredicates[i].AddPart(node);
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    otherPredicates.AddPart(node);
                }
            }
        }

        private void InitFromAndTree(Node andTree)
        {
            if (andTree.Op.OpType == OpType.And)
            {
                this.InitFromAndTree(andTree.Child0);
                this.InitFromAndTree(andTree.Child1);
            }
            else
            {
                this.m_parts.Add(andTree);
            }
        }

        private static bool IsEquiJoinPredicate(Node simplePredicateNode, out Var leftVar, out Var rightVar)
        {
            leftVar = null;
            rightVar = null;
            if (simplePredicateNode.Op.OpType != OpType.EQ)
            {
                return false;
            }
            VarRefOp op = simplePredicateNode.Child0.Op as VarRefOp;
            if (op == null)
            {
                return false;
            }
            VarRefOp op2 = simplePredicateNode.Child1.Op as VarRefOp;
            if (op2 == null)
            {
                return false;
            }
            leftVar = op.Var;
            rightVar = op2.Var;
            return true;
        }

        private static bool IsEquiJoinPredicate(Node simplePredicateNode, VarVec leftTableDefinitions, VarVec rightTableDefinitions, out Var leftVar, out Var rightVar)
        {
            Var var;
            Var var2;
            leftVar = null;
            rightVar = null;
            if (IsEquiJoinPredicate(simplePredicateNode, out var, out var2))
            {
                if (leftTableDefinitions.IsSet(var) && rightTableDefinitions.IsSet(var2))
                {
                    leftVar = var;
                    rightVar = var2;
                    goto Label_004D;
                }
                if (leftTableDefinitions.IsSet(var2) && rightTableDefinitions.IsSet(var))
                {
                    leftVar = var2;
                    rightVar = var;
                    goto Label_004D;
                }
            }
            return false;
        Label_004D:
            return true;
        }

        private bool IsKeyPredicate(Node left, Node right, VarVec keyVars, VarVec definitions, out Var keyVar)
        {
            keyVar = null;
            if (left.Op.OpType != OpType.VarRef)
            {
                return false;
            }
            VarRefOp op = (VarRefOp) left.Op;
            keyVar = op.Var;
            if (!keyVars.IsSet(keyVar))
            {
                return false;
            }
            VarVec vec = this.m_command.GetNodeInfo(right).ExternalReferences.Clone();
            vec.And(definitions);
            return vec.IsEmpty;
        }

        private static bool PreservesNulls(Node simplePredNode, VarVec tableColumns)
        {
            VarRefOp op;
            switch (simplePredNode.Op.OpType)
            {
                case OpType.GT:
                case OpType.GE:
                case OpType.LE:
                case OpType.LT:
                case OpType.EQ:
                case OpType.NE:
                    op = simplePredNode.Child0.Op as VarRefOp;
                    if ((op == null) || !tableColumns.IsSet(op.Var))
                    {
                        op = simplePredNode.Child1.Op as VarRefOp;
                        if ((op != null) && tableColumns.IsSet(op.Var))
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;

                case OpType.Like:
                {
                    ConstantBaseOp op2 = simplePredNode.Child1.Op as ConstantBaseOp;
                    if ((op2 != null) && (op2.OpType != OpType.Null))
                    {
                        op = simplePredNode.Child0.Op as VarRefOp;
                        if ((op != null) && tableColumns.IsSet(op.Var))
                        {
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                case OpType.Not:
                    if (simplePredNode.Child0.Op.OpType == OpType.IsNull)
                    {
                        op = simplePredNode.Child0.Child0.Op as VarRefOp;
                        if (op != null)
                        {
                            return !tableColumns.IsSet(op.Var);
                        }
                    }
                    return true;
            }
            return true;
        }

        internal bool PreservesNulls(VarVec tableColumns, bool ansiNullSemantics)
        {
            if (ansiNullSemantics)
            {
                foreach (Node node in this.m_parts)
                {
                    if (!PreservesNulls(node, tableColumns))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal bool SatisfiesKey(VarVec keyVars, VarVec definitions)
        {
            if (keyVars.Count <= 0)
            {
                return false;
            }
            VarVec vec = keyVars.Clone();
            foreach (Node node in this.m_parts)
            {
                if (node.Op.OpType == OpType.EQ)
                {
                    Var var;
                    if (this.IsKeyPredicate(node.Child0, node.Child1, keyVars, definitions, out var))
                    {
                        vec.Clear(var);
                    }
                    else if (this.IsKeyPredicate(node.Child1, node.Child0, keyVars, definitions, out var))
                    {
                        vec.Clear(var);
                    }
                }
            }
            return vec.IsEmpty;
        }
    }
}

