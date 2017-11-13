namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class ExtentCqlBlock : CqlBlock
    {
        private static readonly List<CqlBlock> EmptyChildren = new List<CqlBlock>();
        private EntitySetBase m_extent;
        private string m_nodeTableAlias;

        internal ExtentCqlBlock(EntitySetBase extent, SlotInfo[] slots, BoolExpression whereClause, CqlIdentifiers identifiers, int blockAliasNum) : base(slots, EmptyChildren, whereClause, identifiers, blockAliasNum)
        {
            this.m_extent = extent;
            this.m_nodeTableAlias = identifiers.GetBlockAlias();
        }

        internal override StringBuilder AsCql(StringBuilder builder, bool isTopLevel, int indentLevel)
        {
            base.GenerateProjectedtList(builder, indentLevel, this.m_nodeTableAlias, false);
            builder.Append("FROM ");
            CqlWriter.AppendEscapedQualifiedName(builder, this.m_extent.EntityContainer.Name, this.m_extent.Name);
            builder.Append(" AS ").Append(this.m_nodeTableAlias);
            if (!BoolExpression.EqualityComparer.Equals(base.WhereClause, BoolExpression.True))
            {
                StringUtil.IndentNewLine(builder, indentLevel);
                builder.Append("WHERE ");
                base.WhereClause.AsCql(builder, this.m_nodeTableAlias);
            }
            return builder;
        }
    }
}

