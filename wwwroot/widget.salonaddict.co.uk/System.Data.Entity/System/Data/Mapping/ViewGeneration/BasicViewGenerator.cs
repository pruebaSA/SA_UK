namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class BasicViewGenerator : InternalBase
    {
        private FragmentQuery m_activeDomain;
        private ConfigViewGenerator m_config;
        private MemberDomainMap m_domainMap;
        private ErrorLog m_errorLog;
        private CellNormalizer m_normalizer;
        private MemberPathMapBase m_projectedSlotMap;
        private List<LeftCellWrapper> m_usedCells;

        internal BasicViewGenerator(MemberPathMapBase projectedSlotMap, List<LeftCellWrapper> usedCells, FragmentQuery activeDomain, CellNormalizer normalizer, MemberDomainMap domainMap, ErrorLog errorLog, ConfigViewGenerator config)
        {
            this.m_projectedSlotMap = projectedSlotMap;
            this.m_usedCells = usedCells;
            this.m_normalizer = normalizer;
            this.m_activeDomain = activeDomain;
            this.m_errorLog = errorLog;
            this.m_config = config;
            this.m_domainMap = domainMap;
        }

        internal CellTreeNode CreateViewExpression()
        {
            OpCellTreeNode rootNode = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.FOJ);
            foreach (LeftCellWrapper wrapper in this.m_usedCells)
            {
                LeafCellTreeNode child = new LeafCellTreeNode(this.m_normalizer, wrapper);
                rootNode.Add(child);
            }
            CellTreeNode node3 = this.GroupByRightExtent(rootNode);
            node3 = this.IsolateUnions(node3);
            node3 = this.IsolateByOperator(node3, CellTreeOpType.Union);
            node3 = this.IsolateByOperator(node3, CellTreeOpType.IJ);
            return this.IsolateByOperator(node3, CellTreeOpType.LOJ);
        }

        internal CellTreeNode GroupByRightExtent(CellTreeNode rootNode)
        {
            KeyToListMap<EntitySetBase, LeafCellTreeNode> map = new KeyToListMap<EntitySetBase, LeafCellTreeNode>(EqualityComparer<EntitySetBase>.Default);
            foreach (LeafCellTreeNode node in rootNode.Children)
            {
                EntitySetBase extent = node.RightCellQuery.Extent;
                map.Add(extent, node);
            }
            OpCellTreeNode node2 = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.FOJ);
            foreach (EntitySetBase base3 in map.Keys)
            {
                OpCellTreeNode child = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.FOJ);
                foreach (LeafCellTreeNode node4 in map.ListForKey(base3))
                {
                    child.Add(node4);
                }
                node2.Add(child);
            }
            return node2.Flatten();
        }

        private bool IsContainedIn(CellTreeNode n1, CellTreeNode n2)
        {
            FragmentQuery query = this.LeftQP.Intersect(n1.LeftFragmentQuery, this.m_activeDomain);
            FragmentQuery query2 = this.LeftQP.Intersect(n2.LeftFragmentQuery, this.m_activeDomain);
            if (this.LeftQP.IsContainedIn(query, query2))
            {
                return true;
            }
            CellTreeNode n = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.LASJ, new CellTreeNode[] { n1, n2 });
            return this.m_normalizer.IsEmpty(n);
        }

        private bool IsDisjoint(CellTreeNode n1, CellTreeNode n2)
        {
            ViewTarget viewTarget = this.m_normalizer.SchemaContext.ViewTarget;
            bool flag = this.LeftQP.IsDisjointFrom(n1.LeftFragmentQuery, n2.LeftFragmentQuery);
            if (!flag || (this.m_normalizer.SchemaContext.ViewTarget != ViewTarget.QueryView))
            {
                CellTreeNode n = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.IJ, new CellTreeNode[] { n1, n2 });
                bool flag2 = this.m_normalizer.IsEmpty(n);
                if (((this.m_normalizer.SchemaContext.ViewTarget == ViewTarget.UpdateView) && flag) && !flag2)
                {
                    if (!ErrorPatternMatcher.FindMappingErrors(this.m_normalizer, this.m_domainMap, this.m_errorLog))
                    {
                        StringBuilder builder = new StringBuilder(Strings.Viewgen_RightSideNotDisjoint(this.m_normalizer.Extent.ToString()));
                        builder.AppendLine();
                        FragmentQuery query = this.LeftQP.Intersect(n1.RightFragmentQuery, n2.RightFragmentQuery);
                        if (this.LeftQP.IsSatisfiable(query))
                        {
                            query.Condition.ExpensiveSimplify();
                            RewritingValidator.EntityConfigurationToUserString(query.Condition, builder);
                        }
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.DisjointConstraintViolation, builder.ToString(), this.m_normalizer.AllWrappersForExtent, string.Empty));
                        ExceptionHelpers.ThrowMappingException(this.m_errorLog, this.m_config);
                    }
                    return false;
                }
                if (!flag)
                {
                    return flag2;
                }
            }
            return true;
        }

        private bool IsEquivalentTo(CellTreeNode n1, CellTreeNode n2) => 
            (this.IsContainedIn(n1, n2) && this.IsContainedIn(n2, n1));

        internal CellTreeNode IsolateByOperator(CellTreeNode rootNode, CellTreeOpType opTypeToIsolate)
        {
            List<CellTreeNode> children = rootNode.Children;
            if (children.Count <= 1)
            {
                return rootNode;
            }
            for (int i = 0; i < children.Count; i++)
            {
                children[i] = this.IsolateByOperator(children[i], opTypeToIsolate);
            }
            if ((rootNode.OpType != CellTreeOpType.FOJ) && (rootNode.OpType != CellTreeOpType.LOJ))
            {
                return rootNode;
            }
            if (rootNode.OpType == opTypeToIsolate)
            {
                return rootNode;
            }
            OpCellTreeNode node = new OpCellTreeNode(this.m_normalizer, rootNode.OpType);
            ModifiableIteratorCollection<CellTreeNode> iterators = new ModifiableIteratorCollection<CellTreeNode>(children);
            while (!iterators.IsEmpty)
            {
                OpCellTreeNode groupNode = new OpCellTreeNode(this.m_normalizer, opTypeToIsolate);
                CellTreeNode child = iterators.RemoveOneElement();
                groupNode.Add(child);
                foreach (CellTreeNode node4 in iterators.Elements())
                {
                    if (this.TryAddChildToGroup(opTypeToIsolate, node4, groupNode))
                    {
                        iterators.RemoveCurrentOfIterator();
                        iterators.ResetIterator();
                    }
                }
                node.Add(groupNode);
            }
            return node.Flatten();
        }

        private CellTreeNode IsolateUnions(CellTreeNode rootNode)
        {
            if (rootNode.Children.Count <= 1)
            {
                return rootNode;
            }
            for (int i = 0; i < rootNode.Children.Count; i++)
            {
                rootNode.Children[i] = this.IsolateUnions(rootNode.Children[i]);
            }
            OpCellTreeNode node = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.Union);
            ModifiableIteratorCollection<CellTreeNode> iterators = new ModifiableIteratorCollection<CellTreeNode>(rootNode.Children);
            while (!iterators.IsEmpty)
            {
                OpCellTreeNode node2 = new OpCellTreeNode(this.m_normalizer, CellTreeOpType.FOJ);
                CellTreeNode child = iterators.RemoveOneElement();
                node2.Add(child);
                foreach (CellTreeNode node4 in iterators.Elements())
                {
                    if (!this.IsDisjoint(node2, node4))
                    {
                        node2.Add(node4);
                        iterators.RemoveCurrentOfIterator();
                        iterators.ResetIterator();
                    }
                }
                node.Add(node2);
            }
            return node.Flatten();
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_projectedSlotMap.ToCompactString(builder);
        }

        private bool TryAddChildToGroup(CellTreeOpType opTypeToIsolate, CellTreeNode childNode, OpCellTreeNode groupNode)
        {
            switch (opTypeToIsolate)
            {
                case CellTreeOpType.Union:
                    if (!this.IsDisjoint(childNode, groupNode))
                    {
                        break;
                    }
                    groupNode.Add(childNode);
                    return true;

                case CellTreeOpType.LOJ:
                    if (!this.IsContainedIn(childNode, groupNode))
                    {
                        if (this.IsContainedIn(groupNode, childNode))
                        {
                            groupNode.AddFirst(childNode);
                            return true;
                        }
                        break;
                    }
                    groupNode.Add(childNode);
                    return true;

                case CellTreeOpType.IJ:
                    if (!this.IsEquivalentTo(childNode, groupNode))
                    {
                        break;
                    }
                    groupNode.Add(childNode);
                    return true;
            }
            return false;
        }

        private FragmentQueryProcessor LeftQP =>
            this.m_normalizer.LeftFragmentQP;
    }
}

