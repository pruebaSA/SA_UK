namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class JoinGraph
    {
        private Command m_command;
        private ConstraintManager m_constraintManager;
        private bool m_modifiedGraph;
        private Dictionary<Node, Node> m_processedNodes;
        private AugmentedJoinNode m_root;
        private List<AugmentedTableNode> m_tableVertexes;
        private Dictionary<Table, AugmentedTableNode> m_tableVertexMap;
        private VarMap m_varMap;
        private VarRefManager m_varRefManager;
        private Dictionary<Var, AugmentedTableNode> m_varToDefiningNodeMap;
        private List<AugmentedNode> m_vertexes;

        internal JoinGraph(Command command, ConstraintManager constraintManager, VarRefManager varRefManager, Node joinNode)
        {
            this.m_command = command;
            this.m_constraintManager = constraintManager;
            this.m_varRefManager = varRefManager;
            this.m_vertexes = new List<AugmentedNode>();
            this.m_tableVertexes = new List<AugmentedTableNode>();
            this.m_tableVertexMap = new Dictionary<Table, AugmentedTableNode>();
            this.m_varMap = new VarMap();
            this.m_varToDefiningNodeMap = new Dictionary<Var, AugmentedTableNode>();
            this.m_processedNodes = new Dictionary<Node, Node>();
            this.m_root = this.BuildAugmentedNodeTree(joinNode) as AugmentedJoinNode;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_root != null, "The root isn't a join?");
            this.BuildJoinEdges(this.m_root, this.m_root.Id);
        }

        private bool AddJoinEdge(AugmentedJoinNode joinNode, ColumnVar leftVar, ColumnVar rightVar)
        {
            AugmentedTableNode node;
            AugmentedTableNode node2;
            if (!this.m_tableVertexMap.TryGetValue(leftVar.Table, out node))
            {
                return false;
            }
            if (!this.m_tableVertexMap.TryGetValue(rightVar.Table, out node2))
            {
                return false;
            }
            if ((node.LastVisibleId < joinNode.Id) || (node2.LastVisibleId < joinNode.Id))
            {
                return false;
            }
            foreach (JoinEdge edge in node.JoinEdges)
            {
                if (edge.Right.Table.Equals(rightVar.Table))
                {
                    return edge.AddCondition(joinNode, leftVar, rightVar);
                }
            }
            JoinEdge item = JoinEdge.CreateJoinEdge(node, node2, joinNode, leftVar, rightVar);
            node.JoinEdges.Add(item);
            return true;
        }

        private AugmentedNode BuildAugmentedNodeTree(Node node)
        {
            AugmentedNode node2;
            switch (node.Op.OpType)
            {
                case OpType.ScanTable:
                {
                    this.m_processedNodes[node] = node;
                    ScanTableOp op = (ScanTableOp) node.Op;
                    node2 = new AugmentedTableNode(this.m_vertexes.Count, node);
                    this.m_tableVertexMap[op.Table] = (AugmentedTableNode) node2;
                    break;
                }
                case OpType.InnerJoin:
                case OpType.LeftOuterJoin:
                case OpType.FullOuterJoin:
                {
                    List<ColumnVar> list;
                    List<ColumnVar> list2;
                    Node node5;
                    this.m_processedNodes[node] = node;
                    AugmentedNode leftChild = this.BuildAugmentedNodeTree(node.Child0);
                    AugmentedNode rightChild = this.BuildAugmentedNodeTree(node.Child1);
                    this.SplitPredicate(node, out list, out list2, out node5);
                    this.m_varRefManager.AddChildren(node);
                    node2 = new AugmentedJoinNode(this.m_vertexes.Count, node, leftChild, rightChild, list, list2, node5);
                    break;
                }
                case OpType.CrossJoin:
                {
                    this.m_processedNodes[node] = node;
                    List<AugmentedNode> children = new List<AugmentedNode>();
                    foreach (Node node6 in node.Children)
                    {
                        children.Add(this.BuildAugmentedNodeTree(node6));
                    }
                    node2 = new AugmentedJoinNode(this.m_vertexes.Count, node, children);
                    this.m_varRefManager.AddChildren(node);
                    break;
                }
                default:
                    node2 = new AugmentedNode(this.m_vertexes.Count, node);
                    break;
            }
            this.m_vertexes.Add(node2);
            return node2;
        }

        private Node BuildFilterForNullableColumns(Node inputNode, VarVec nonNullableColumns)
        {
            if (nonNullableColumns == null)
            {
                return inputNode;
            }
            VarVec vec = nonNullableColumns.Remap(this.m_varMap);
            if (vec.IsEmpty)
            {
                return inputNode;
            }
            Node node = null;
            foreach (Var var in vec)
            {
                Node node2 = this.m_command.CreateNode(this.m_command.CreateVarRefOp(var));
                Node node3 = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.IsNull), node2);
                node3 = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.Not), node3);
                if (node == null)
                {
                    node = node3;
                }
                else
                {
                    node = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node, node3);
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node != null, "Null predicate?");
            return this.m_command.CreateNode(this.m_command.CreateFilterOp(), inputNode, node);
        }

        private Node BuildFilterNode(Node inputNode, Node predicateNode)
        {
            if (predicateNode == null)
            {
                return inputNode;
            }
            return this.m_command.CreateNode(this.m_command.CreateFilterOp(), inputNode, predicateNode);
        }

        private void BuildJoinEdges(AugmentedJoinNode joinNode, int maxVisibility)
        {
            int id;
            int num2;
            OpType opType = joinNode.Node.Op.OpType;
            switch (opType)
            {
                case OpType.FullOuterJoin:
                    id = joinNode.Id;
                    num2 = joinNode.Id;
                    break;

                case OpType.LeftOuterJoin:
                    id = maxVisibility;
                    num2 = joinNode.Id;
                    break;

                case OpType.CrossJoin:
                    foreach (AugmentedNode node in joinNode.Children)
                    {
                        this.BuildJoinEdges(node, maxVisibility);
                    }
                    return;

                default:
                    id = maxVisibility;
                    num2 = maxVisibility;
                    break;
            }
            this.BuildJoinEdges(joinNode.Children[0], id);
            this.BuildJoinEdges(joinNode.Children[1], num2);
            if ((((joinNode.Node.Op.OpType != OpType.FullOuterJoin) && (joinNode.OtherPredicate == null)) && (joinNode.LeftVars.Count != 0)) && ((opType != OpType.LeftOuterJoin) || (SingleTableVars(joinNode.RightVars) && SingleTableVars(joinNode.LeftVars))))
            {
                JoinKind kind = (opType == OpType.LeftOuterJoin) ? JoinKind.LeftOuter : JoinKind.Inner;
                for (int i = 0; i < joinNode.LeftVars.Count; i++)
                {
                    if (this.AddJoinEdge(joinNode, joinNode.LeftVars[i], joinNode.RightVars[i]) && (kind == JoinKind.Inner))
                    {
                        this.AddJoinEdge(joinNode, joinNode.RightVars[i], joinNode.LeftVars[i]);
                    }
                }
            }
        }

        private void BuildJoinEdges(AugmentedNode node, int maxVisibility)
        {
            switch (node.Node.Op.OpType)
            {
                case OpType.ScanTable:
                {
                    AugmentedTableNode node2 = (AugmentedTableNode) node;
                    node2.LastVisibleId = maxVisibility;
                    break;
                }
                case OpType.ScanView:
                case OpType.Filter:
                case OpType.Project:
                    break;

                case OpType.InnerJoin:
                case OpType.LeftOuterJoin:
                case OpType.FullOuterJoin:
                case OpType.CrossJoin:
                    this.BuildJoinEdges(node as AugmentedJoinNode, maxVisibility);
                    return;

                default:
                    return;
            }
        }

        private Node BuildNodeTree()
        {
            Dictionary<Node, int> dictionary;
            if (!this.m_modifiedGraph)
            {
                return this.m_root.Node;
            }
            VarMap map = new VarMap();
            foreach (KeyValuePair<Var, Var> pair in this.m_varMap)
            {
                Var var2;
                Var key = pair.Value;
                while (this.m_varMap.TryGetValue(key, out var2))
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(var2 != null, "null var mapping?");
                    key = var2;
                }
                map[pair.Key] = key;
            }
            this.m_varMap = map;
            Node node = this.RebuildNodeTree(this.m_root, out dictionary);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node != null, "Resulting node tree is null");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((dictionary == null) || (dictionary.Count == 0), "Leaking predicates?");
            return node;
        }

        private static bool CanBeEliminated(AugmentedTableNode table, AugmentedTableNode replacingTable)
        {
            if (replacingTable.Id < table.NewLocationId)
            {
                return CanBeMoved(table, replacingTable);
            }
            return CanBeMoved(replacingTable, table);
        }

        private static bool CanBeMoved(AugmentedTableNode table, AugmentedTableNode replacingTable)
        {
            AugmentedNode leastCommonAncestor = GetLeastCommonAncestor(table, replacingTable);
            for (AugmentedNode node2 = table; (node2.Parent != null) && (node2 != leastCommonAncestor); node2 = node2.Parent)
            {
                if ((node2.Parent.Node.Op.OpType == OpType.LeftOuterJoin) && (node2.Parent.Children[0] == node2))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ChildTableHasKeyReferences(JoinEdge joinEdge) => 
            ((joinEdge.JoinNode == null) || this.m_varRefManager.HasKeyReferences(joinEdge.Right.Table.Keys, joinEdge.Right.Node, joinEdge.JoinNode.Node));

        private Node ClassifyPredicate(int targetNodeId, Node predicateNode, int predicateMinLocationId, Node result, Dictionary<Node, int> outPredicates)
        {
            if (targetNodeId >= predicateMinLocationId)
            {
                result = this.CombinePredicates(result, predicateNode);
                return result;
            }
            outPredicates.Add(predicateNode, predicateMinLocationId);
            return result;
        }

        private Node CombinePredicateNodes(int targetNodeId, Node localPredicateNode, int localPredicateMinLocationId, Dictionary<Node, int> leftPredicates, Dictionary<Node, int> rightPredicates, out Dictionary<Node, int> outPredicates)
        {
            Node result = null;
            outPredicates = new Dictionary<Node, int>();
            if (localPredicateNode != null)
            {
                result = this.ClassifyPredicate(targetNodeId, localPredicateNode, localPredicateMinLocationId, result, outPredicates);
            }
            if (leftPredicates != null)
            {
                foreach (KeyValuePair<Node, int> pair in leftPredicates)
                {
                    result = this.ClassifyPredicate(targetNodeId, pair.Key, pair.Value, result, outPredicates);
                }
            }
            if (rightPredicates != null)
            {
                foreach (KeyValuePair<Node, int> pair2 in rightPredicates)
                {
                    result = this.ClassifyPredicate(targetNodeId, pair2.Key, pair2.Value, result, outPredicates);
                }
            }
            return result;
        }

        private Node CombinePredicates(Node node1, Node node2)
        {
            if (node1 == null)
            {
                return node2;
            }
            if (node2 == null)
            {
                return node1;
            }
            return this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node1, node2);
        }

        internal Node DoJoinElimination(out VarMap varMap, out Dictionary<Node, Node> processedNodes)
        {
            this.GenerateTransitiveEdges();
            this.EliminateSelfJoins();
            this.EliminateParentChildJoins();
            Node node = this.BuildNodeTree();
            varMap = this.m_varMap;
            processedNodes = this.m_processedNodes;
            return node;
        }

        private void EliminateChildTable(JoinEdge joinEdge)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(joinEdge.JoinKind == JoinKind.LeftOuter, "Expected left-outer-join");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(joinEdge.Left.Id < joinEdge.Right.Id, string.Concat(new object[] { "(left-id, right-id) = (", joinEdge.Left.Id, ",", joinEdge.Right.Id, ")" }));
            this.MarkTableAsEliminated<ColumnVar>(joinEdge.Right, joinEdge.Left, joinEdge.RightVars, joinEdge.LeftVars);
        }

        private void EliminateParentChildJoin(JoinEdge joinEdge)
        {
            List<ForeignKeyConstraint> list;
            if (this.m_constraintManager.IsParentChildRelationship(joinEdge.Left.Table.TableMetadata.Extent, joinEdge.Right.Table.TableMetadata.Extent, out list))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert((list != null) && (list.Count > 0), "invalid fk constraints?");
                foreach (ForeignKeyConstraint constraint in list)
                {
                    if (this.TryEliminateParentChildJoin(joinEdge, constraint))
                    {
                        break;
                    }
                }
            }
        }

        private void EliminateParentChildJoins()
        {
            foreach (AugmentedNode node in this.m_vertexes)
            {
                AugmentedTableNode tableNode = node as AugmentedTableNode;
                if ((tableNode != null) && !tableNode.IsEliminated)
                {
                    this.EliminateParentChildJoins(tableNode);
                }
            }
        }

        private void EliminateParentChildJoins(AugmentedTableNode tableNode)
        {
            foreach (JoinEdge edge in tableNode.JoinEdges)
            {
                this.EliminateParentChildJoin(edge);
                if (tableNode.IsEliminated)
                {
                    break;
                }
            }
        }

        private void EliminateParentTable(JoinEdge joinEdge)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(joinEdge.JoinKind == JoinKind.Inner, "Expected inner join");
            this.MarkTableAsEliminated<ColumnVar>(joinEdge.Left, joinEdge.Right, joinEdge.LeftVars, joinEdge.RightVars);
            if (joinEdge.Right.NullableColumns == null)
            {
                joinEdge.Right.NullableColumns = this.m_command.CreateVarVec();
            }
            foreach (ColumnVar var in joinEdge.RightVars)
            {
                if (var.ColumnMetadata.IsNullable)
                {
                    joinEdge.Right.NullableColumns.Set(var);
                }
            }
        }

        private bool EliminateSelfJoin(JoinEdge joinEdge)
        {
            if (joinEdge.IsEliminated)
            {
                return false;
            }
            if (!joinEdge.Left.Table.TableMetadata.Extent.Equals(joinEdge.Right.Table.TableMetadata.Extent))
            {
                return false;
            }
            for (int i = 0; i < joinEdge.LeftVars.Count; i++)
            {
                if (!joinEdge.LeftVars[i].ColumnMetadata.Name.Equals(joinEdge.RightVars[i].ColumnMetadata.Name))
                {
                    return false;
                }
            }
            VarVec vec = this.m_command.CreateVarVec(joinEdge.Left.Table.Keys);
            foreach (Var var in joinEdge.LeftVars)
            {
                if ((joinEdge.JoinKind == JoinKind.LeftOuter) && !vec.IsSet(var))
                {
                    return false;
                }
                vec.Clear(var);
            }
            if (!vec.IsEmpty)
            {
                return false;
            }
            if (!CanBeEliminated(joinEdge.Right, joinEdge.Left))
            {
                return false;
            }
            this.EliminateSelfJoinedTable(joinEdge.Right, joinEdge.Left);
            return true;
        }

        private void EliminateSelfJoinedTable(AugmentedTableNode tableNode, AugmentedTableNode replacementNode)
        {
            this.MarkTableAsEliminated<Var>(tableNode, replacementNode, tableNode.Table.Columns, replacementNode.Table.Columns);
        }

        private void EliminateSelfJoins()
        {
            foreach (AugmentedNode node in this.m_vertexes)
            {
                AugmentedTableNode tableNode = node as AugmentedTableNode;
                if (tableNode != null)
                {
                    this.EliminateSelfJoins(tableNode);
                    this.EliminateStarSelfJoins(tableNode);
                }
            }
        }

        private void EliminateSelfJoins(AugmentedTableNode tableNode)
        {
            if (!tableNode.IsEliminated)
            {
                foreach (JoinEdge edge in tableNode.JoinEdges)
                {
                    this.EliminateSelfJoin(edge);
                }
            }
        }

        private void EliminateStarSelfJoin(List<JoinEdge> joinEdges)
        {
            JoinEdge edge = joinEdges[0];
            VarVec vec = this.m_command.CreateVarVec(edge.Right.Table.Keys);
            foreach (Var var in edge.RightVars)
            {
                if ((edge.JoinKind == JoinKind.LeftOuter) && !vec.IsSet(var))
                {
                    return;
                }
                vec.Clear(var);
            }
            if (vec.IsEmpty)
            {
                for (int i = 1; i < joinEdges.Count; i++)
                {
                    JoinEdge edge2 = joinEdges[i];
                    if ((edge2.LeftVars.Count != edge.LeftVars.Count) || (edge2.JoinKind != edge.JoinKind))
                    {
                        return;
                    }
                    for (int j = 0; j < edge2.LeftVars.Count; j++)
                    {
                        if (!edge2.LeftVars[j].Equals(edge.LeftVars[j]) || !edge2.RightVars[j].ColumnMetadata.Name.Equals(edge.RightVars[j].ColumnMetadata.Name))
                        {
                            return;
                        }
                    }
                }
                JoinEdge edge3 = edge;
                foreach (JoinEdge edge4 in joinEdges)
                {
                    if (edge3.Right.Id > edge4.Right.Id)
                    {
                        edge3 = edge4;
                    }
                }
                foreach (JoinEdge edge5 in joinEdges)
                {
                    if ((edge5 != edge3) && CanBeEliminated(edge5.Right, edge3.Right))
                    {
                        this.EliminateSelfJoinedTable(edge5.Right, edge3.Right);
                    }
                }
            }
        }

        private void EliminateStarSelfJoins(AugmentedTableNode tableNode)
        {
            Dictionary<EntitySetBase, List<JoinEdge>> dictionary = new Dictionary<EntitySetBase, List<JoinEdge>>();
            foreach (JoinEdge edge in tableNode.JoinEdges)
            {
                if (!edge.IsEliminated)
                {
                    List<JoinEdge> list;
                    if (!dictionary.TryGetValue(edge.Right.Table.TableMetadata.Extent, out list))
                    {
                        list = new List<JoinEdge>();
                        dictionary[edge.Right.Table.TableMetadata.Extent] = list;
                    }
                    list.Add(edge);
                }
            }
            foreach (KeyValuePair<EntitySetBase, List<JoinEdge>> pair in dictionary)
            {
                if (pair.Value.Count > 1)
                {
                    this.EliminateStarSelfJoin(pair.Value);
                }
            }
        }

        private bool GenerateTransitiveEdge(JoinEdge edge1, JoinEdge edge2)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(edge1.Right == edge2.Left, "need a common table for transitive predicate generation");
            if (edge2.Right == edge1.Left)
            {
                return false;
            }
            if (edge1.JoinKind != edge2.JoinKind)
            {
                return false;
            }
            if ((edge1.JoinKind == JoinKind.LeftOuter) && ((edge1.Left != edge1.Right) || (edge2.Left != edge2.Right)))
            {
                return false;
            }
            if (edge1.RightVars.Count != edge2.LeftVars.Count)
            {
                return false;
            }
            foreach (JoinEdge edge in edge1.Left.JoinEdges)
            {
                if (edge.Right == edge2.Right)
                {
                    return false;
                }
            }
            VarVec vec = this.m_command.CreateVarVec();
            foreach (Var var in edge1.RightVars)
            {
                vec.Set(var);
            }
            foreach (Var var2 in edge2.LeftVars)
            {
                if (!vec.IsSet(var2))
                {
                    return false;
                }
            }
            Dictionary<ColumnVar, ColumnVar> dictionary = new Dictionary<ColumnVar, ColumnVar>();
            for (int i = 0; i < edge1.LeftVars.Count; i++)
            {
                dictionary[edge1.RightVars[i]] = edge1.LeftVars[i];
            }
            List<ColumnVar> leftVars = new List<ColumnVar>();
            List<ColumnVar> rightVars = new List<ColumnVar>(edge2.RightVars);
            for (int j = 0; j < edge1.LeftVars.Count; j++)
            {
                ColumnVar var3 = dictionary[edge2.LeftVars[j]];
                leftVars.Add(var3);
            }
            JoinEdge item = JoinEdge.CreateTransitiveJoinEdge(edge1.Left, edge2.Right, edge1.JoinKind, leftVars, rightVars);
            edge1.Left.JoinEdges.Add(item);
            if (edge1.JoinKind == JoinKind.Inner)
            {
                JoinEdge edge4 = JoinEdge.CreateTransitiveJoinEdge(edge2.Right, edge1.Left, edge1.JoinKind, rightVars, leftVars);
                edge2.Right.JoinEdges.Add(edge4);
            }
            return true;
        }

        private void GenerateTransitiveEdges()
        {
            foreach (AugmentedNode node in this.m_vertexes)
            {
                AugmentedTableNode node2 = node as AugmentedTableNode;
                if (node2 != null)
                {
                    for (int i = 0; i < node2.JoinEdges.Count; i++)
                    {
                        JoinEdge edge = node2.JoinEdges[i];
                        int num2 = 0;
                        AugmentedTableNode right = edge.Right;
                        while (num2 < right.JoinEdges.Count)
                        {
                            JoinEdge edge2 = right.JoinEdges[num2];
                            this.GenerateTransitiveEdge(edge, edge2);
                            num2++;
                        }
                    }
                }
            }
        }

        private VarVec GetColumnVars(VarVec varVec)
        {
            VarVec vec = this.m_command.CreateVarVec();
            foreach (Var var in varVec)
            {
                if (var.VarType == VarType.Column)
                {
                    vec.Set(var);
                }
            }
            return vec;
        }

        private static void GetColumnVars(List<ColumnVar> columnVars, IEnumerable<Var> vec)
        {
            foreach (Var var in vec)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(var.VarType == VarType.Column, "Expected a columnVar. Found " + var.VarType);
                columnVars.Add((ColumnVar) var);
            }
        }

        private static AugmentedNode GetLeastCommonAncestor(AugmentedNode node1, AugmentedNode node2)
        {
            AugmentedNode parent;
            AugmentedNode node3;
            if (node1.Id == node2.Id)
            {
                return node1;
            }
            if (node1.Id < node2.Id)
            {
                parent = node1;
                node3 = node2;
            }
            else
            {
                parent = node2;
                node3 = node1;
            }
            while (parent.Id < node3.Id)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        private int GetLeastCommonAncestor(int nodeId1, int nodeId2)
        {
            if (nodeId1 == nodeId2)
            {
                return nodeId1;
            }
            AugmentedNode root = this.m_root;
            AugmentedNode node2 = root;
            for (AugmentedNode node3 = root; node2 == node3; node3 = PickSubtree(nodeId2, root))
            {
                root = node2;
                if ((root.Id == nodeId1) || (root.Id == nodeId2))
                {
                    return root.Id;
                }
                node2 = PickSubtree(nodeId1, root);
            }
            return root.Id;
        }

        private int GetLocationId(Var var, int defaultLocationId)
        {
            AugmentedTableNode node;
            if (!this.m_varToDefiningNodeMap.TryGetValue(var, out node))
            {
                return defaultLocationId;
            }
            if (node.IsMoved)
            {
                return node.NewLocationId;
            }
            return node.Id;
        }

        private static bool HasNonKeyReferences(Table table) => 
            !table.Keys.Subsumes(table.ReferencedColumns);

        private void MarkTableAsEliminated<T>(AugmentedTableNode tableNode, AugmentedTableNode replacementNode, List<T> tableVars, List<T> replacementVars) where T: Var
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((tableVars != null) && (replacementVars != null), "null vars");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(tableVars.Count == replacementVars.Count, "var count mismatch");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(tableVars.Count > 0, "no vars in the table ?");
            this.m_modifiedGraph = true;
            if (tableNode.Id < replacementNode.NewLocationId)
            {
                tableNode.ReplacementTable = replacementNode;
                replacementNode.NewLocationId = tableNode.Id;
            }
            else
            {
                tableNode.ReplacementTable = null;
            }
            for (int i = 0; i < tableVars.Count; i++)
            {
                if (tableNode.Table.ReferencedColumns.IsSet(tableVars[i]))
                {
                    this.m_varMap[tableVars[i]] = replacementVars[i];
                    replacementNode.Table.ReferencedColumns.Set(replacementVars[i]);
                }
            }
            foreach (Var var in replacementNode.Table.ReferencedColumns)
            {
                this.m_varToDefiningNodeMap[var] = replacementNode;
            }
        }

        private static AugmentedNode PickSubtree(int nodeId, AugmentedNode root)
        {
            AugmentedNode node = root.Children[0];
            for (int i = 1; (node.Id < nodeId) && (i < root.Children.Count); i++)
            {
                node = root.Children[i];
            }
            return node;
        }

        private Node RebuildNodeTree(AugmentedTableNode tableNode)
        {
            AugmentedTableNode replacementTable = tableNode;
            if (!tableNode.IsMoved)
            {
                while (replacementTable.IsEliminated)
                {
                    replacementTable = replacementTable.ReplacementTable;
                    if (replacementTable == null)
                    {
                        return null;
                    }
                }
                if (replacementTable.NewLocationId < tableNode.Id)
                {
                    return null;
                }
                return this.BuildFilterForNullableColumns(replacementTable.Node, replacementTable.NullableColumns);
            }
            return null;
        }

        private Node RebuildNodeTree(AugmentedJoinNode joinNode, out Dictionary<Node, int> predicates)
        {
            Dictionary<Node, int> dictionary;
            Dictionary<Node, int> dictionary2;
            int id;
            Node node3;
            if (joinNode.Node.Op.OpType == OpType.CrossJoin)
            {
                predicates = null;
                return this.RebuildNodeTreeForCrossJoins(joinNode);
            }
            Node inputNode = this.RebuildNodeTree(joinNode.Children[0], out dictionary);
            Node node2 = this.RebuildNodeTree(joinNode.Children[1], out dictionary2);
            if (((inputNode != null) && (node2 == null)) && (joinNode.Node.Op.OpType == OpType.LeftOuterJoin))
            {
                id = joinNode.Id;
                node3 = null;
            }
            else
            {
                node3 = this.RebuildPredicate(joinNode, out id);
            }
            node3 = this.CombinePredicateNodes(joinNode.Id, node3, id, dictionary, dictionary2, out predicates);
            if ((inputNode == null) && (node2 == null))
            {
                if (node3 == null)
                {
                    return null;
                }
                Node node4 = this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
                return this.BuildFilterNode(node4, node3);
            }
            if (inputNode == null)
            {
                return this.BuildFilterNode(node2, node3);
            }
            if (node2 == null)
            {
                return this.BuildFilterNode(inputNode, node3);
            }
            if (node3 == null)
            {
                node3 = this.m_command.CreateNode(this.m_command.CreateTrueOp());
            }
            Node node5 = this.m_command.CreateNode(joinNode.Node.Op, inputNode, node2, node3);
            this.m_processedNodes[node5] = node5;
            return node5;
        }

        private Node RebuildNodeTree(AugmentedNode augmentedNode, out Dictionary<Node, int> predicates)
        {
            switch (augmentedNode.Node.Op.OpType)
            {
                case OpType.ScanTable:
                    predicates = null;
                    return this.RebuildNodeTree((AugmentedTableNode) augmentedNode);

                case OpType.InnerJoin:
                case OpType.LeftOuterJoin:
                case OpType.FullOuterJoin:
                case OpType.CrossJoin:
                    return this.RebuildNodeTree((AugmentedJoinNode) augmentedNode, out predicates);
            }
            predicates = null;
            return augmentedNode.Node;
        }

        private Node RebuildNodeTreeForCrossJoins(AugmentedJoinNode joinNode)
        {
            List<Node> args = new List<Node>();
            foreach (AugmentedNode node in joinNode.Children)
            {
                Dictionary<Node, int> dictionary;
                args.Add(this.RebuildNodeTree(node, out dictionary));
                System.Data.Query.PlanCompiler.PlanCompiler.Assert((dictionary == null) || (dictionary.Count == 0), "Leaking predicates");
            }
            if (args.Count == 0)
            {
                return null;
            }
            if (args.Count == 1)
            {
                return args[0];
            }
            Node node2 = this.m_command.CreateNode(this.m_command.CreateCrossJoinOp(), args);
            this.m_processedNodes[node2] = node2;
            return node2;
        }

        private Node RebuildPredicate(AugmentedJoinNode joinNode, out int minLocationId)
        {
            minLocationId = joinNode.Id;
            if (joinNode.OtherPredicate != null)
            {
                foreach (Var var in joinNode.OtherPredicate.GetNodeInfo(this.m_command).ExternalReferences)
                {
                    Var var2;
                    if (!this.m_varMap.TryGetValue(var, out var2))
                    {
                        var2 = var;
                    }
                    minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(var2, minLocationId));
                }
            }
            Node otherPredicate = joinNode.OtherPredicate;
            for (int i = 0; i < joinNode.LeftVars.Count; i++)
            {
                Var var3;
                Var var4;
                if (!this.m_varMap.TryGetValue(joinNode.LeftVars[i], out var3))
                {
                    var3 = joinNode.LeftVars[i];
                }
                if (!this.m_varMap.TryGetValue(joinNode.RightVars[i], out var4))
                {
                    var4 = joinNode.RightVars[i];
                }
                if (!var3.Equals(var4))
                {
                    minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(var3, minLocationId));
                    minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(var4, minLocationId));
                    Node node2 = this.m_command.CreateNode(this.m_command.CreateVarRefOp(var3));
                    Node node3 = this.m_command.CreateNode(this.m_command.CreateVarRefOp(var4));
                    Node node4 = this.m_command.CreateNode(this.m_command.CreateComparisonOp(OpType.EQ), node2, node3);
                    if (otherPredicate != null)
                    {
                        otherPredicate = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node4, otherPredicate);
                    }
                    else
                    {
                        otherPredicate = node4;
                    }
                }
            }
            return otherPredicate;
        }

        private static bool SingleTableVars(IEnumerable<ColumnVar> varList)
        {
            Table table = null;
            foreach (ColumnVar var in varList)
            {
                if (table == null)
                {
                    table = var.Table;
                }
                else if (var.Table != table)
                {
                    return false;
                }
            }
            return true;
        }

        private void SplitPredicate(Node joinNode, out List<ColumnVar> leftVars, out List<ColumnVar> rightVars, out Node otherPredicateNode)
        {
            leftVars = new List<ColumnVar>();
            rightVars = new List<ColumnVar>();
            otherPredicateNode = joinNode.Child2;
            if (joinNode.Op.OpType != OpType.FullOuterJoin)
            {
                Predicate predicate2;
                List<Var> list;
                List<Var> list2;
                Predicate predicate = new Predicate(this.m_command, joinNode.Child2);
                ExtendedNodeInfo extendedNodeInfo = this.m_command.GetExtendedNodeInfo(joinNode.Child0);
                ExtendedNodeInfo info2 = this.m_command.GetExtendedNodeInfo(joinNode.Child1);
                VarVec columnVars = this.GetColumnVars(extendedNodeInfo.Definitions);
                VarVec rightTableDefinitions = this.GetColumnVars(info2.Definitions);
                predicate.GetEquiJoinPredicates(columnVars, rightTableDefinitions, out list, out list2, out predicate2);
                otherPredicateNode = predicate2.BuildAndTree();
                GetColumnVars(leftVars, list);
                GetColumnVars(rightVars, list2);
            }
        }

        private bool TryEliminateParentChildJoin(JoinEdge joinEdge, ForeignKeyConstraint fkConstraint)
        {
            if ((joinEdge.JoinKind == JoinKind.LeftOuter) && (fkConstraint.ChildMultiplicity == RelationshipMultiplicity.Many))
            {
                return false;
            }
            foreach (string str in fkConstraint.ParentKeys)
            {
                bool flag = false;
                foreach (ColumnVar var in joinEdge.LeftVars)
                {
                    if (var.ColumnMetadata.Name.Equals(str))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            foreach (string str2 in fkConstraint.ChildKeys)
            {
                bool flag2 = false;
                for (int i = 0; i < joinEdge.LeftVars.Count; i++)
                {
                    ColumnVar var2 = joinEdge.RightVars[i];
                    if (var2.ColumnMetadata.Name.Equals(str2))
                    {
                        string str3;
                        flag2 = true;
                        ColumnVar var3 = joinEdge.LeftVars[i];
                        if (!fkConstraint.GetParentProperty(var2.ColumnMetadata.Name, out str3) || !str3.Equals(var3.ColumnMetadata.Name))
                        {
                            return false;
                        }
                        break;
                    }
                }
                if (!flag2)
                {
                    return false;
                }
            }
            if (joinEdge.JoinKind == JoinKind.Inner)
            {
                if (HasNonKeyReferences(joinEdge.Left.Table))
                {
                    return false;
                }
                if (!CanBeEliminated(joinEdge.Right, joinEdge.Left))
                {
                    return false;
                }
                this.EliminateParentTable(joinEdge);
            }
            else if (joinEdge.JoinKind == JoinKind.LeftOuter)
            {
                if (HasNonKeyReferences(joinEdge.Right.Table) || ((fkConstraint.ChildMultiplicity == RelationshipMultiplicity.ZeroOrOne) && this.ChildTableHasKeyReferences(joinEdge)))
                {
                    return false;
                }
                if (!CanBeEliminated(joinEdge.Right, joinEdge.Left))
                {
                    return false;
                }
                this.EliminateChildTable(joinEdge);
            }
            return true;
        }
    }
}

