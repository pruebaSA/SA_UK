namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class JoinCqlBlock : CqlBlock
    {
        private List<OnClause> m_onClauses;
        private CellTreeOpType m_opType;

        internal JoinCqlBlock(CellTreeOpType opType, SlotInfo[] slotInfos, List<CqlBlock> children, List<OnClause> onClauses, CqlIdentifiers identifiers, int blockAliasNum) : base(slotInfos, children, BoolExpression.True, identifiers, blockAliasNum)
        {
            this.m_opType = opType;
            this.m_onClauses = onClauses;
        }

        internal override StringBuilder AsCql(StringBuilder builder, bool isTopLevel, int indentLevel)
        {
            base.GenerateProjectedtList(builder, indentLevel, base.CqlAlias, true);
            builder.Append("FROM ");
            int num = 0;
            foreach (CqlBlock block in base.Children)
            {
                if (num > 0)
                {
                    StringUtil.IndentNewLine(builder, indentLevel + 1);
                    builder.Append(OpCellTreeNode.OpToCql(this.m_opType));
                }
                builder.Append(" (");
                block.AsCql(builder, false, indentLevel + 1);
                builder.Append(") AS ").Append(block.CqlAlias);
                if (num > 0)
                {
                    StringUtil.IndentNewLine(builder, indentLevel + 1);
                    builder.Append("ON ");
                    this.m_onClauses[num - 1].AsCql(builder);
                }
                num++;
            }
            return builder;
        }

        internal class OnClause : InternalBase
        {
            private List<SingleClause> m_singleClauses = new List<SingleClause>();

            internal OnClause()
            {
            }

            internal void Add(AliasedSlot firstSlot, AliasedSlot secondSlot)
            {
                SingleClause item = new SingleClause(firstSlot, secondSlot);
                this.m_singleClauses.Add(item);
            }

            internal StringBuilder AsCql(StringBuilder builder)
            {
                bool flag = true;
                foreach (SingleClause clause in this.m_singleClauses)
                {
                    if (!flag)
                    {
                        builder.Append(" AND ");
                    }
                    clause.AsCql(builder);
                    flag = false;
                }
                return builder;
            }

            internal override void ToCompactString(StringBuilder builder)
            {
                builder.Append("ON ");
                StringUtil.ToSeparatedString(builder, this.m_singleClauses, " AND ");
            }

            private class SingleClause : InternalBase
            {
                private AliasedSlot m_firstSlot;
                private AliasedSlot m_secondSlot;

                internal SingleClause(AliasedSlot firstSlot, AliasedSlot secondSlot)
                {
                    this.m_firstSlot = firstSlot;
                    this.m_secondSlot = secondSlot;
                }

                internal StringBuilder AsCql(StringBuilder builder)
                {
                    builder.Append(this.m_firstSlot.FullCqlAlias()).Append(" = ").Append(this.m_secondSlot.FullCqlAlias());
                    return builder;
                }

                internal override void ToCompactString(StringBuilder builder)
                {
                    this.m_firstSlot.ToCompactString(builder);
                    builder.Append(" = ");
                    this.m_secondSlot.ToCompactString(builder);
                }
            }
        }
    }
}

