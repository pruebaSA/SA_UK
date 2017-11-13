namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal abstract class CqlBlock : InternalBase
    {
        private string m_blockAlias;
        private ReadOnlyCollection<CqlBlock> m_children;
        private ReadOnlyCollection<SlotInfo> m_slots;
        private BoolExpression m_whereClause;

        protected CqlBlock(SlotInfo[] slotInfos, List<CqlBlock> children, BoolExpression whereClause, CqlIdentifiers identifiers, int blockAliasNum)
        {
            this.m_slots = new ReadOnlyCollection<SlotInfo>(slotInfos);
            this.m_children = new ReadOnlyCollection<CqlBlock>(children);
            this.m_whereClause = whereClause;
            this.m_blockAlias = identifiers.GetBlockAlias(blockAliasNum);
        }

        internal abstract StringBuilder AsCql(StringBuilder builder, bool isTopLevel, int indentLevel);
        protected void GenerateProjectedtList(StringBuilder builder, int indentLevel, string blockAlias, bool asForCaseStatementsOnly)
        {
            StringUtil.IndentNewLine(builder, indentLevel);
            builder.Append("SELECT ");
            List<string> list = new List<string>();
            bool flag = true;
            foreach (SlotInfo info in this.Slots)
            {
                if (info.IsRequiredByParent)
                {
                    if (!flag)
                    {
                        builder.Append(", ");
                    }
                    if (!asForCaseStatementsOnly)
                    {
                        StringUtil.IndentNewLine(builder, indentLevel + 1);
                    }
                    info.AsCql(builder, blockAlias, indentLevel);
                    if (((info.SlotValue is CaseStatementSlot) || (info.SlotValue is BooleanProjectedSlot)) || (((info.SlotValue is ConstantSlot) || !asForCaseStatementsOnly) || info.IsEnforcedNotNull))
                    {
                        builder.Append(" AS ").Append(info.CqlFieldAlias);
                    }
                    flag = false;
                }
            }
            StringUtil.ToSeparatedString(builder, list, ", ", null);
            StringUtil.IndentNewLine(builder, indentLevel);
        }

        internal bool IsProjected(int slotNum) => 
            this.m_slots[slotNum].IsProjected;

        internal System.Data.Mapping.ViewGeneration.Structures.ProjectedSlot ProjectedSlot(int slotNum) => 
            this.m_slots[slotNum].SlotValue;

        internal override void ToCompactString(StringBuilder builder)
        {
            for (int i = 0; i < this.m_slots.Count; i++)
            {
                StringUtil.FormatStringBuilder(builder, "{0}: ", new object[] { i });
                this.m_slots[i].ToCompactString(builder);
                builder.Append(' ');
            }
            this.m_whereClause.ToCompactString(builder);
        }

        protected ReadOnlyCollection<CqlBlock> Children =>
            this.m_children;

        internal string CqlAlias =>
            this.m_blockAlias;

        internal ReadOnlyCollection<SlotInfo> Slots
        {
            get => 
                this.m_slots;
            set
            {
                this.m_slots = value;
            }
        }

        protected BoolExpression WhereClause =>
            this.m_whereClause;
    }
}

