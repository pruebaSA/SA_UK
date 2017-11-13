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
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class MemberDomainMap : InternalBase
    {
        private Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> m_domainMap;
        private Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> m_nonConditionDomainMap;
        private System.Data.Common.Utils.Set<MemberPath> m_projectedConditionMembers;
        private MetadataWorkspace m_workspace;

        private MemberDomainMap(Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> domainMap, Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> nonConditionDomainMap, MetadataWorkspace workspace)
        {
            this.m_projectedConditionMembers = new System.Data.Common.Utils.Set<MemberPath>();
            this.m_domainMap = domainMap;
            this.m_workspace = workspace;
            this.m_nonConditionDomainMap = nonConditionDomainMap;
        }

        internal MemberDomainMap(ViewTarget viewTarget, IEnumerable<Cell> extentCells, MetadataWorkspace workspace, ConfigViewGenerator config, Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> inheritanceGraph)
        {
            this.m_projectedConditionMembers = new System.Data.Common.Utils.Set<MemberPath>();
            this.m_domainMap = new Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>(MemberPath.EqualityComparer);
            this.m_workspace = workspace;
            Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> dictionary = null;
            if (viewTarget == ViewTarget.UpdateView)
            {
                dictionary = CellConstantDomain.ComputeConstantDomainSetsForSlotsInUpdateViews(extentCells, workspace);
            }
            else
            {
                dictionary = CellConstantDomain.ComputeConstantDomainSetsForSlotsInQueryViews(extentCells, workspace);
            }
            foreach (Cell cell in extentCells)
            {
                foreach (OneOfConst @const in cell.GetLeftQuery(viewTarget).GetConjunctsFromWhereClause())
                {
                    MemberPath memberPath = @const.Slot.MemberPath;
                    if (!this.m_domainMap.ContainsKey(memberPath))
                    {
                        System.Data.Common.Utils.Set<CellConstant> set;
                        if (!dictionary.TryGetValue(memberPath, out set))
                        {
                            set = CellConstantDomain.CreateDomainSetFromMemberPath(memberPath, this.m_workspace);
                        }
                        if (!set.Contains(CellConstant.NotNull) && @const.Values.Values.All<CellConstant>(conditionConstant => conditionConstant.Equals(CellConstant.NotNull)))
                        {
                            continue;
                        }
                        if ((set.Count <= 0) || (!set.Contains(CellConstant.Null) && @const.Values.Values.Contains<CellConstant>(CellConstant.Null)))
                        {
                            string message = System.Data.Entity.Strings.ViewGen_InvalidCondition_0(memberPath.PathToString(false));
                            ErrorLog.Record errorRecord = new ErrorLog.Record(true, ViewGenErrorCode.InvalidCondition, message, cell, string.Empty);
                            ExceptionHelpers.ThrowMappingException(errorRecord, config);
                        }
                        if (!memberPath.IsAlwaysDefined(inheritanceGraph))
                        {
                            set.Add(CellConstant.Undefined);
                        }
                        this.AddToDomainMap(memberPath, set);
                    }
                }
            }
            this.m_nonConditionDomainMap = new Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>(MemberPath.EqualityComparer);
            foreach (Cell cell2 in extentCells)
            {
                foreach (JoinTreeSlot slot in cell2.GetLeftQuery(viewTarget).GetAllQuerySlots())
                {
                    MemberPath key = slot.MemberPath;
                    if (!this.m_domainMap.ContainsKey(key) && !this.m_nonConditionDomainMap.ContainsKey(key))
                    {
                        System.Data.Common.Utils.Set<CellConstant> domain = CellConstantDomain.CreateDomainSetFromMemberPath(key, this.m_workspace);
                        if (!key.IsAlwaysDefined(inheritanceGraph))
                        {
                            domain.Add(CellConstant.Undefined);
                        }
                        domain = CellConstantDomain.NormalizeDomain(domain, domain);
                        this.m_nonConditionDomainMap.Add(key, new CellConstantSetInfo(domain, slot));
                    }
                }
            }
        }

        private static void AddNegatedConstantsIfNeeded(Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> domainMapForMembers)
        {
            foreach (MemberPath path in domainMapForMembers.Keys)
            {
                System.Data.Common.Utils.Set<CellConstant> source = domainMapForMembers[path];
                if ((!MetadataHelper.HasDiscreteDomain(path.EdmType) && path.IsScalarType()) && !source.Any<CellConstant>(c => (c is NegatedCellConstant)))
                {
                    NegatedCellConstant element = new NegatedCellConstant(source);
                    source.Add(element);
                }
            }
        }

        internal void AddNegatedConstantsToPossibleValues()
        {
            AddNegatedConstantsIfNeeded(this.m_domainMap);
        }

        internal void AddSentinel(MemberPath path)
        {
            this.GetDomainInternal(path).Add(CellConstant.AllOtherConstants);
        }

        private void AddToDomainMap(MemberPath member, IEnumerable<CellConstant> domainValues)
        {
            System.Data.Common.Utils.Set<CellConstant> set;
            if (!this.m_domainMap.TryGetValue(member, out set))
            {
                set = new System.Data.Common.Utils.Set<CellConstant>(CellConstant.EqualityComparer);
            }
            set.Unite(domainValues);
            this.m_domainMap[member] = CellConstantDomain.NormalizeDomain(set, set);
        }

        internal IEnumerable<MemberPath> ConditionMembers(EntitySetBase extent)
        {
            foreach (MemberPath iteratorVariable0 in this.m_domainMap.Keys)
            {
                if (iteratorVariable0.Extent.Equals(extent))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        internal void FixEnumerableDomains(ConfigViewGenerator config)
        {
            FixEnumerableDomainsInMap(this.m_domainMap, config, this.m_workspace);
            FixEnumerableDomainsInMap(this.m_nonConditionDomainMap, config, this.m_workspace);
        }

        private static void FixEnumerableDomainsInMap(Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> domainMap, ConfigViewGenerator config, MetadataWorkspace workspace)
        {
            foreach (MemberPath path in domainMap.Keys)
            {
                if (MetadataHelper.HasDiscreteDomain(path.EdmType))
                {
                    System.Data.Common.Utils.Set<CellConstant> other = CellConstantDomain.CreateDomainSetFromMemberPath(path, workspace);
                    System.Data.Common.Utils.Set<CellConstant> set2 = domainMap[path].Difference(other);
                    set2.Remove(CellConstant.Undefined);
                    if (set2.Count > 0)
                    {
                        if (config.IsNormalTracing)
                        {
                            Helpers.FormatTraceLine("Changed domain of {0} from {1} - subtract {2}", new object[] { path, domainMap[path], set2 });
                        }
                        domainMap[path].Subtract(set2);
                    }
                }
            }
        }

        internal void FixQueryDomainMap(IEnumerable<Cell> cells, MemberDomainMap updateDomainMap)
        {
            foreach (Cell cell in cells)
            {
                CellQuery cQuery = cell.CQuery;
                CellQuery sQuery = cell.SQuery;
                for (int i = 0; i < cQuery.NumProjectedSlots; i++)
                {
                    JoinTreeSlot slot = cQuery.ProjectedSlotAt(i) as JoinTreeSlot;
                    JoinTreeSlot slot2 = sQuery.ProjectedSlotAt(i) as JoinTreeSlot;
                    if ((slot != null) && (slot2 != null))
                    {
                        MemberPath memberPath = slot.MemberPath;
                        MemberPath path = slot2.MemberPath;
                        System.Data.Common.Utils.Set<CellConstant> domainInternal = this.GetDomainInternal(memberPath);
                        System.Data.Common.Utils.Set<CellConstant> set2 = updateDomainMap.GetDomainInternal(path);
                        domainInternal.Unite(from constant in set2
                            where !constant.IsNull() && !(constant is NegatedCellConstant)
                            select constant);
                        if (updateDomainMap.IsConditionMember(path) && !this.IsConditionMember(memberPath))
                        {
                            this.m_projectedConditionMembers.Add(memberPath);
                        }
                    }
                }
            }
            List<MemberPath> list = new List<MemberPath>(this.m_domainMap.Keys);
            foreach (MemberPath path3 in list)
            {
                System.Data.Common.Utils.Set<CellConstant> domain = this.m_domainMap[path3];
                this.m_domainMap[path3] = CellConstantDomain.NormalizeDomain(domain, domain);
            }
            list = new List<MemberPath>(this.m_nonConditionDomainMap.Keys);
            foreach (MemberPath path4 in list)
            {
                CellConstantSetInfo info = (CellConstantSetInfo) this.m_nonConditionDomainMap[path4];
                System.Data.Common.Utils.Set<CellConstant> iconstants = CellConstantDomain.NormalizeDomain(info, info);
                CellConstantSetInfo info2 = (CellConstantSetInfo) this.m_nonConditionDomainMap[path4];
                this.m_nonConditionDomainMap[path4] = new CellConstantSetInfo(iconstants, info2.slot);
            }
        }

        internal IEnumerable<CellConstant> GetDomain(MemberPath path) => 
            this.GetDomainInternal(path);

        private System.Data.Common.Utils.Set<CellConstant> GetDomainInternal(MemberPath path)
        {
            System.Data.Common.Utils.Set<CellConstant> set;
            if (!this.m_domainMap.TryGetValue(path, out set))
            {
                set = this.m_nonConditionDomainMap[path];
            }
            return set;
        }

        internal MemberDomainMap GetOpenDomain()
        {
            Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> domainMapForMembers = this.m_domainMap.ToDictionary<KeyValuePair<MemberPath, System.Data.Common.Utils.Set<CellConstant>>, MemberPath, System.Data.Common.Utils.Set<CellConstant>>(p => p.Key, p => new System.Data.Common.Utils.Set<CellConstant>(p.Value, CellConstant.EqualityComparer));
            AddNegatedConstantsIfNeeded(domainMapForMembers);
            return new MemberDomainMap(domainMapForMembers, this.m_nonConditionDomainMap, this.m_workspace);
        }

        internal bool IsConditionMember(MemberPath path) => 
            this.m_domainMap.ContainsKey(path);

        internal bool IsProjectedConditionMember(MemberPath memberPath) => 
            this.m_projectedConditionMembers.Contains(memberPath);

        internal MemberDomainMap MakeCopy() => 
            new MemberDomainMap(this.m_domainMap.ToDictionary<KeyValuePair<MemberPath, System.Data.Common.Utils.Set<CellConstant>>, MemberPath, System.Data.Common.Utils.Set<CellConstant>>(p => p.Key, p => new System.Data.Common.Utils.Set<CellConstant>(p.Value, CellConstant.EqualityComparer)), this.m_nonConditionDomainMap, this.m_workspace);

        internal IEnumerable<MemberPath> NonConditionMembers(EntitySetBase extent)
        {
            foreach (MemberPath iteratorVariable0 in this.m_nonConditionDomainMap.Keys)
            {
                if (iteratorVariable0.Extent.Equals(extent))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        internal void RemoveSentinel(MemberPath path)
        {
            this.GetDomainInternal(path).Remove(CellConstant.AllOtherConstants);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            foreach (MemberPath path in this.m_domainMap.Keys)
            {
                builder.Append('(');
                path.ToCompactString(builder);
                IEnumerable<CellConstant> domain = this.GetDomain(path);
                builder.Append(": ");
                StringUtil.ToCommaSeparatedStringSorted(builder, domain);
                builder.Append(") ");
            }
        }

        internal void UpdateConditionMemberDomain(MemberPath path, IEnumerable<CellConstant> domainValues)
        {
            System.Data.Common.Utils.Set<CellConstant> set = this.m_domainMap[path];
            set.Clear();
            set.Unite(domainValues);
        }



        private class CellConstantSetInfo : System.Data.Common.Utils.Set<CellConstant>
        {
            internal JoinTreeSlot slot;

            internal CellConstantSetInfo(System.Data.Common.Utils.Set<CellConstant> iconstants, JoinTreeSlot islot) : base(iconstants)
            {
                this.slot = islot;
            }

            public override string ToString() => 
                base.ToString();
        }
    }
}

