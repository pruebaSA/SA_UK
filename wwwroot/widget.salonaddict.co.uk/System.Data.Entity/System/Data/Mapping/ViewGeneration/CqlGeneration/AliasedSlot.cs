namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class AliasedSlot : ProjectedSlot
    {
        private CqlBlock m_block;
        private System.Data.Mapping.ViewGeneration.Structures.MemberPath m_memberPath;
        private ProjectedSlot m_slot;

        internal AliasedSlot(CqlBlock block, ProjectedSlot slot, System.Data.Mapping.ViewGeneration.Structures.MemberPath memberPath, int slotNum)
        {
            this.m_block = block;
            this.m_slot = slot;
            this.m_memberPath = memberPath;
        }

        internal override StringBuilder AsCql(StringBuilder builder, System.Data.Mapping.ViewGeneration.Structures.MemberPath outputMember, string blockAlias, int indentLevel)
        {
            builder.Append(this.FullCqlAlias());
            return builder;
        }

        internal override string CqlFieldAlias(System.Data.Mapping.ViewGeneration.Structures.MemberPath outputMember)
        {
            ProjectedSlot element = this.m_slot;
            Set<ProjectedSlot> set = new Set<ProjectedSlot>();
            while (true)
            {
                set.Add(element);
                AliasedSlot slot2 = element as AliasedSlot;
                if (slot2 == null)
                {
                    break;
                }
                element = slot2.m_slot;
            }
            return element.CqlFieldAlias(this.m_memberPath);
        }

        internal string FullCqlAlias() => 
            CqlWriter.GetQualifiedName(this.m_block.CqlAlias, this.CqlFieldAlias(this.m_memberPath));

        internal override ProjectedSlot MakeAliasedSlot(CqlBlock block, System.Data.Mapping.ViewGeneration.Structures.MemberPath outputPath, int slotNum) => 
            new AliasedSlot(block, this.m_slot, outputPath, slotNum);

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.FormatStringBuilder(builder, "{0} ", new object[] { this.m_block.CqlAlias });
            this.m_slot.ToCompactString(builder);
        }

        internal CqlBlock Block =>
            this.m_block;

        internal ProjectedSlot InnerSlot =>
            this.m_slot;

        internal System.Data.Mapping.ViewGeneration.Structures.MemberPath MemberPath =>
            this.m_memberPath;
    }
}

