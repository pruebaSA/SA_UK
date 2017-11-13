namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class BooleanProjectedSlot : ProjectedSlot
    {
        private BoolExpression m_expr;
        private CellIdBoolean m_originalCell;

        internal BooleanProjectedSlot(BoolExpression expr, CqlIdentifiers identifiers, int originalCellNum)
        {
            this.m_expr = expr;
            this.m_originalCell = new CellIdBoolean(identifiers, originalCellNum);
            BoolLiteral asLiteral = expr.AsLiteral;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias, int indentLevel)
        {
            if (this.m_expr.IsTrue || this.m_expr.IsFalse)
            {
                this.m_expr.AsCql(builder, blockAlias);
                return builder;
            }
            builder.Append("CASE WHEN ");
            this.m_expr.AsCql(builder, blockAlias);
            builder.Append(" THEN True ELSE False END");
            return builder;
        }

        internal override string CqlFieldAlias(MemberPath outputMember) => 
            this.m_originalCell.SlotName;

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.FormatStringBuilder(builder, "<{0}, ", new object[] { this.m_originalCell.SlotName });
            this.m_expr.ToCompactString(builder);
            builder.Append('>');
        }
    }
}

