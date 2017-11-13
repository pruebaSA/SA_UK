namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class ViewKeyConstraint : KeyConstraint<ViewCellRelation, ViewCellSlot>
    {
        internal ViewKeyConstraint(ViewCellRelation relation, IEnumerable<ViewCellSlot> keySlots) : base(relation, keySlots, ViewCellSlot.SpecificEqualityComparer)
        {
        }

        internal static ErrorLog.Record GetErrorRecord(IEnumerable<ViewKeyConstraint> rightKeyConstraints, MetadataWorkspace workspace)
        {
            ViewKeyConstraint constraint = null;
            string str2;
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (ViewKeyConstraint constraint2 in rightKeyConstraints)
            {
                string str = ViewCellSlot.SlotsToUserString(constraint2.KeySlots, true);
                if (!flag)
                {
                    builder.Append("; ");
                }
                flag = false;
                builder.Append(str);
                constraint = constraint2;
            }
            List<ViewCellSlot> list = new List<ViewCellSlot>(constraint.KeySlots);
            EntitySetBase extent = list[0].SSlot.MemberPath.Extent;
            EntitySetBase base3 = list[0].CSlot.MemberPath.Extent;
            MemberPath prefix = new MemberPath(extent, workspace);
            ExtentKey primaryKeyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(prefix, (EntityType) extent.ElementType);
            if (base3 is EntitySet)
            {
                str2 = Strings.ViewGen_KeyConstraint_Update_Violation_EntitySet_3(builder.ToString(), base3.Name, primaryKeyForEntityType.ToUserString(), extent.Name);
            }
            else
            {
                str2 = Strings.ViewGen_KeyConstraint_Update_Violation_AssociationSet_2(base3.Name, primaryKeyForEntityType.ToUserString(), extent.Name);
            }
            return new ErrorLog.Record(true, ViewGenErrorCode.KeyConstraintUpdateViolation, str2, constraint.CellRelation.Cell, StringUtil.FormatInvariant("PROBLEM: Not implied {0}", new object[] { constraint }));
        }

        internal static ErrorLog.Record GetErrorRecord(ViewKeyConstraint rightKeyConstraint, MetadataWorkspace workspace)
        {
            List<ViewCellSlot> list = new List<ViewCellSlot>(rightKeyConstraint.KeySlots);
            EntitySetBase extent = list[0].SSlot.MemberPath.Extent;
            EntitySetBase base3 = list[0].CSlot.MemberPath.Extent;
            MemberPath prefix = new MemberPath(extent, workspace);
            MemberPath path2 = new MemberPath(base3, workspace);
            ExtentKey primaryKeyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(prefix, (EntityType) extent.ElementType);
            ExtentKey keyForRelationType = null;
            if (base3 is EntitySet)
            {
                keyForRelationType = ExtentKey.GetPrimaryKeyForEntityType(path2, (EntityType) base3.ElementType);
            }
            else
            {
                keyForRelationType = ExtentKey.GetKeyForRelationType(path2, (AssociationType) base3.ElementType);
            }
            string message = Strings.ViewGen_KeyConstraint_Violation_5(extent.Name, ViewCellSlot.SlotsToUserString(rightKeyConstraint.KeySlots, false), primaryKeyForEntityType.ToUserString(), base3.Name, ViewCellSlot.SlotsToUserString(rightKeyConstraint.KeySlots, true), keyForRelationType.ToUserString());
            return new ErrorLog.Record(true, ViewGenErrorCode.KeyConstraintViolation, message, rightKeyConstraint.CellRelation.Cell, StringUtil.FormatInvariant("PROBLEM: Not implied {0}", new object[] { rightKeyConstraint }));
        }

        internal bool Implies(ViewKeyConstraint second)
        {
            if (!object.ReferenceEquals(base.CellRelation, second.CellRelation))
            {
                return false;
            }
            if (!base.KeySlots.IsSubsetOf(second.KeySlots))
            {
                Set<ViewCellSlot> set = new Set<ViewCellSlot>(second.KeySlots);
                foreach (ViewCellSlot slot in base.KeySlots)
                {
                    bool flag = false;
                    foreach (ViewCellSlot slot2 in set)
                    {
                        if (ProjectedSlot.EqualityComparer.Equals(slot.SSlot, slot2.SSlot))
                        {
                            MemberPath memberPath = slot.CSlot.MemberPath;
                            MemberPath y = slot2.CSlot.MemberPath;
                            if (MemberPath.EqualityComparer.Equals(memberPath, y) || memberPath.IsEquivalentViaRefConstraint(y))
                            {
                                set.Remove(slot2);
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal System.Data.Mapping.ViewGeneration.Structures.Cell Cell =>
            base.CellRelation.Cell;
    }
}

