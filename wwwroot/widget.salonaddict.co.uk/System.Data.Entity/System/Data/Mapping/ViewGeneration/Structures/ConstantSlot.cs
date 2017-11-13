namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Text;

    internal class ConstantSlot : ProjectedSlot
    {
        private System.Data.Mapping.ViewGeneration.Structures.CellConstant m_constant;

        internal ConstantSlot(System.Data.Mapping.ViewGeneration.Structures.CellConstant value)
        {
            this.m_constant = value;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias, int indentLevel) => 
            this.m_constant.AsCql(builder, outputMember, blockAlias);

        protected override int GetHash() => 
            System.Data.Mapping.ViewGeneration.Structures.CellConstant.EqualityComparer.GetHashCode(this.m_constant);

        protected override bool IsEqualTo(ProjectedSlot right)
        {
            ConstantSlot slot = right as ConstantSlot;
            if (slot == null)
            {
                return false;
            }
            return System.Data.Mapping.ViewGeneration.Structures.CellConstant.EqualityComparer.Equals(this.m_constant, slot.m_constant);
        }

        internal override ProjectedSlot MakeAliasedSlot(CqlBlock block, MemberPath memberPath, int slotNum) => 
            this;

        internal override ProjectedSlot RemapSlot(Dictionary<JoinTreeNode, JoinTreeNode> remap) => 
            this;

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_constant.ToCompactString(builder);
        }

        internal System.Data.Mapping.ViewGeneration.Structures.CellConstant CellConstant =>
            this.m_constant;
    }
}

