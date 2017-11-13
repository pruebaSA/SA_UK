namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class Cell : InternalBase
    {
        private int m_cellNumber;
        private CellQuery m_cQuery;
        private System.Data.Mapping.ViewGeneration.Structures.CellLabel m_label;
        private CellQuery m_sQuery;
        private ViewCellRelation m_viewCellRelation;

        internal Cell(Cell source)
        {
            this.m_cQuery = new CellQuery(source.m_cQuery);
            this.m_sQuery = new CellQuery(source.m_sQuery);
            this.m_label = new System.Data.Mapping.ViewGeneration.Structures.CellLabel(source.m_label);
            this.m_cellNumber = source.m_cellNumber;
        }

        private Cell(CellQuery cQuery, CellQuery sQuery, System.Data.Mapping.ViewGeneration.Structures.CellLabel label, int cellNumber)
        {
            this.m_cQuery = cQuery;
            this.m_sQuery = sQuery;
            this.m_label = label;
            this.m_cellNumber = cellNumber;
        }

        internal static void CellsToBuilder(StringBuilder builder, IEnumerable<Cell> cells)
        {
            builder.AppendLine();
            builder.AppendLine("=========================================================================");
            foreach (Cell cell in cells)
            {
                builder.AppendLine();
                StringUtil.FormatStringBuilder(builder, "Mapping Cell V{0}:", new object[] { cell.CellNumber });
                builder.AppendLine();
                builder.Append("C: ");
                cell.CQuery.ToFullString(builder);
                builder.AppendLine();
                builder.AppendLine();
                builder.Append("S: ");
                cell.SQuery.ToFullString(builder);
                builder.AppendLine();
            }
        }

        internal override bool CheckRepInvariant()
        {
            this.m_cQuery.CheckRepInvariant(this.m_sQuery);
            return true;
        }

        internal static Cell CreateCS(CellQuery cQuery, CellQuery sQuery, System.Data.Mapping.ViewGeneration.Structures.CellLabel label, int cellNumber) => 
            new Cell(cQuery, sQuery, label, cellNumber);

        internal ViewCellRelation CreateViewCellRelation(int cellNumber)
        {
            if (this.m_viewCellRelation == null)
            {
                this.GenerateCellRelations(cellNumber);
            }
            return this.m_viewCellRelation;
        }

        private void GenerateCellRelations(int cellNumber)
        {
            List<ViewCellSlot> slots = new List<ViewCellSlot>();
            for (int i = 0; i < this.CQuery.NumProjectedSlots; i++)
            {
                ProjectedSlot slot = this.CQuery.ProjectedSlotAt(i);
                ProjectedSlot slot2 = this.SQuery.ProjectedSlotAt(i);
                JoinTreeSlot cSlot = (JoinTreeSlot) slot;
                JoinTreeSlot sSlot = (JoinTreeSlot) slot2;
                ViewCellSlot item = new ViewCellSlot(i, cSlot, sSlot);
                slots.Add(item);
            }
            this.m_viewCellRelation = new ViewCellRelation(this, slots, cellNumber);
        }

        internal Set<EdmProperty> GetCSlotsForTableColumns(IEnumerable<MemberPath> columns)
        {
            List<int> projectedPositions = this.SQuery.GetProjectedPositions(columns);
            if (projectedPositions == null)
            {
                return null;
            }
            Set<EdmProperty> set = new Set<EdmProperty>();
            foreach (int num in projectedPositions)
            {
                JoinTreeSlot slot2 = this.CQuery.ProjectedSlotAt(num) as JoinTreeSlot;
                if (slot2 != null)
                {
                    set.Add((EdmProperty) slot2.MemberPath.LastMember);
                }
                else
                {
                    return null;
                }
            }
            return set;
        }

        internal void GetIdentifiers(CqlIdentifiers identifiers)
        {
            this.m_cQuery.GetIdentifiers(identifiers);
            this.m_sQuery.GetIdentifiers(identifiers);
        }

        internal CellQuery GetLeftQuery(ViewTarget side)
        {
            if (side != ViewTarget.QueryView)
            {
                return this.m_sQuery;
            }
            return this.m_cQuery;
        }

        internal CellQuery GetRightQuery(ViewTarget side)
        {
            if (side != ViewTarget.QueryView)
            {
                return this.m_cQuery;
            }
            return this.m_sQuery;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.CQuery.ToCompactString(builder);
            builder.Append(" = ");
            this.SQuery.ToCompactString(builder);
        }

        internal override void ToFullString(StringBuilder builder)
        {
            this.CQuery.ToFullString(builder);
            builder.Append(" = ");
            this.SQuery.ToFullString(builder);
        }

        public override string ToString() => 
            this.ToFullString();

        internal System.Data.Mapping.ViewGeneration.Structures.CellLabel CellLabel =>
            this.m_label;

        internal int CellNumber =>
            this.m_cellNumber;

        internal string CellNumberAsString =>
            StringUtil.FormatInvariant("V{0}", new object[] { this.CellNumber });

        internal CellQuery CQuery =>
            this.m_cQuery;

        internal CellQuery SQuery =>
            this.m_sQuery;
    }
}

