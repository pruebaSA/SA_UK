namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class IfJoinedCondition : TrueFalseLiteral
    {
        private JoinTreeSlot m_var;

        internal IfJoinedCondition(JoinTreeNode node)
        {
            this.m_var = new JoinTreeSlot(node);
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            builder;

        internal override StringBuilder AsNegatedUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            builder.Append("NOT(");
            builder = this.AsUserString(builder, blockAlias, canSkipIsNotNull);
            builder.Append(")");
            return builder;
        }

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            this.AsCql(builder, blockAlias, canSkipIsNotNull);

        protected override int GetHash() => 
            ProjectedSlot.EqualityComparer.GetHashCode(this.m_var);

        internal override void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
        {
            MemberPath memberPath = this.Var.MemberPath;
            int index = projectedSlotMap.IndexOf(memberPath);
            requiredSlots[index] = true;
        }

        protected override bool IsEqualTo(BoolLiteral right)
        {
            IfJoinedCondition objB = right as IfJoinedCondition;
            if (objB == null)
            {
                return false;
            }
            return (object.ReferenceEquals(this, objB) || ProjectedSlot.EqualityComparer.Equals(this.m_var, objB.m_var));
        }

        internal override BoolLiteral RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            JoinTreeSlot slot = (JoinTreeSlot) this.m_var.RemapSlot(remap);
            return new IfJoinedCondition(slot.JoinTreeNode);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("IfJoined(");
            this.Var.ToCompactString(builder);
            builder.Append(")");
        }

        internal JoinTreeSlot Var =>
            this.m_var;
    }
}

