namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.Structures;

    internal class BasicKeyConstraint : KeyConstraint<BasicCellRelation, JoinTreeSlot>
    {
        internal BasicKeyConstraint(BasicCellRelation relation, IEnumerable<JoinTreeSlot> keySlots) : base(relation, keySlots, JoinTreeSlot.SpecificEqualityComparer)
        {
        }

        internal ViewKeyConstraint Propagate()
        {
            ViewCellRelation viewCellRelation = base.CellRelation.ViewCellRelation;
            List<ViewCellSlot> keySlots = new List<ViewCellSlot>();
            foreach (JoinTreeSlot slot in base.KeySlots)
            {
                ViewCellSlot viewSlot = viewCellRelation.LookupViewSlot(slot);
                if (viewSlot == null)
                {
                    return null;
                }
                keySlots.Add(viewSlot);
            }
            return new ViewKeyConstraint(viewCellRelation, keySlots);
        }
    }
}

