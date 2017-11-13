namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal class ExtentJoinTreeNode : JoinTreeNode
    {
        private EntitySetBase m_extent;

        internal ExtentJoinTreeNode(EntitySetBase extent, IEnumerable<MemberJoinTreeNode> children, MetadataWorkspace workspace) : base(false, children, workspace)
        {
            this.m_extent = extent;
        }

        protected override JoinTreeNode CreateNodeFromContext(bool nodeIsOptional, List<MemberJoinTreeNode> children) => 
            new ExtentJoinTreeNode(this.Extent, children, base.MetadataWorkspace);

        internal override void GetIdentifiers(CqlIdentifiers identifiers)
        {
            identifiers.AddIdentifier(this.m_extent.Name);
            identifiers.AddIdentifier(this.m_extent.ElementType.Name);
        }

        protected override bool IsSameContext(JoinTreeNode second)
        {
            ExtentJoinTreeNode node = second as ExtentJoinTreeNode;
            return ((node != null) && this.Extent.Equals(node.Extent));
        }

        protected override string ContextName =>
            this.m_extent.Name;

        internal EntitySetBase Extent =>
            this.m_extent;

        internal override EdmType NodeType =>
            this.m_extent.ElementType;
    }
}

