namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Text;

    internal class ViewCellSlot : ProjectedSlot
    {
        private JoinTreeSlot m_cSlot;
        private int m_slotNum;
        private JoinTreeSlot m_sSlot;
        internal static readonly IEqualityComparer<ViewCellSlot> SpecificEqualityComparer = new ViewCellSlotEqualityComparer();

        internal ViewCellSlot(int slotNum, JoinTreeSlot cSlot, JoinTreeSlot sSlot)
        {
            this.m_slotNum = slotNum;
            this.m_cSlot = cSlot;
            this.m_sSlot = sSlot;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias, int indentLevel)
        {
            ExceptionHelpers.CheckAndThrowRes(false, () => Strings.ViewGen_Internal_Error);
            return null;
        }

        protected override int GetHash() => 
            ((ProjectedSlot.EqualityComparer.GetHashCode(this.m_cSlot) ^ ProjectedSlot.EqualityComparer.GetHashCode(this.m_sSlot)) ^ this.m_slotNum);

        protected override bool IsEqualTo(ProjectedSlot right)
        {
            ViewCellSlot slot = right as ViewCellSlot;
            if (slot == null)
            {
                return false;
            }
            return (((this.m_slotNum == slot.m_slotNum) && ProjectedSlot.EqualityComparer.Equals(this.m_cSlot, slot.m_cSlot)) && ProjectedSlot.EqualityComparer.Equals(this.m_sSlot, slot.m_sSlot));
        }

        internal static string SlotsToUserString(IEnumerable<ViewCellSlot> slots, bool isFromCside)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (ViewCellSlot slot in slots)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                builder.Append(SlotToUserString(slot, isFromCside));
                flag = false;
            }
            return builder.ToString();
        }

        internal static string SlotToUserString(ViewCellSlot slot, bool isFromCside)
        {
            JoinTreeSlot slot2 = isFromCside ? slot.CSlot : slot.SSlot;
            return StringUtil.FormatInvariant("{0}", new object[] { slot2 });
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append('<');
            StringUtil.FormatStringBuilder(builder, "{0}", new object[] { this.m_slotNum });
            builder.Append(':');
            this.m_cSlot.ToCompactString(builder);
            builder.Append('-');
            this.m_sSlot.ToCompactString(builder);
            builder.Append('>');
        }

        internal JoinTreeSlot CSlot =>
            this.m_cSlot;

        internal JoinTreeSlot SSlot =>
            this.m_sSlot;

        internal class ViewCellSlotEqualityComparer : IEqualityComparer<ViewCellSlot>
        {
            public bool Equals(ViewCellSlot left, ViewCellSlot right) => 
                ProjectedSlot.EqualityComparer.Equals(left, right);

            public int GetHashCode(ViewCellSlot key) => 
                ProjectedSlot.EqualityComparer.GetHashCode(key);
        }
    }
}

