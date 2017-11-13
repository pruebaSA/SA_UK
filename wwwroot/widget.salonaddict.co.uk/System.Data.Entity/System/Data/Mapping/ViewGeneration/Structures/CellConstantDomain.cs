namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class CellConstantDomain : InternalBase
    {
        private Set<CellConstant> m_domain;
        private Set<CellConstant> m_possibleValues;

        internal CellConstantDomain(CellConstantDomain domain)
        {
            this.m_domain = new Set<CellConstant>(domain.m_domain, CellConstant.EqualityComparer);
            this.m_possibleValues = new Set<CellConstant>(domain.m_possibleValues, CellConstant.EqualityComparer);
        }

        internal CellConstantDomain(IEnumerable<CellConstant> values, IEnumerable<CellConstant> possibleDiscreteValues)
        {
            this.m_possibleValues = DeterminePossibleValues(values, possibleDiscreteValues);
            this.m_domain = NormalizeDomain(values, this.m_possibleValues);
        }

        internal CellConstantDomain(CellConstant value, IEnumerable<CellConstant> possibleDiscreteValues) : this(new CellConstant[] { value }, possibleDiscreteValues)
        {
        }

        internal override bool CheckRepInvariant() => 
            this.CheckRepInvariantLocal();

        private bool CheckRepInvariantLocal()
        {
            GetNegatedConstant(this.m_domain);
            GetNegatedConstant(this.m_possibleValues);
            return true;
        }

        [Conditional("DEBUG")]
        private static void CheckTwoDomainInvariants(CellConstantDomain domain1, CellConstantDomain domain2)
        {
            domain1.CheckRepInvariant();
            domain2.CheckRepInvariant();
        }

        internal static Dictionary<MemberPath, Set<CellConstant>> ComputeConstantDomainSetsForSlotsInQueryViews(IEnumerable<Cell> cells, MetadataWorkspace workspace)
        {
            Dictionary<MemberPath, Set<CellConstant>> dictionary = new Dictionary<MemberPath, Set<CellConstant>>(MemberPath.EqualityComparer);
            foreach (Cell cell in cells)
            {
                foreach (OneOfConst @const in cell.CQuery.GetConjunctsFromWhereClause())
                {
                    Set<CellConstant> set2;
                    JoinTreeSlot slot = @const.Slot;
                    Set<CellConstant> elements = CreateDomainSetFromSlot(slot, workspace);
                    elements.AddRange(from c in @const.Values.Values
                        where !c.Equals(CellConstant.Null) && !c.Equals(CellConstant.NotNull)
                        select c);
                    if (!dictionary.TryGetValue(slot.MemberPath, out set2))
                    {
                        dictionary[slot.MemberPath] = elements;
                    }
                    else
                    {
                        set2.AddRange(elements);
                    }
                }
            }
            return dictionary;
        }

        internal static Dictionary<MemberPath, Set<CellConstant>> ComputeConstantDomainSetsForSlotsInUpdateViews(IEnumerable<Cell> cells, MetadataWorkspace workspace)
        {
            Dictionary<MemberPath, Set<CellConstant>> dictionary = new Dictionary<MemberPath, Set<CellConstant>>(MemberPath.EqualityComparer);
            foreach (Cell cell in cells)
            {
                CellQuery cQuery = cell.CQuery;
                CellQuery sQuery = cell.SQuery;
                foreach (JoinTreeSlot slot in sQuery.GetAllQuerySlots())
                {
                    CellConstant constant;
                    Set<CellConstant> set4;
                    Set<CellConstant> domain = CreateDomainSetFromSlot(slot, workspace);
                    Set<CellConstant> elements = RestrictDomainByWhereClause(domain, slot, sQuery);
                    if (elements.SetEquals(domain))
                    {
                        int projectedPosition = sQuery.GetProjectedPosition(slot);
                        if (projectedPosition >= 0)
                        {
                            JoinTreeSlot slot2 = cQuery.ProjectedSlotAt(projectedPosition) as JoinTreeSlot;
                            elements = RestrictDomainByWhereClause(CreateDomainSetFromSlot(slot2, workspace), slot2, cQuery);
                        }
                    }
                    MemberPath memberPath = slot.MemberPath;
                    if (TryGetDefaultValueForMemberPath(memberPath, out constant))
                    {
                        elements.Add(constant);
                    }
                    if (!dictionary.TryGetValue(memberPath, out set4))
                    {
                        dictionary[memberPath] = elements;
                    }
                    else
                    {
                        set4.AddRange(elements);
                    }
                }
            }
            return dictionary;
        }

        internal bool Contains(CellConstant constant) => 
            this.m_domain.Contains(constant);

        internal bool ContainsNotNull()
        {
            NegatedCellConstant negatedConstant = GetNegatedConstant(this.m_domain);
            return ((negatedConstant != null) && negatedConstant.Contains(CellConstant.Null));
        }

        private static Set<CellConstant> CreateDomainFromType(EdmType type, MetadataWorkspace workspace)
        {
            PrimitiveType type2 = type as PrimitiveType;
            EnumType enumType = type as EnumType;
            if (type2 != null)
            {
                if (type2.PrimitiveTypeKind == PrimitiveTypeKind.Boolean)
                {
                    return new Set<CellConstant>(CreateList(true, false), CellConstant.EqualityComparer);
                }
                return new Set<CellConstant>(CellConstant.EqualityComparer) { CellConstant.NotNull };
            }
            if (enumType != null)
            {
                return GetEnumeratedDomain(enumType);
            }
            if (Helper.IsRefType(type))
            {
                type = ((RefType) type).ElementType;
            }
            List<CellConstant> elements = new List<CellConstant>();
            foreach (EdmType type4 in MetadataHelper.GetTypeAndSubtypesOf(type, workspace, false))
            {
                TypeConstant item = new TypeConstant(type4);
                elements.Add(item);
            }
            return new Set<CellConstant>(elements, CellConstant.EqualityComparer);
        }

        internal static Set<CellConstant> CreateDomainSetFromMemberPath(MemberPath memberPath, MetadataWorkspace workspace)
        {
            Set<CellConstant> set = CreateDomainFromType(memberPath.EdmType, workspace);
            if (memberPath.IsNullable)
            {
                set.Add(CellConstant.Null);
            }
            return set;
        }

        private static Set<CellConstant> CreateDomainSetFromSlot(JoinTreeSlot slot, MetadataWorkspace workspace) => 
            CreateDomainSetFromMemberPath(slot.MemberPath, workspace);

        private static IEnumerable<CellConstant> CreateList(object value1, object value2)
        {
            yield return new ScalarConstant(value1);
            yield return new ScalarConstant(value2);
        }

        private static Set<CellConstant> DeterminePossibleValues(IEnumerable<CellConstant> domain, bool addNegatedIfPresent)
        {
            Set<CellConstant> values = new Set<CellConstant>(CellConstant.EqualityComparer);
            bool flag = false;
            foreach (CellConstant constant in domain)
            {
                NegatedCellConstant constant2 = constant as NegatedCellConstant;
                if (constant2 != null)
                {
                    flag = true;
                    foreach (CellConstant constant3 in constant2.Elements)
                    {
                        values.Add(constant3);
                    }
                }
                else
                {
                    values.Add(constant);
                }
            }
            if (flag && addNegatedIfPresent)
            {
                NegatedCellConstant element = new NegatedCellConstant(values);
                values.Add(element);
            }
            return values;
        }

        private static Set<CellConstant> DeterminePossibleValues(IEnumerable<CellConstant> domain1, IEnumerable<CellConstant> domain2)
        {
            Set<CellConstant> domain = new Set<CellConstant>(domain1, CellConstant.EqualityComparer);
            domain.Unite(domain2);
            return DeterminePossibleValues(domain, false);
        }

        internal static CellConstant GetDefaultValueForMemberPath(MemberPath memberPath, IEnumerable<LeftCellWrapper> wrappersForErrorReporting, ConfigViewGenerator config)
        {
            CellConstant defaultConstant = null;
            if (!TryGetDefaultValueForMemberPath(memberPath, out defaultConstant))
            {
                string message = Strings.ViewGen_No_Default_Value_1(memberPath.Extent.Name, memberPath.PathToString(false));
                ErrorLog.Record errorRecord = new ErrorLog.Record(true, ViewGenErrorCode.NoDefaultValue, message, wrappersForErrorReporting, string.Empty);
                ExceptionHelpers.ThrowMappingException(errorRecord, config);
            }
            return defaultConstant;
        }

        private static Set<CellConstant> GetEnumeratedDomain(EnumType enumType)
        {
            Set<CellConstant> set = new Set<CellConstant>(CellConstant.EqualityComparer);
            foreach (EnumMember member in enumType.EnumMembers)
            {
                set.Add(new ScalarConstant(member));
            }
            return set;
        }

        internal int GetHash()
        {
            int num = 0;
            foreach (CellConstant constant in this.m_domain)
            {
                num ^= CellConstant.EqualityComparer.GetHashCode(constant);
            }
            return num;
        }

        private static NegatedCellConstant GetNegatedConstant(IEnumerable<CellConstant> constants)
        {
            NegatedCellConstant constant = null;
            foreach (CellConstant constant2 in constants)
            {
                NegatedCellConstant constant3 = constant2 as NegatedCellConstant;
                if (constant3 != null)
                {
                    constant = constant3;
                }
            }
            return constant;
        }

        private CellConstantDomain Intersect(CellConstantDomain second)
        {
            CellConstantDomain domain = new CellConstantDomain(this);
            domain.m_domain.Intersect(second.m_domain);
            return domain;
        }

        internal bool IsEqualTo(CellConstantDomain second) => 
            this.m_domain.SetEquals(second.m_domain);

        internal static Set<CellConstant> NormalizeDomain(IEnumerable<CellConstant> domain, IEnumerable<CellConstant> extraValues)
        {
            Set<CellConstant> values = DeterminePossibleValues(domain, extraValues);
            Set<CellConstant> set2 = new Set<CellConstant>(CellConstant.EqualityComparer);
            foreach (CellConstant constant in domain)
            {
                NegatedCellConstant constant2 = constant as NegatedCellConstant;
                if (constant2 != null)
                {
                    set2.Add(new NegatedCellConstant(values));
                    Set<CellConstant> elements = values.Difference(constant2.Elements);
                    set2.AddRange(elements);
                }
                else
                {
                    set2.Add(constant);
                }
            }
            return set2;
        }

        private static Set<CellConstant> RestrictDomainByWhereClause(IEnumerable<CellConstant> domain, JoinTreeSlot slot, CellQuery cellQuery)
        {
            Set<CellConstant> set = null;
            foreach (OneOfConst @const in cellQuery.GetConjunctsFromWhereClause())
            {
                if (MemberPath.EqualityComparer.Equals(@const.Slot.MemberPath, slot.MemberPath))
                {
                    set = new Set<CellConstant>(@const.Values.Values, CellConstant.EqualityComparer);
                }
            }
            if (set == null)
            {
                return new Set<CellConstant>(domain, CellConstant.EqualityComparer);
            }
            Set<CellConstant> possibleDiscreteValues = DeterminePossibleValues(set, domain);
            CellConstantDomain domain2 = new CellConstantDomain(domain, possibleDiscreteValues);
            CellConstantDomain second = new CellConstantDomain(set, possibleDiscreteValues);
            return new Set<CellConstant>(domain2.Intersect(second).Values, CellConstant.EqualityComparer);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.ToUserString());
        }

        internal string ToUserString()
        {
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (CellConstant constant in this.m_domain)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                builder.Append(constant.ToUserString());
                flag = false;
            }
            return builder.ToString();
        }

        internal static bool TryGetDefaultValueForMemberPath(MemberPath memberPath, out CellConstant defaultConstant)
        {
            object defaultValue = memberPath.DefaultValue;
            defaultConstant = CellConstant.Null;
            if (defaultValue != null)
            {
                defaultConstant = new ScalarConstant(defaultValue);
                return true;
            }
            if (!memberPath.IsNullable && !memberPath.IsComputed)
            {
                return false;
            }
            return true;
        }

        internal IEnumerable<CellConstant> AllPossibleValues =>
            this.AllPossibleValuesInternal;

        private Set<CellConstant> AllPossibleValuesInternal
        {
            get
            {
                NegatedCellConstant constant = new NegatedCellConstant(this.m_possibleValues);
                return this.m_possibleValues.Union(new CellConstant[] { constant });
            }
        }

        internal int Count =>
            this.m_domain.Count;

        internal IEnumerable<CellConstant> Values =>
            this.m_domain;

    }
}

