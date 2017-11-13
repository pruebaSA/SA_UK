namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Text;

    internal class CellIdBoolean : TrueFalseLiteral
    {
        private CqlIdentifiers m_identifiers;
        private int m_index;

        internal CellIdBoolean(CqlIdentifiers identifiers, int index)
        {
            this.m_identifiers = identifiers;
            this.m_index = index;
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            string qualifiedName = CqlWriter.GetQualifiedName(blockAlias, this.SlotName);
            builder.Append(qualifiedName);
            return builder;
        }

        internal override StringBuilder AsNegatedUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            builder.Append("NOT(");
            builder = this.AsUserString(builder, blockAlias, canSkipIsNotNull);
            builder.Append(")");
            return builder;
        }

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            this.AsCql(builder, blockAlias, canSkipIsNotNull);

        internal override bool CheckRepInvariant()
        {
            ExceptionHelpers.CheckAndThrowRes(this.m_index >= 0, () => Strings.ViewGen_CellIdBooleans_Invalid);
            return true;
        }

        protected override int GetHash() => 
            this.m_index.GetHashCode();

        internal override void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
        {
            int numBoolSlots = requiredSlots.Length - projectedSlotMap.Count;
            int index = ProjectedSlot.BoolIndexToSlot(this.m_index, projectedSlotMap, numBoolSlots);
            requiredSlots[index] = true;
        }

        protected override bool IsEqualTo(BoolLiteral right)
        {
            CellIdBoolean flag = right as CellIdBoolean;
            if (flag == null)
            {
                return false;
            }
            return (this.m_index == flag.m_index);
        }

        internal override BoolLiteral RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap) => 
            this;

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.SlotName);
        }

        internal string SlotName =>
            this.m_identifiers.GetFromVariable(this.m_index);
    }
}

