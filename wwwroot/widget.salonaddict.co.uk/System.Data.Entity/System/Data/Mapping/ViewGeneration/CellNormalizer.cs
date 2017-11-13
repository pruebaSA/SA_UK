namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class CellNormalizer : InternalBase
    {
        private List<LeftCellWrapper> m_cellWrappers;
        private ConfigViewGenerator m_config;
        private StorageEntityContainerMapping m_entityContainerMapping;
        private EntitySetBase m_extent;
        private System.Data.Mapping.ViewGeneration.Structures.CqlIdentifiers m_identifiers;
        private FragmentQueryProcessor m_leftFragmentQP;
        private System.Data.Mapping.ViewGeneration.Structures.MemberMaps m_memberMaps;
        private Dictionary<FragmentQuery, Tile<FragmentQuery>> m_rewritingCache;
        private FragmentQueryProcessor m_rightFragmentQP;
        private System.Data.Mapping.ViewGeneration.SchemaContext m_schemaContext;
        private MetadataWorkspace m_workspace;

        internal CellNormalizer(EntitySetBase extent, IEnumerable<Cell> extentCells, System.Data.Mapping.ViewGeneration.SchemaContext schemaContext, System.Data.Mapping.ViewGeneration.Structures.CqlIdentifiers identifiers, ConfigViewGenerator config, MemberDomainMap queryDomainMap, MemberDomainMap updateDomainMap, StorageEntityContainerMapping entityContainerMapping, MetadataWorkspace workspace)
        {
            this.m_extent = extent;
            this.m_schemaContext = schemaContext;
            this.m_config = config;
            this.m_workspace = workspace;
            this.m_entityContainerMapping = entityContainerMapping;
            this.m_identifiers = identifiers;
            ValidateCells(extent, extentCells, schemaContext.ViewTarget);
            updateDomainMap = updateDomainMap.MakeCopy();
            MemberDomainMap domainMap = (schemaContext.ViewTarget == ViewTarget.QueryView) ? queryDomainMap : updateDomainMap;
            SignatureGenerator generator = new SignatureGenerator(extent, this.m_workspace);
            this.m_memberMaps = new System.Data.Mapping.ViewGeneration.Structures.MemberMaps(schemaContext, generator.ProjectedSlotMap, queryDomainMap, updateDomainMap);
            FragmentQueryKB kb = new FragmentQueryKB();
            kb.CreateVariableConstraints(extent, domainMap, workspace);
            this.m_leftFragmentQP = new FragmentQueryProcessor(kb);
            this.m_rewritingCache = new Dictionary<FragmentQuery, Tile<FragmentQuery>>(FragmentQuery.GetEqualityComparer(this.m_leftFragmentQP));
            if (this.CreateLeftCellWrappers(extentCells, schemaContext.ViewTarget))
            {
                FragmentQueryKB ykb2 = new FragmentQueryKB();
                MemberDomainMap map2 = (schemaContext.ViewTarget == ViewTarget.QueryView) ? updateDomainMap : queryDomainMap;
                foreach (LeftCellWrapper wrapper in this.m_cellWrappers)
                {
                    EntitySetBase rightExtent = wrapper.RightExtent;
                    ykb2.CreateVariableConstraints(rightExtent, map2, workspace);
                    ykb2.CreateAssociationConstraints(rightExtent, map2, workspace);
                }
                this.m_rightFragmentQP = new FragmentQueryProcessor(ykb2);
                if (this.m_schemaContext.ViewTarget == ViewTarget.QueryView)
                {
                    this.CheckConcurrencyControlTokens();
                }
                this.m_cellWrappers.Sort(LeftCellWrapper.Comparer);
            }
        }

        private static List<Cell> AlignFields(IEnumerable<Cell> cells, MemberPathMapBase projectedSlotMap, ViewTarget viewTarget)
        {
            List<Cell> list = new List<Cell>();
            foreach (Cell cell in cells)
            {
                CellQuery query3;
                CellQuery query4;
                CellQuery leftQuery = cell.GetLeftQuery(viewTarget);
                CellQuery rightQuery = cell.GetRightQuery(viewTarget);
                leftQuery.CreateFieldAlignedCellQueries(rightQuery, projectedSlotMap, out query3, out query4);
                Cell item = (viewTarget == ViewTarget.QueryView) ? Cell.CreateCS(query3, query4, cell.CellLabel, cell.CellNumber) : Cell.CreateCS(query4, query3, cell.CellLabel, cell.CellNumber);
                list.Add(item);
            }
            return list;
        }

        private void CheckConcurrencyControlTokens()
        {
            EntityTypeBase elementType = this.m_extent.ElementType;
            Set<EdmMember> concurrencyMembersForTypeHierarchy = MetadataHelper.GetConcurrencyMembersForTypeHierarchy(elementType, this.m_workspace);
            Set<MemberPath> other = new Set<MemberPath>(MemberPath.EqualityComparer);
            foreach (EdmMember member in concurrencyMembersForTypeHierarchy)
            {
                if (!member.DeclaringType.Equals(elementType))
                {
                    string message = Strings.ViewGen_Concurrency_Derived_Class_2(member.Name, member.DeclaringType.Name, this.m_extent);
                    ErrorLog.Record errorRecord = new ErrorLog.Record(true, ViewGenErrorCode.ConcurrencyDerivedClass, message, this.m_cellWrappers, string.Empty);
                    ExceptionHelpers.ThrowMappingException(errorRecord, this.m_config);
                }
                other.Add(new MemberPath(this.m_extent, member, this.m_schemaContext.MetadataWorkspace));
            }
            if (concurrencyMembersForTypeHierarchy.Count > 0)
            {
                foreach (LeftCellWrapper wrapper in this.m_cellWrappers)
                {
                    Set<MemberPath> members = new Set<MemberPath>(from oneOf in wrapper.OnlyInputCell.CQuery.WhereClause.OneOfConstVariables select oneOf.Slot.MemberPath, MemberPath.EqualityComparer);
                    members.Intersect(other);
                    if (members.Count > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine(Strings.ViewGen_Concurrency_Invalid_Condition_1(MemberPath.PropertiesToUserString(members, false), this.m_extent.Name));
                        ErrorLog.Record record2 = new ErrorLog.Record(true, ViewGenErrorCode.ConcurrencyTokenHasCondition, builder.ToString(), new LeftCellWrapper[] { wrapper }, string.Empty);
                        ExceptionHelpers.ThrowMappingException(record2, this.m_config);
                    }
                }
            }
        }

        private bool CreateLeftCellWrappers(IEnumerable<Cell> extentCells, ViewTarget viewTarget)
        {
            List<Cell> cells = new List<Cell>(extentCells);
            List<Cell> list2 = AlignFields(cells, this.m_memberMaps.ProjectedSlotMap, viewTarget);
            this.m_cellWrappers = new List<LeftCellWrapper>();
            for (int i = 0; i < list2.Count; i++)
            {
                Cell cell = list2[i];
                CellQuery leftQuery = cell.GetLeftQuery(viewTarget);
                CellQuery rightQuery = cell.GetRightQuery(viewTarget);
                Set<MemberPath> nonNullSlots = leftQuery.GetNonNullSlots();
                FragmentQuery query = FragmentQuery.Create(BoolExpression.CreateLiteral(new CellIdBoolean(this.m_identifiers, cells[i].CellNumber), this.m_memberMaps.LeftDomainMap), leftQuery);
                if (viewTarget == ViewTarget.UpdateView)
                {
                    query = this.m_leftFragmentQP.CreateDerivedViewBySelectingConstantAttributes(query) ?? query;
                }
                LeftCellWrapper item = new LeftCellWrapper(this.m_schemaContext, nonNullSlots, query, rightQuery, this.m_memberMaps, cells[i]);
                this.m_cellWrappers.Add(item);
            }
            return true;
        }

        internal bool IsEmpty(CellTreeNode n) => 
            n.IsEmptyRightFragmentQuery;

        internal void SetCachedRewriting(FragmentQuery query, Tile<FragmentQuery> rewriting)
        {
            this.m_rewritingCache[query] = rewriting;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            LeftCellWrapper.WrappersToStringBuilder(builder, this.m_cellWrappers, "Left Celll Wrappers");
        }

        internal bool TryGetCachedRewriting(FragmentQuery query, out Tile<FragmentQuery> rewriting) => 
            this.m_rewritingCache.TryGetValue(query, out rewriting);

        private static void ValidateCells(EntitySetBase extent, IEnumerable<Cell> extentCells, ViewTarget viewTarget)
        {
            foreach (Cell cell in extentCells)
            {
                cell.CheckRepInvariant();
                EntitySetBase base2 = cell.GetLeftQuery(viewTarget).Extent;
                ExceptionHelpers.CheckAndThrowResArgs(extent.Equals(base2), new Func<object, object, string>(Strings.ViewGen_InputCells_NotIsolated_1), extent.ElementType.Name, base2.ElementType.Name);
            }
        }

        internal List<LeftCellWrapper> AllWrappersForExtent =>
            this.m_cellWrappers;

        internal ConfigViewGenerator Config =>
            this.m_config;

        internal System.Data.Mapping.ViewGeneration.Structures.CqlIdentifiers CqlIdentifiers =>
            this.m_identifiers;

        internal StorageEntityContainerMapping EntityContainerMapping =>
            this.m_entityContainerMapping;

        internal EntitySetBase Extent =>
            this.m_extent;

        internal FragmentQueryProcessor LeftFragmentQP =>
            this.m_leftFragmentQP;

        internal System.Data.Mapping.ViewGeneration.Structures.MemberMaps MemberMaps =>
            this.m_memberMaps;

        internal FragmentQueryProcessor RightFragmentQP =>
            this.m_rightFragmentQP;

        internal System.Data.Mapping.ViewGeneration.SchemaContext SchemaContext =>
            this.m_schemaContext;

        internal MetadataWorkspace Workspace =>
            this.m_workspace;
    }
}

