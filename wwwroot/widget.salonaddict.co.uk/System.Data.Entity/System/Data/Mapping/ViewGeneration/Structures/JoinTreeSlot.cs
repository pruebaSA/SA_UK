namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class JoinTreeSlot : ProjectedSlot
    {
        private System.Data.Mapping.ViewGeneration.Structures.JoinTreeNode m_node;
        internal static readonly IEqualityComparer<JoinTreeSlot> SpecificEqualityComparer = new JoinTreeSlotEqualityComparer();

        internal JoinTreeSlot(System.Data.Mapping.ViewGeneration.Structures.JoinTreeNode node)
        {
            this.m_node = node;
        }

        internal override StringBuilder AsCql(StringBuilder builder, System.Data.Mapping.ViewGeneration.Structures.MemberPath outputMember, string blockAlias, int indentLevel)
        {
            TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(this.MemberPath.LastMember);
            TypeUsage usage2 = Helper.GetModelTypeUsage(outputMember.LastMember);
            if (!modelTypeUsage.EdmType.Equals(usage2.EdmType))
            {
                builder.Append("CAST(");
                this.MemberPath.AsCql(builder, blockAlias);
                builder.Append(" AS ");
                CqlWriter.AppendEscapedTypeName(builder, usage2.EdmType);
                builder.Append(')');
                return builder;
            }
            this.MemberPath.AsCql(builder, blockAlias);
            return builder;
        }

        protected override int GetHash() => 
            System.Data.Mapping.ViewGeneration.Structures.MemberPath.EqualityComparer.GetHashCode(this.m_node.MemberPath);

        internal static List<JoinTreeSlot> GetKeySlots(IEnumerable<JoinTreeSlot> slots, System.Data.Mapping.ViewGeneration.Structures.MemberPath prefix)
        {
            EntitySet entitySet = prefix.EntitySet;
            List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(prefix, entitySet.ElementType);
            return GetSlots(slots, keysForEntityType[0].KeyFields);
        }

        internal static JoinTreeSlot GetSlotForMember(IEnumerable<ProjectedSlot> slots, System.Data.Mapping.ViewGeneration.Structures.MemberPath member)
        {
            foreach (JoinTreeSlot slot in slots)
            {
                if (System.Data.Mapping.ViewGeneration.Structures.MemberPath.EqualityComparer.Equals(slot.MemberPath, member))
                {
                    return slot;
                }
            }
            return null;
        }

        internal static List<JoinTreeSlot> GetSlots(IEnumerable<JoinTreeSlot> slots, IEnumerable<System.Data.Mapping.ViewGeneration.Structures.MemberPath> members)
        {
            List<JoinTreeSlot> list = new List<JoinTreeSlot>();
            foreach (System.Data.Mapping.ViewGeneration.Structures.MemberPath path in members)
            {
                JoinTreeSlot slotForMember = GetSlotForMember(Helpers.AsSuperTypeList<JoinTreeSlot, ProjectedSlot>(slots), path);
                if (slotForMember == null)
                {
                    return null;
                }
                list.Add(slotForMember);
            }
            return list;
        }

        protected override bool IsEqualTo(ProjectedSlot right)
        {
            JoinTreeSlot slot = right as JoinTreeSlot;
            if (slot == null)
            {
                return false;
            }
            return System.Data.Mapping.ViewGeneration.Structures.MemberPath.EqualityComparer.Equals(this.m_node.MemberPath, slot.m_node.MemberPath);
        }

        internal override ProjectedSlot RemapSlot(Dictionary<System.Data.Mapping.ViewGeneration.Structures.JoinTreeNode, System.Data.Mapping.ViewGeneration.Structures.JoinTreeNode> remap) => 
            new JoinTreeSlot(remap[this.JoinTreeNode]);

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_node.ToCompactString(builder);
        }

        internal string ToUserString() => 
            this.m_node.MemberPath.PathToString(false);

        internal System.Data.Mapping.ViewGeneration.Structures.JoinTreeNode JoinTreeNode =>
            this.m_node;

        internal System.Data.Mapping.ViewGeneration.Structures.MemberPath MemberPath =>
            this.m_node.MemberPath;

        internal class JoinTreeSlotEqualityComparer : IEqualityComparer<JoinTreeSlot>
        {
            public bool Equals(JoinTreeSlot left, JoinTreeSlot right) => 
                ProjectedSlot.EqualityComparer.Equals(left, right);

            public int GetHashCode(JoinTreeSlot key) => 
                ProjectedSlot.EqualityComparer.GetHashCode(key);
        }
    }
}

