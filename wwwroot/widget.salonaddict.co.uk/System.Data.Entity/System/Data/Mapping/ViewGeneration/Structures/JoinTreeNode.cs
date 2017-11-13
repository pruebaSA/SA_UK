namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Text;

    internal abstract class JoinTreeNode : InternalBase
    {
        private List<MemberJoinTreeNode> m_children;
        private bool m_isOptional;
        private System.Data.Mapping.ViewGeneration.Structures.MemberPath m_memberPath;
        private JoinTreeNode m_parent;
        private System.Data.Metadata.Edm.MetadataWorkspace m_workspace;

        protected JoinTreeNode(bool isOptional, IEnumerable<MemberJoinTreeNode> children, System.Data.Metadata.Edm.MetadataWorkspace workspace)
        {
            this.m_isOptional = isOptional;
            this.m_children = new List<MemberJoinTreeNode>(children);
            this.m_workspace = workspace;
            foreach (JoinTreeNode node in this.m_children)
            {
                node.m_parent = this;
            }
        }

        private void Add(MemberJoinTreeNode node)
        {
            this.m_children.Add(node);
            node.m_parent = this;
        }

        internal JoinTreeNode CreateAttributeNode(EdmMember member)
        {
            foreach (MemberJoinTreeNode node in this.m_children)
            {
                if (member.Equals(node.Member))
                {
                    return node;
                }
            }
            MemberJoinTreeNode node2 = new MemberJoinTreeNode(member, false, new MemberJoinTreeNode[0], this.m_workspace);
            this.Add(node2);
            return node2;
        }

        protected abstract JoinTreeNode CreateNodeFromContext(bool nodeIsOptional, List<MemberJoinTreeNode> children);
        private JoinTreeNode DeepClone(bool isOptionalForNode, Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            List<MemberJoinTreeNode> children = new List<MemberJoinTreeNode>();
            foreach (MemberJoinTreeNode node in this.m_children)
            {
                children.Add((MemberJoinTreeNode) node.DeepClone(node.m_isOptional, remap));
            }
            JoinTreeNode node2 = this.CreateNodeFromContext(isOptionalForNode, children);
            remap[this] = node2;
            return node2;
        }

        internal void GatherDescendantSlots(List<JoinTreeSlot> slots)
        {
            slots.Add(new JoinTreeSlot(this));
            foreach (JoinTreeNode node in this.m_children)
            {
                node.GatherDescendantSlots(slots);
            }
        }

        internal abstract void GetIdentifiers(CqlIdentifiers identifiers);
        protected abstract bool IsSameContext(JoinTreeNode second);
        private static BoolExpression ProcessUnmergedChildren(JoinTreeNode mergedNode, JoinTreeNode node, BoolExpression g, Dictionary<JoinTreeNode, JoinTreeNode> remap, bool[] matched, CellTreeOpType opType, MemberDomainMap memberDomainMap)
        {
            for (int i = 0; i < node.m_children.Count; i++)
            {
                if (!matched[i])
                {
                    JoinTreeNode node2 = node.m_children[i];
                    bool isOptionalForNode = ((opType != CellTreeOpType.IJ) || node2.m_isOptional) && (MinOccurs == Occurrences.Zero);
                    MemberJoinTreeNode node3 = (MemberJoinTreeNode) node2.DeepClone(isOptionalForNode, remap);
                    mergedNode.Add(node3);
                    if (node3.m_isOptional != node2.m_isOptional)
                    {
                        IfJoinedCondition literal = new IfJoinedCondition(node2);
                        g = BoolExpression.CreateAnd(new BoolExpression[] { g, BoolExpression.CreateLiteral(literal, memberDomainMap) });
                    }
                }
            }
            return g;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.MemberPath.ToCompactString(builder);
        }

        internal override void ToFullString(StringBuilder builder)
        {
            builder.Append(this.ContextName);
            if (this.m_isOptional)
            {
                builder.Append('?');
            }
            bool flag = true;
            foreach (MemberJoinTreeNode node in this.m_children)
            {
                if (flag)
                {
                    builder.Append('<');
                    flag = false;
                }
                else
                {
                    builder.Append(", ");
                }
                node.ToFullString(builder);
            }
            if (!flag)
            {
                builder.Append('>');
            }
        }

        internal JoinTreeNode TryMergeNode(JoinTreeNode node2, CellTreeOpType opType, ref BoolExpression g1, ref BoolExpression g2, Dictionary<JoinTreeNode, JoinTreeNode> remap, MemberDomainMap memberDomainMap)
        {
            JoinTreeNode node = this;
            if (!node.IsSameContext(node2))
            {
                return null;
            }
            bool nodeIsOptional = false;
            switch (opType)
            {
                case CellTreeOpType.Union:
                case CellTreeOpType.FOJ:
                    nodeIsOptional = node.m_isOptional || node2.m_isOptional;
                    break;

                case CellTreeOpType.LOJ:
                case CellTreeOpType.LASJ:
                    nodeIsOptional = node.m_isOptional;
                    break;

                case CellTreeOpType.IJ:
                    nodeIsOptional = node.m_isOptional && node2.m_isOptional;
                    break;
            }
            JoinTreeNode mergedNode = node.CreateNodeFromContext(nodeIsOptional, new List<MemberJoinTreeNode>());
            if (mergedNode.m_isOptional && !node2.m_isOptional)
            {
                IfJoinedCondition literal = new IfJoinedCondition(node2);
                g2 = BoolExpression.CreateAnd(new BoolExpression[] { g2, BoolExpression.CreateLiteral(literal, memberDomainMap) });
            }
            if (mergedNode.m_isOptional && !node.m_isOptional)
            {
                IfJoinedCondition condition2 = new IfJoinedCondition(node);
                g1 = BoolExpression.CreateAnd(new BoolExpression[] { g1, BoolExpression.CreateLiteral(condition2, memberDomainMap) });
            }
            bool[] matched = new bool[node.m_children.Count];
            bool[] flagArray2 = new bool[node2.m_children.Count];
            for (int i = 0; i < node.m_children.Count; i++)
            {
                MemberJoinTreeNode node4 = node.m_children[i];
                for (int j = 0; j < node2.m_children.Count; j++)
                {
                    MemberJoinTreeNode node5 = node2.m_children[j];
                    if (node4.Member.Equals(node5.Member))
                    {
                        MemberJoinTreeNode node6 = (MemberJoinTreeNode) node4.TryMergeNode(node5, opType, ref g1, ref g2, remap, memberDomainMap);
                        mergedNode.Add(node6);
                        matched[i] = true;
                        flagArray2[j] = true;
                    }
                }
            }
            g2 = ProcessUnmergedChildren(mergedNode, node, g2, remap, matched, opType, memberDomainMap);
            g1 = ProcessUnmergedChildren(mergedNode, node2, g1, remap, flagArray2, opType, memberDomainMap);
            remap[node] = mergedNode;
            remap[node2] = mergedNode;
            return mergedNode;
        }

        [Conditional("DEBUG")]
        internal void Validate()
        {
            EdmType nodeType = this.NodeType;
            foreach (MemberJoinTreeNode node in this.m_children)
            {
                EdmMember member = node.Member;
            }
        }

        protected abstract string ContextName { get; }

        internal System.Data.Mapping.ViewGeneration.Structures.MemberPath MemberPath
        {
            get
            {
                System.Data.Mapping.ViewGeneration.Structures.MemberPath memberPath = this.m_memberPath;
                EntitySetBase extent = null;
                if (this.m_memberPath == null)
                {
                    List<EdmMember> path = new List<EdmMember>();
                    for (JoinTreeNode node = this; node != null; node = node.m_parent)
                    {
                        MemberJoinTreeNode node2 = node as MemberJoinTreeNode;
                        if (node2 != null)
                        {
                            path.Add(node2.Member);
                        }
                        else
                        {
                            ExtentJoinTreeNode node3 = (ExtentJoinTreeNode) node;
                            extent = node3.Extent;
                        }
                    }
                    path.Reverse();
                    memberPath = new System.Data.Mapping.ViewGeneration.Structures.MemberPath(extent, path, this.MetadataWorkspace);
                }
                this.m_memberPath = memberPath;
                return this.m_memberPath;
            }
        }

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_workspace;

        internal static Occurrences MinOccurs =>
            Occurrences.Once;

        internal abstract EdmType NodeType { get; }

        internal enum Occurrences
        {
            Zero,
            Once,
            Many
        }
    }
}

