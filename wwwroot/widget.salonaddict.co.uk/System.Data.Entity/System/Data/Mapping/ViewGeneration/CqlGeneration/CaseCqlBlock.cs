namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class CaseCqlBlock : CqlBlock
    {
        private SlotInfo m_caseSlotInfo;

        internal CaseCqlBlock(SlotInfo[] slots, int caseSlot, CqlBlock child, BoolExpression whereClause, CqlIdentifiers identifiers, int blockAliasNum) : base(slots, new List<CqlBlock>(new CqlBlock[] { child }), whereClause, identifiers, blockAliasNum)
        {
            this.m_caseSlotInfo = slots[caseSlot];
        }

        internal override StringBuilder AsCql(StringBuilder builder, bool isTopLevel, int indentLevel)
        {
            StringUtil.IndentNewLine(builder, indentLevel);
            builder.Append("SELECT ");
            if (isTopLevel)
            {
                builder.Append("VALUE ");
            }
            builder.Append("-- Constructing ").Append(this.m_caseSlotInfo.MemberPath.LastComponentName);
            CqlBlock block = base.Children[0];
            this.m_caseSlotInfo.AsCql(builder, block.CqlAlias, indentLevel);
            if (!isTopLevel)
            {
                builder.Append(" AS ").Append(this.m_caseSlotInfo.CqlFieldAlias);
            }
            bool flag = true;
            foreach (SlotInfo info in base.Slots)
            {
                if (info.IsRequiredByParent && !object.ReferenceEquals(info, this.m_caseSlotInfo))
                {
                    if (flag)
                    {
                        builder.Append(", ");
                        StringUtil.IndentNewLine(builder, indentLevel + 1);
                    }
                    else
                    {
                        builder.Append(", ");
                    }
                    info.AsCql(builder, block.CqlAlias, indentLevel);
                    flag = false;
                }
            }
            StringUtil.IndentNewLine(builder, indentLevel);
            builder.Append("FROM (");
            block.AsCql(builder, false, indentLevel + 1);
            StringUtil.IndentNewLine(builder, indentLevel);
            builder.Append(") AS ").Append(block.CqlAlias);
            if (!BoolExpression.EqualityComparer.Equals(base.WhereClause, BoolExpression.True))
            {
                StringUtil.IndentNewLine(builder, indentLevel);
                builder.Append("WHERE ");
                base.WhereClause.AsCql(builder, block.CqlAlias);
            }
            return builder;
        }
    }
}

