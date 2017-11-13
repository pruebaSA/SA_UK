namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Text;

    internal class KeyConstraint<TCellRelation, TSlot> : InternalBase where TCellRelation: System.Data.Mapping.ViewGeneration.Validation.CellRelation
    {
        private Set<TSlot> m_keySlots;
        private TCellRelation m_relation;

        internal KeyConstraint(TCellRelation relation, IEnumerable<TSlot> keySlots, IEqualityComparer<TSlot> comparer)
        {
            this.m_relation = relation;
            this.m_keySlots = new Set<TSlot>(keySlots, comparer).MakeReadOnly();
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.FormatStringBuilder(builder, "Key (V{0}) - ", new object[] { this.m_relation.CellNumber });
            StringUtil.ToSeparatedStringSorted(builder, this.KeySlots, ", ");
        }

        protected TCellRelation CellRelation =>
            this.m_relation;

        protected Set<TSlot> KeySlots =>
            this.m_keySlots;
    }
}

