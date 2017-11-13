namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class LeftCellWrapper : InternalBase
    {
        internal static readonly IEqualityComparer<LeftCellWrapper> BoolEqualityComparer = new BoolWrapperComparer();
        internal static readonly IComparer<LeftCellWrapper> Comparer = new LeftCellWrapperComparer();
        private System.Data.Common.Utils.Set<MemberPath> m_attributes;
        private HashSet<Cell> m_cells;
        private System.Data.Mapping.ViewGeneration.QueryRewriting.FragmentQuery m_fragmentQuery;
        private MemberMaps m_memberMaps;
        private CellQuery m_rightCellQuery;
        private System.Data.Mapping.ViewGeneration.SchemaContext m_schemaContext;
        internal static readonly IComparer<LeftCellWrapper> OriginalCellIdComparer = new CellIdComparer();

        internal LeftCellWrapper(System.Data.Mapping.ViewGeneration.SchemaContext schemaContext, System.Data.Common.Utils.Set<MemberPath> attrs, System.Data.Mapping.ViewGeneration.QueryRewriting.FragmentQuery fragmentQuery, CellQuery cellQuery, MemberMaps memberMaps, IEnumerable<Cell> inputCells)
        {
            this.m_fragmentQuery = fragmentQuery;
            this.m_rightCellQuery = cellQuery;
            this.m_attributes = attrs;
            this.m_schemaContext = schemaContext;
            this.m_memberMaps = memberMaps;
            this.m_cells = new HashSet<Cell>(inputCells);
        }

        internal LeftCellWrapper(System.Data.Mapping.ViewGeneration.SchemaContext schemaContext, System.Data.Common.Utils.Set<MemberPath> attrs, System.Data.Mapping.ViewGeneration.QueryRewriting.FragmentQuery fragmentQuery, CellQuery cellQuery, MemberMaps memberMaps, Cell inputCell) : this(schemaContext, attrs, fragmentQuery, cellQuery, memberMaps, new Cell[] { inputCell })
        {
        }

        [Conditional("DEBUG")]
        internal void AssertHasUniqueCell()
        {
        }

        internal RoleBoolean CreateRoleBoolean()
        {
            if (this.RightExtent is AssociationSet)
            {
                System.Data.Common.Utils.Set<AssociationEndMember> endsForTablePrimaryKey = this.GetEndsForTablePrimaryKey();
                if (endsForTablePrimaryKey.Count == 1)
                {
                    return new RoleBoolean(((AssociationSet) this.RightExtent).AssociationSetEnds[endsForTablePrimaryKey.First<AssociationEndMember>().Name]);
                }
            }
            return new RoleBoolean(this.RightExtent);
        }

        private System.Data.Common.Utils.Set<AssociationEndMember> GetEndsForTablePrimaryKey()
        {
            CellQuery rightCellQuery = this.RightCellQuery;
            System.Data.Common.Utils.Set<AssociationEndMember> set = new System.Data.Common.Utils.Set<AssociationEndMember>(EqualityComparer<AssociationEndMember>.Default);
            foreach (int num in this.m_memberMaps.ProjectedSlotMap.KeySlots)
            {
                JoinTreeSlot slot = (JoinTreeSlot) rightCellQuery.ProjectedSlotAt(num);
                AssociationEndMember firstMember = (AssociationEndMember) slot.MemberPath.FirstMember;
                set.Add(firstMember);
            }
            return set;
        }

        internal static string GetExtentListAsUserString(IEnumerable<LeftCellWrapper> wrappers)
        {
            System.Data.Common.Utils.Set<EntitySetBase> set = new System.Data.Common.Utils.Set<EntitySetBase>(EqualityComparer<EntitySetBase>.Default);
            foreach (LeftCellWrapper wrapper in wrappers)
            {
                set.Add(wrapper.RightExtent);
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (EntitySetBase base2 in set)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                flag = false;
                builder.Append(base2.Name);
            }
            return builder.ToString();
        }

        internal static IEnumerable<Cell> GetInputCellsForWrappers(IEnumerable<LeftCellWrapper> wrappers)
        {
            foreach (LeftCellWrapper iteratorVariable0 in wrappers)
            {
                foreach (Cell iteratorVariable1 in iteratorVariable0.m_cells)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        internal override void ToCompactString(StringBuilder stringBuilder)
        {
            stringBuilder.Append(this.OriginalCellNumberString);
        }

        internal override void ToFullString(StringBuilder builder)
        {
            builder.Append("P[");
            StringUtil.ToSeparatedString(builder, this.m_attributes, ",");
            builder.Append("] = ");
            this.m_rightCellQuery.ToFullString(builder);
        }

        internal static void WrappersToStringBuilder(StringBuilder builder, List<LeftCellWrapper> wrappers, string header)
        {
            builder.AppendLine().Append(header).AppendLine();
            LeftCellWrapper[] array = wrappers.ToArray();
            Array.Sort<LeftCellWrapper>(array, OriginalCellIdComparer);
            foreach (LeftCellWrapper wrapper in array)
            {
                wrapper.ToCompactString(builder);
                builder.Append(" = ");
                wrapper.ToFullString(builder);
                builder.AppendLine();
            }
        }

        internal System.Data.Common.Utils.Set<MemberPath> Attributes =>
            this.m_attributes;

        internal IEnumerable<Cell> Cells =>
            this.m_cells;

        internal System.Data.Mapping.ViewGeneration.QueryRewriting.FragmentQuery FragmentQuery =>
            this.m_fragmentQuery;

        internal EntitySetBase LeftExtent =>
            this.m_cells.First<Cell>().GetLeftQuery(this.m_schemaContext.ViewTarget).Extent;

        internal Cell OnlyInputCell =>
            this.m_cells.First<Cell>();

        internal string OriginalCellNumberString =>
            StringUtil.ToSeparatedString(from cell in this.m_cells select cell.CellNumberAsString, "+", "");

        internal CellQuery RightCellQuery =>
            this.m_rightCellQuery;

        internal MemberDomainMap RightDomainMap =>
            this.m_memberMaps.RightDomainMap;

        internal EntitySetBase RightExtent =>
            this.m_rightCellQuery.Extent;

        internal System.Data.Mapping.ViewGeneration.SchemaContext SchemaContext =>
            this.m_schemaContext;


        private class BoolWrapperComparer : IEqualityComparer<LeftCellWrapper>
        {
            public bool Equals(LeftCellWrapper left, LeftCellWrapper right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return true;
                }
                if ((left == null) || (right == null))
                {
                    return false;
                }
                bool flag = BoolExpression.EqualityComparer.Equals(left.RightCellQuery.WhereClause, right.RightCellQuery.WhereClause);
                return (left.RightExtent.Equals(right.RightExtent) && flag);
            }

            public int GetHashCode(LeftCellWrapper wrapper) => 
                (BoolExpression.EqualityComparer.GetHashCode(wrapper.RightCellQuery.WhereClause) ^ wrapper.RightExtent.GetHashCode());
        }

        internal class CellIdComparer : IComparer<LeftCellWrapper>
        {
            public int Compare(LeftCellWrapper x, LeftCellWrapper y) => 
                StringComparer.Ordinal.Compare(x.OriginalCellNumberString, y.OriginalCellNumberString);
        }

        private class LeftCellWrapperComparer : IComparer<LeftCellWrapper>
        {
            public int Compare(LeftCellWrapper x, LeftCellWrapper y)
            {
                if (x.FragmentQuery.Attributes.Count > y.FragmentQuery.Attributes.Count)
                {
                    return -1;
                }
                if (x.FragmentQuery.Attributes.Count < y.FragmentQuery.Attributes.Count)
                {
                    return 1;
                }
                return string.CompareOrdinal(x.OriginalCellNumberString, y.OriginalCellNumberString);
            }
        }
    }
}

