namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class VarRefManager
    {
        private Command m_command;
        private Dictionary<Node, Node> m_nodeToParentMap = new Dictionary<Node, Node>();
        private Dictionary<Node, int> m_nodeToSiblingNumber = new Dictionary<Node, int>();

        internal VarRefManager(Command command)
        {
            this.m_command = command;
        }

        internal void AddChildren(Node parent)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                this.m_nodeToParentMap[parent.Children[i]] = parent;
                this.m_nodeToSiblingNumber[parent.Children[i]] = i;
            }
        }

        internal bool HasKeyReferences(VarVec keys, Node definingNode, Node targetJoinNode)
        {
            Node node2;
            Node key = definingNode;
            bool continueUp = true;
            while (continueUp & this.m_nodeToParentMap.TryGetValue(key, out node2))
            {
                if (node2 != targetJoinNode)
                {
                    if (HasVarReferencesShallow(node2, keys, this.m_nodeToSiblingNumber[key], out continueUp))
                    {
                        return true;
                    }
                    for (int i = this.m_nodeToSiblingNumber[key] + 1; i < node2.Children.Count; i++)
                    {
                        if (node2.Children[i].GetNodeInfo(this.m_command).ExternalReferences.Overlaps(keys))
                        {
                            return true;
                        }
                    }
                }
                key = node2;
            }
            return false;
        }

        private static bool HasVarReferences(List<SortKey> listToCheck, VarVec vars)
        {
            foreach (SortKey key in listToCheck)
            {
                if (vars.IsSet(key.Var))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasVarReferences(VarList listToCheck, VarVec vars)
        {
            foreach (Var var in vars)
            {
                if (listToCheck.Contains(var))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasVarReferences(VarVec listToCheck, VarVec vars) => 
            listToCheck.Overlaps(vars);

        private static bool HasVarReferences(SetOp op, VarVec vars, int index)
        {
            foreach (Var var in op.VarMap[index].Values)
            {
                if (vars.IsSet(var))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasVarReferencesShallow(Node node, VarVec vars, int childIndex, out bool continueUp)
        {
            switch (node.Op.OpType)
            {
                case OpType.Project:
                    continueUp = false;
                    return HasVarReferences(((ProjectOp) node.Op).Outputs, vars);

                case OpType.Sort:
                case OpType.ConstrainedSort:
                    continueUp = true;
                    return HasVarReferences(((SortBaseOp) node.Op).Keys, vars);

                case OpType.GroupBy:
                    continueUp = false;
                    return HasVarReferences(((GroupByOp) node.Op).Keys, vars);

                case OpType.UnionAll:
                case OpType.Intersect:
                case OpType.Except:
                    continueUp = false;
                    return HasVarReferences((SetOp) node.Op, vars, childIndex);

                case OpType.Distinct:
                    continueUp = false;
                    return HasVarReferences(((DistinctOp) node.Op).Keys, vars);

                case OpType.PhysicalProject:
                    continueUp = false;
                    return HasVarReferences(((PhysicalProjectOp) node.Op).Outputs, vars);
            }
            continueUp = true;
            return false;
        }
    }
}

