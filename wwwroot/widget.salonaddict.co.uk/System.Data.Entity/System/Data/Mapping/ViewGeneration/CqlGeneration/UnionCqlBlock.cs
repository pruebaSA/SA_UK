namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class UnionCqlBlock : CqlBlock
    {
        internal UnionCqlBlock(SlotInfo[] slotInfos, List<CqlBlock> children, CqlIdentifiers identifiers, int blockAliasNum) : base(slotInfos, children, BoolExpression.True, identifiers, blockAliasNum)
        {
        }

        internal override StringBuilder AsCql(StringBuilder builder, bool isTopLevel, int indentLevel)
        {
            bool flag = true;
            foreach (CqlBlock block in base.Children)
            {
                if (!flag)
                {
                    StringUtil.IndentNewLine(builder, indentLevel + 1);
                    builder.Append(OpCellTreeNode.OpToCql(CellTreeOpType.Union));
                }
                flag = false;
                builder.Append(" (");
                block.AsCql(builder, isTopLevel, indentLevel + 1);
                builder.Append(')');
            }
            return builder;
        }
    }
}

