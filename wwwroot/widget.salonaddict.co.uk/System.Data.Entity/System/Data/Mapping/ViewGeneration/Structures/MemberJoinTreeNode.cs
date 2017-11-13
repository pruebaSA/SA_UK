namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal class MemberJoinTreeNode : JoinTreeNode
    {
        private EdmMember m_member;

        internal MemberJoinTreeNode(EdmMember value, bool isOptional, IEnumerable<MemberJoinTreeNode> children, MetadataWorkspace workspace) : base(isOptional, children, workspace)
        {
            this.m_member = value;
        }

        protected override JoinTreeNode CreateNodeFromContext(bool nodeIsOptional, List<MemberJoinTreeNode> children) => 
            new MemberJoinTreeNode(this.m_member, nodeIsOptional, children, base.MetadataWorkspace);

        internal override void GetIdentifiers(CqlIdentifiers identifiers)
        {
            identifiers.AddIdentifier(this.m_member.Name);
            identifiers.AddIdentifier(this.m_member.TypeUsage.EdmType.Name);
        }

        protected override bool IsSameContext(JoinTreeNode second)
        {
            MemberJoinTreeNode node = second as MemberJoinTreeNode;
            return ((node != null) && this.Member.Equals(node.Member));
        }

        protected override string ContextName =>
            this.m_member.Name;

        internal EdmMember Member =>
            this.m_member;

        internal override EdmType NodeType =>
            this.m_member.TypeUsage.EdmType;
    }
}

