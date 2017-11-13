namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class Simplifier : InternalBase
    {
        private CellNormalizer m_normalizer;

        private Simplifier(CellNormalizer normalizer)
        {
            this.m_normalizer = normalizer;
        }

        private static System.Data.Common.Utils.Set<LeafCellTreeNode> GetCommonGrandChildren(List<CellTreeNode> nodes)
        {
            System.Data.Common.Utils.Set<LeafCellTreeNode> set = null;
            CellTreeOpType leaf = CellTreeOpType.Leaf;
            foreach (CellTreeNode node in nodes)
            {
                OpCellTreeNode node2 = node as OpCellTreeNode;
                if (node2 == null)
                {
                    return null;
                }
                if (leaf == CellTreeOpType.Leaf)
                {
                    leaf = node2.OpType;
                }
                else if (!CellTreeNode.IsAssociativeOp(node2.OpType) || (leaf != node2.OpType))
                {
                    return null;
                }
                System.Data.Common.Utils.Set<LeafCellTreeNode> other = new System.Data.Common.Utils.Set<LeafCellTreeNode>(LeafCellTreeNode.EqualityComparer);
                foreach (CellTreeNode node3 in node2.Children)
                {
                    LeafCellTreeNode element = node3 as LeafCellTreeNode;
                    if (element == null)
                    {
                        return null;
                    }
                    other.Add(element);
                }
                if (set == null)
                {
                    set = other;
                }
                else
                {
                    set.Intersect(other);
                }
            }
            if (set.Count == 0)
            {
                return null;
            }
            return set;
        }

        private static List<CellTreeNode> GroupLeafChildrenByExtent(List<CellTreeNode> nodes)
        {
            KeyToListMap<EntitySetBase, CellTreeNode> map = new KeyToListMap<EntitySetBase, CellTreeNode>(EqualityComparer<EntitySetBase>.Default);
            List<CellTreeNode> list = new List<CellTreeNode>();
            foreach (CellTreeNode node in nodes)
            {
                LeafCellTreeNode node2 = node as LeafCellTreeNode;
                if (node2 != null)
                {
                    map.Add(node2.RightCellQuery.Extent, node2);
                }
                else
                {
                    list.Add(node);
                }
            }
            list.AddRange(map.AllValues);
            return list;
        }

        private static List<CellTreeNode> GroupNonAssociativeLeafChildren(List<CellTreeNode> nodes)
        {
            KeyToListMap<EntitySetBase, CellTreeNode> map = new KeyToListMap<EntitySetBase, CellTreeNode>(EqualityComparer<EntitySetBase>.Default);
            List<CellTreeNode> list = new List<CellTreeNode>();
            List<CellTreeNode> collection = new List<CellTreeNode>();
            list.Add(nodes[0]);
            for (int i = 1; i < nodes.Count; i++)
            {
                CellTreeNode item = nodes[i];
                LeafCellTreeNode node2 = item as LeafCellTreeNode;
                if (node2 != null)
                {
                    map.Add(node2.RightCellQuery.Extent, node2);
                }
                else
                {
                    collection.Add(item);
                }
            }
            LeafCellTreeNode node3 = nodes[0] as LeafCellTreeNode;
            if (node3 != null)
            {
                EntitySetBase extent = node3.RightCellQuery.Extent;
                if (map.ContainsKey(extent))
                {
                    list.AddRange(map.ListForKey(extent));
                    map.RemoveKey(extent);
                }
            }
            list.AddRange(map.AllValues);
            list.AddRange(collection);
            return list;
        }

        private CellTreeNode RestructureTreeForMerges(CellTreeNode rootNode)
        {
            List<CellTreeNode> children = rootNode.Children;
            if (!CellTreeNode.IsAssociativeOp(rootNode.OpType) || (children.Count <= 1))
            {
                return rootNode;
            }
            System.Data.Common.Utils.Set<LeafCellTreeNode> commonGrandChildren = GetCommonGrandChildren(children);
            if (commonGrandChildren == null)
            {
                return rootNode;
            }
            CellTreeOpType opType = children[0].OpType;
            List<OpCellTreeNode> values = new List<OpCellTreeNode>(children.Count);
            foreach (OpCellTreeNode node in children)
            {
                List<LeafCellTreeNode> list3 = new List<LeafCellTreeNode>(node.Children.Count);
                foreach (LeafCellTreeNode node2 in node.Children)
                {
                    if (!commonGrandChildren.Contains(node2))
                    {
                        list3.Add(node2);
                    }
                }
                OpCellTreeNode item = new OpCellTreeNode(this.m_normalizer, node.OpType, Helpers.AsSuperTypeList<LeafCellTreeNode, CellTreeNode>(list3));
                values.Add(item);
            }
            CellTreeNode node4 = new OpCellTreeNode(this.m_normalizer, rootNode.OpType, Helpers.AsSuperTypeList<OpCellTreeNode, CellTreeNode>(values));
            CellTreeNode node5 = new OpCellTreeNode(this.m_normalizer, opType, Helpers.AsSuperTypeList<LeafCellTreeNode, CellTreeNode>(commonGrandChildren));
            CellTreeNode node6 = new OpCellTreeNode(this.m_normalizer, opType, new CellTreeNode[] { node5, node4 });
            return node6.AssociativeFlatten();
        }

        internal static CellTreeNode Simplify(CellTreeNode rootNode, bool canBooleansOverlap)
        {
            Simplifier simplifier = new Simplifier(rootNode.CellNormalizer);
            return simplifier.SimplifyTree(rootNode, canBooleansOverlap);
        }

        private CellTreeNode SimplifyTree(CellTreeNode rootNode, bool canBooleansOverlap)
        {
            if (rootNode is LeafCellTreeNode)
            {
                return rootNode;
            }
            rootNode = this.RestructureTreeForMerges(rootNode);
            List<CellTreeNode> children = rootNode.Children;
            for (int i = 0; i < children.Count; i++)
            {
                children[i] = this.SimplifyTree(children[i], canBooleansOverlap);
            }
            bool flag = CellTreeNode.IsAssociativeOp(rootNode.OpType);
            if (flag)
            {
                children = GroupLeafChildrenByExtent(children);
            }
            else
            {
                children = GroupNonAssociativeLeafChildren(children);
            }
            OpCellTreeNode node = new OpCellTreeNode(this.m_normalizer, rootNode.OpType);
            CellTreeNode node2 = null;
            bool flag2 = false;
            foreach (CellTreeNode node3 in children)
            {
                if (node2 == null)
                {
                    node2 = node3;
                }
                else
                {
                    bool flag3 = false;
                    if ((!flag2 && (node2.OpType == CellTreeOpType.Leaf)) && (node3.OpType == CellTreeOpType.Leaf))
                    {
                        flag3 = this.TryMergeCellQueries(rootNode.OpType, canBooleansOverlap, ref node2, node3);
                    }
                    if (!flag3)
                    {
                        node.Add(node2);
                        node2 = node3;
                        if (!flag)
                        {
                            flag2 = true;
                        }
                    }
                }
            }
            node.Add(node2);
            return node.AssociativeFlatten();
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_normalizer.MemberMaps.ProjectedSlotMap.ToCompactString(builder);
        }

        private bool TryMergeCellQueries(CellTreeOpType opType, bool canBooleansOverlap, ref CellTreeNode node1, CellTreeNode node2)
        {
            CellQuery query3;
            LeafCellTreeNode node = node1 as LeafCellTreeNode;
            LeafCellTreeNode node3 = node2 as LeafCellTreeNode;
            CellQuery rightCellQuery = node.RightCellQuery;
            CellQuery query2 = node3.RightCellQuery;
            if (!rightCellQuery.TryMerge(query2, opType, canBooleansOverlap, this.m_normalizer.MemberMaps.RightDomainMap, out query3))
            {
                return false;
            }
            OpCellTreeNode node4 = new OpCellTreeNode(this.m_normalizer, opType);
            node4.Add(node1);
            node4.Add(node2);
            if (!canBooleansOverlap && (opType != CellTreeOpType.FOJ))
            {
            }
            LeftCellWrapper cellWrapper = new LeftCellWrapper(node.LeftCellWrapper.SchemaContext, node4.Attributes, node4.LeftFragmentQuery, query3, this.m_normalizer.MemberMaps, node.LeftCellWrapper.Cells.Concat<Cell>(node3.LeftCellWrapper.Cells));
            node1 = new LeafCellTreeNode(this.m_normalizer, cellWrapper, node4.RightFragmentQuery);
            return true;
        }
    }
}

