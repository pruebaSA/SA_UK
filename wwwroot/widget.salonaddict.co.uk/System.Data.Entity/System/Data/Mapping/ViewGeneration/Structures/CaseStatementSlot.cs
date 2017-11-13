namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Text;

    internal class CaseStatementSlot : ProjectedSlot
    {
        private CaseStatement m_caseStatement;
        private IEnumerable<WithStatement> m_withStatements;

        internal CaseStatementSlot(CaseStatement statement, IEnumerable<WithStatement> withStatements)
        {
            this.m_caseStatement = statement;
            this.m_withStatements = withStatements;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias, int indentLevel)
        {
            this.m_caseStatement.AsCql(builder, this.m_withStatements, blockAlias, indentLevel);
            return builder;
        }

        internal override ProjectedSlot MakeAliasedSlot(CqlBlock block, MemberPath outputPath, int slotNum) => 
            new CaseStatementSlot(this.m_caseStatement.MakeCaseWithAliasedSlots(block, outputPath, slotNum), null);

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_caseStatement.ToCompactString(builder);
        }
    }
}

