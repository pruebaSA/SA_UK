namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class ViewCellRelation : CellRelation
    {
        private System.Data.Mapping.ViewGeneration.Structures.Cell m_cell;
        private List<ViewCellSlot> m_slots;

        internal ViewCellRelation(System.Data.Mapping.ViewGeneration.Structures.Cell cell, List<ViewCellSlot> slots, int cellNumber) : base(cellNumber)
        {
            this.m_cell = cell;
            this.m_slots = slots;
            this.m_cell.CQuery.CreateBasicCellRelation(this);
            this.m_cell.SQuery.CreateBasicCellRelation(this);
        }

        protected override int GetHash() => 
            this.m_cell.GetHashCode();

        internal ViewCellSlot LookupViewSlot(JoinTreeSlot slot)
        {
            foreach (ViewCellSlot slot2 in this.m_slots)
            {
                if (ProjectedSlot.EqualityComparer.Equals(slot, slot2.CSlot) || ProjectedSlot.EqualityComparer.Equals(slot, slot2.SSlot))
                {
                    return slot2;
                }
            }
            return null;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("ViewRel[");
            this.m_cell.ToCompactString(builder);
            builder.Append(']');
        }

        internal System.Data.Mapping.ViewGeneration.Structures.Cell Cell =>
            this.m_cell;
    }
}

