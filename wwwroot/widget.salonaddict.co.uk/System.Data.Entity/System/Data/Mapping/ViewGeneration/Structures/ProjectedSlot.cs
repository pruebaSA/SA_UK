namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Runtime.InteropServices;
    using System.Text;

    internal abstract class ProjectedSlot : InternalBase
    {
        internal static readonly IEqualityComparer<ProjectedSlot> EqualityComparer = new ProjectedSlotComparer();

        protected ProjectedSlot()
        {
        }

        internal abstract StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias, int indentLevel);
        internal static int BoolIndexToSlot(int boolIndex, MemberPathMapBase projectedSlotMap, int numBoolSlots) => 
            (projectedSlotMap.Count + boolIndex);

        internal virtual string CqlFieldAlias(MemberPath outputMember) => 
            outputMember.CqlFieldAlias;

        protected virtual int GetHash()
        {
            ExceptionHelpers.CheckAndThrowRes(false, () => Strings.ViewGen_Internal_Error);
            return 0;
        }

        internal static MemberPath GetMemberPath(int slotNum, MemberPathMapBase projectedSlotMap, int numBoolSlots) => 
            (IsBoolSlot(slotNum, projectedSlotMap, numBoolSlots) ? null : projectedSlotMap[slotNum]);

        internal static bool IsBoolSlot(int slotNum, MemberPathMapBase projectedSlotMap, int numBoolSlots) => 
            (slotNum >= projectedSlotMap.Count);

        protected virtual bool IsEqualTo(ProjectedSlot right)
        {
            ExceptionHelpers.CheckAndThrowRes(false, () => Strings.ViewGen_Internal_Error);
            return false;
        }

        internal static bool IsKeySlot(int slotNum, MemberPathMapBase projectedSlotMap, int numBoolSlots) => 
            ((slotNum < projectedSlotMap.Count) && projectedSlotMap[slotNum].IsPartOfKey);

        internal virtual ProjectedSlot MakeAliasedSlot(CqlBlock block, MemberPath outputPath, int slotNum) => 
            new AliasedSlot(block, this, outputPath, slotNum);

        internal virtual ProjectedSlot RemapSlot(Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            ExceptionHelpers.CheckAndThrowRes(false, () => Strings.ViewGen_Internal_Error);
            return null;
        }

        private static ProjectedSlot[] RemapSlots(ProjectedSlot[] oldSlots, Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            ProjectedSlot[] slotArray = new ProjectedSlot[oldSlots.Length];
            for (int i = 0; i < slotArray.Length; i++)
            {
                ProjectedSlot slot = oldSlots[i];
                if (slot != null)
                {
                    slotArray[i] = slot.RemapSlot(remap);
                }
            }
            return slotArray;
        }

        internal static int SlotToBoolIndex(int slotNum, MemberPathMapBase projectedSlotMap, int numBoolSlots) => 
            (slotNum - projectedSlotMap.Count);

        internal static bool TryMergeRemapSlots(ProjectedSlot[] slots1, ProjectedSlot[] slots2, Dictionary<JoinTreeNode, JoinTreeNode> remap, out ProjectedSlot[] result)
        {
            ProjectedSlot[] slotArray;
            if (!TryMergeSlots(slots1, slots2, out slotArray))
            {
                result = null;
                return false;
            }
            result = RemapSlots(slotArray, remap);
            return true;
        }

        private static bool TryMergeSlots(ProjectedSlot[] slots1, ProjectedSlot[] slots2, out ProjectedSlot[] slots)
        {
            slots = new ProjectedSlot[slots1.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                ProjectedSlot slot = slots1[i];
                ProjectedSlot slot2 = slots2[i];
                if (slot == null)
                {
                    slots[i] = slot2;
                }
                else if (slot2 == null)
                {
                    slots[i] = slot;
                }
                else
                {
                    JoinTreeSlot x = slot as JoinTreeSlot;
                    JoinTreeSlot y = slot2 as JoinTreeSlot;
                    if (((x != null) && (y != null)) && !EqualityComparer.Equals(x, y))
                    {
                        return false;
                    }
                    slots[i] = (x != null) ? slot : slot2;
                }
            }
            return true;
        }

        internal class ProjectedSlotComparer : IEqualityComparer<ProjectedSlot>
        {
            public bool Equals(ProjectedSlot left, ProjectedSlot right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.IsEqualTo(right)));

            public int GetHashCode(ProjectedSlot key)
            {
                EntityUtil.CheckArgumentNull<ProjectedSlot>(key, "key");
                return key.GetHash();
            }
        }
    }
}

