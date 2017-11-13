namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class SlotInfo : InternalBase
    {
        private bool m_enforceNotNull;
        private bool m_isProjected;
        private bool m_isRequiredByParent;
        private System.Data.Mapping.ViewGeneration.Structures.MemberPath m_memberPath;
        private ProjectedSlot m_slot;

        internal SlotInfo(bool isRequiredByParent, bool isProjected, ProjectedSlot slot, System.Data.Mapping.ViewGeneration.Structures.MemberPath memberPath) : this(isRequiredByParent, isProjected, slot, memberPath, false)
        {
        }

        internal SlotInfo(bool isRequiredByParent, bool isProjected, ProjectedSlot slot, System.Data.Mapping.ViewGeneration.Structures.MemberPath memberPath, bool enforceNotNull)
        {
            this.m_isRequiredByParent = isRequiredByParent;
            this.m_isProjected = isProjected;
            this.m_slot = slot;
            this.m_memberPath = memberPath;
            this.m_enforceNotNull = enforceNotNull;
        }

        internal StringBuilder AsCql(StringBuilder builder, string blockAlias, int indentLevel)
        {
            if (this.m_enforceNotNull)
            {
                builder.Append('(');
                this.m_slot.AsCql(builder, this.m_memberPath, blockAlias, indentLevel);
                builder.Append(" AND ");
                this.m_slot.AsCql(builder, this.m_memberPath, blockAlias, indentLevel);
                builder.Append(" IS NOT NULL)");
                return builder;
            }
            this.m_slot.AsCql(builder, this.m_memberPath, blockAlias, indentLevel);
            return builder;
        }

        internal void ResetIsRequiredByParent()
        {
            this.m_isRequiredByParent = false;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            if (this.m_slot != null)
            {
                builder.Append(this.CqlFieldAlias);
            }
        }

        internal string CqlFieldAlias =>
            this.m_slot.CqlFieldAlias(this.m_memberPath);

        internal bool IsEnforcedNotNull =>
            this.m_enforceNotNull;

        internal bool IsProjected =>
            this.m_isProjected;

        internal bool IsRequiredByParent =>
            this.m_isRequiredByParent;

        internal System.Data.Mapping.ViewGeneration.Structures.MemberPath MemberPath =>
            this.m_memberPath;

        internal ProjectedSlot SlotValue =>
            this.m_slot;
    }
}

