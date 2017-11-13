namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Text;

    internal class QualifiedCellIdBoolean : CellIdBoolean
    {
        private CqlBlock m_block;

        internal QualifiedCellIdBoolean(CqlBlock block, CqlIdentifiers identifiers, int originalCellNum) : base(identifiers, originalCellNum)
        {
            this.m_block = block;
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            string qualifiedName = CqlWriter.GetQualifiedName(this.m_block.CqlAlias, base.SlotName);
            builder.Append(qualifiedName);
            return builder;
        }
    }
}

