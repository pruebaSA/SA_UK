namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class ViewGenerator : InternalBase
    {
        private System.Data.Common.Utils.Set<Cell> m_cellGroup;
        private ConfigViewGenerator m_config;
        private StorageEntityContainerMapping m_entityContainerMapping;
        private List<ForeignConstraint> m_foreignKeyConstraints;
        private MemberDomainMap m_queryDomainMap;
        private Dictionary<EntitySetBase, QueryRewriter> m_queryRewriterMap;
        private MemberDomainMap m_updateDomainMap;
        private MetadataWorkspace m_workSpace;

        private ViewGenerator(System.Data.Common.Utils.Set<Cell> cellGroup, ConfigViewGenerator config, List<ForeignConstraint> foreignKeyConstraints, StorageEntityContainerMapping entityContainerMapping, MetadataWorkspace workSpace)
        {
            this.m_cellGroup = cellGroup;
            this.m_config = config;
            this.m_workSpace = workSpace;
            this.m_queryRewriterMap = new Dictionary<EntitySetBase, QueryRewriter>();
            this.m_foreignKeyConstraints = foreignKeyConstraints;
            this.m_entityContainerMapping = entityContainerMapping;
            Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> inheritanceGraph = MetadataHelper.BuildUndirectedGraphOfTypes(this.m_workSpace);
            this.m_queryDomainMap = new MemberDomainMap(ViewTarget.QueryView, cellGroup, this.m_workSpace, this.m_config, inheritanceGraph);
            this.m_updateDomainMap = new MemberDomainMap(ViewTarget.UpdateView, cellGroup, this.m_workSpace, this.m_config, inheritanceGraph);
            this.m_queryDomainMap.FixQueryDomainMap(cellGroup, this.m_updateDomainMap);
            FixCellConstantDomains(ViewTarget.QueryView, cellGroup, this.m_queryDomainMap, this.m_config);
            FixCellConstantDomains(ViewTarget.UpdateView, cellGroup, this.m_updateDomainMap, this.m_config);
            MemberDomainMap openDomain = this.m_queryDomainMap.GetOpenDomain();
            MemberDomainMap domainMap = this.m_updateDomainMap.GetOpenDomain();
            foreach (Cell cell in cellGroup)
            {
                cell.CQuery.WhereClause.FixDomainMap(openDomain);
                cell.SQuery.WhereClause.FixDomainMap(domainMap);
                cell.CQuery.WhereClause.ExpensiveSimplify();
                cell.SQuery.WhereClause.ExpensiveSimplify();
                cell.CQuery.WhereClause.FixDomainMap(this.m_queryDomainMap);
                cell.SQuery.WhereClause.FixDomainMap(this.m_updateDomainMap);
            }
        }

        private void CheckForeignKeyConstraints(ErrorLog errorLog, MetadataWorkspace workspace)
        {
            foreach (ForeignConstraint constraint in this.m_foreignKeyConstraints)
            {
                QueryRewriter rewriter = null;
                QueryRewriter rewriter2 = null;
                this.m_queryRewriterMap.TryGetValue(constraint.ChildTable, out rewriter);
                this.m_queryRewriterMap.TryGetValue(constraint.ParentTable, out rewriter2);
                constraint.CheckConstraint(this.m_cellGroup, rewriter, rewriter2, errorLog, this.m_config, workspace);
            }
        }

        private static bool DoesGroupContainExtent(System.Data.Common.Utils.Set<Cell> group, EntitySetBase entity)
        {
            foreach (Cell cell in group)
            {
                if (cell.GetLeftQuery(ViewTarget.QueryView).Extent.Equals(entity))
                {
                    return true;
                }
            }
            return false;
        }

        private static ErrorLog EnsureAllCSpaceContainerSetsAreMapped(IEnumerable<Cell> cells, ConfigViewGenerator config, StorageEntityContainerMapping containerMapping)
        {
            System.Data.Common.Utils.Set<EntitySetBase> set = new System.Data.Common.Utils.Set<EntitySetBase>();
            EntityContainer entityContainer = null;
            foreach (Cell cell in cells)
            {
                set.Add(cell.CQuery.Extent);
                string sourceLocation = cell.CellLabel.SourceLocation;
                entityContainer = cell.CQuery.Extent.EntityContainer;
            }
            List<EntitySetBase> list = new List<EntitySetBase>();
            foreach (EntitySetBase base2 in entityContainer.BaseEntitySets)
            {
                if (!set.Contains(base2) && !containerMapping.HasQueryViewForSetMap(base2.Name))
                {
                    list.Add(base2);
                }
            }
            ErrorLog log = new ErrorLog();
            if (list.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                bool flag = true;
                foreach (EntitySetBase base3 in list)
                {
                    if (!flag)
                    {
                        builder.Append(", ");
                    }
                    flag = false;
                    builder.Append(base3.Name);
                }
                string message = System.Data.Entity.Strings.ViewGen_Missing_Set_Mapping_0(builder);
                int startLineNumber = -1;
                foreach (Cell cell2 in cells)
                {
                    if ((startLineNumber == -1) || (cell2.CellLabel.StartLineNumber < startLineNumber))
                    {
                        startLineNumber = cell2.CellLabel.StartLineNumber;
                    }
                }
                EdmSchemaError error = new EdmSchemaError(message, 0xbd3, EdmSchemaErrorSeverity.Error, containerMapping.SourceLocation, containerMapping.StartLineNumber, containerMapping.StartLinePosition, null);
                ErrorLog.Record record = new ErrorLog.Record(error);
                log.AddEntry(record);
            }
            return log;
        }

        private static void FixCellConstantDomains(ViewTarget viewTarget, IEnumerable<Cell> extentCells, MemberDomainMap domainMap, ConfigViewGenerator config)
        {
            foreach (Cell cell in extentCells)
            {
                cell.GetLeftQuery(viewTarget).FixCellConstantDomains(domainMap, viewTarget);
            }
            domainMap.FixEnumerableDomains(config);
        }

        private ErrorLog GenerateAllViews(KeyToListMap<EntitySetBase, GeneratedView> views, CqlIdentifiers identifiers)
        {
            if (this.m_config.IsNormalTracing)
            {
                StringBuilder builder = new StringBuilder();
                Cell.CellsToBuilder(builder, this.m_cellGroup);
                Helpers.StringTraceLine(builder.ToString());
            }
            this.m_config.SetTimeForFinishedActivity(PerfType.CellCreation);
            ErrorLog errorLog = new Validator(this.m_cellGroup, this.m_config).Validate(this.m_workSpace);
            if (errorLog.Count > 0)
            {
                errorLog.PrintTrace();
                return errorLog;
            }
            this.m_config.SetTimeForFinishedActivity(PerfType.KeyConstraint);
            SchemaContext schemaContext = new SchemaContext(ViewTarget.UpdateView, this.m_workSpace);
            errorLog = this.GenerateViewsForSchemaContext(schemaContext, identifiers, views);
            if (errorLog.Count > 0)
            {
                return errorLog;
            }
            if (this.m_config.IsValidationEnabled)
            {
                this.CheckForeignKeyConstraints(errorLog, this.m_workSpace);
            }
            this.m_config.SetTimeForFinishedActivity(PerfType.ForeignConstraint);
            if (errorLog.Count > 0)
            {
                errorLog.PrintTrace();
                return errorLog;
            }
            this.m_updateDomainMap.AddNegatedConstantsToPossibleValues();
            SchemaContext context2 = new SchemaContext(ViewTarget.QueryView, this.m_workSpace);
            return this.GenerateViewsForSchemaContext(context2, identifiers, views);
        }

        private ErrorLog GenerateQueryViewForExtentAndSchemaContext(SchemaContext schemaContext, CqlIdentifiers identifiers, KeyToListMap<EntitySetBase, GeneratedView> views, EntitySetBase entity, EntityTypeBase type, ViewGenerationMode mode)
        {
            ErrorLog log = new ErrorLog();
            if (this.m_config.IsViewTracing)
            {
                Helpers.StringTraceLine(string.Empty);
                Helpers.StringTraceLine(string.Empty);
                Helpers.FormatTraceLine("================= Generating {0} Query View for: {1} ===========================", new object[] { (mode == ViewGenerationMode.OfTypeViews) ? "OfType" : "OfTypeOnly", entity.Name });
                Helpers.StringTraceLine(string.Empty);
                Helpers.StringTraceLine(string.Empty);
            }
            try
            {
                CellNormalizer normalizer = this.GetCellNormalizer(entity, schemaContext, identifiers);
                this.GenerateViewsForExtentAndType(type, normalizer, identifiers, views, mode);
            }
            catch (InternalMappingException exception)
            {
                log.Merge(exception.ErrorLog);
            }
            return log;
        }

        private ErrorLog GenerateQueryViewForSingleExtent(KeyToListMap<EntitySetBase, GeneratedView> views, CqlIdentifiers identifiers, EntitySetBase entity, EntityTypeBase type, ViewGenerationMode mode)
        {
            if (this.m_config.IsNormalTracing)
            {
                StringBuilder builder = new StringBuilder();
                Cell.CellsToBuilder(builder, this.m_cellGroup);
                Helpers.StringTraceLine(builder.ToString());
            }
            ErrorLog errorLog = new Validator(this.m_cellGroup, this.m_config).Validate(this.m_workSpace);
            if (errorLog.Count > 0)
            {
                errorLog.PrintTrace();
                return errorLog;
            }
            if (this.m_config.IsValidationEnabled)
            {
                this.CheckForeignKeyConstraints(errorLog, this.m_workSpace);
            }
            if (errorLog.Count > 0)
            {
                errorLog.PrintTrace();
                return errorLog;
            }
            this.m_updateDomainMap.AddNegatedConstantsToPossibleValues();
            SchemaContext schemaContext = new SchemaContext(ViewTarget.QueryView, this.m_workSpace);
            return this.GenerateQueryViewForExtentAndSchemaContext(schemaContext, identifiers, views, entity, type, mode);
        }

        internal static ViewGenResults GenerateQueryViewOfType(StorageEntityContainerMapping containerMapping, MetadataWorkspace workSpace, ConfigViewGenerator config, EntitySetBase entity, EntityTypeBase type, bool includeSubtypes, out bool success)
        {
            List<System.Data.Common.Utils.Set<Cell>> list2;
            EntityUtil.CheckArgumentNull<StorageEntityContainerMapping>(containerMapping, "containerMapping");
            EntityUtil.CheckArgumentNull<ConfigViewGenerator>(config, "config");
            EntityUtil.CheckArgumentNull<EntitySetBase>(entity, "entity");
            EntityUtil.CheckArgumentNull<EntityTypeBase>(type, "type");
            if (config.IsNormalTracing)
            {
                Helpers.StringTraceLine("");
                Helpers.StringTraceLine("<<<<<<<< Generating Query View for Entity [" + entity.Name + "] OfType" + (includeSubtypes ? "" : "Only") + "(" + type.Name + ") >>>>>>>");
            }
            if (containerMapping.GetEntitySetMapping(entity.Name).QueryView != null)
            {
                success = false;
                return null;
            }
            InputForComputingCellGroups args = new InputForComputingCellGroups(containerMapping, config);
            OutputFromComputeCellGroups cellgroups = containerMapping.GetCellgroups(args);
            success = cellgroups.Success;
            if (!success)
            {
                return null;
            }
            List<ForeignConstraint> foreignKeyConstraints = cellgroups.ForeignKeyConstraints;
            list2 = list2 = (from setOfcells in cellgroups.CellGroups select new System.Data.Common.Utils.Set<Cell>(from cell in setOfcells select new Cell(cell))).ToList<System.Data.Common.Utils.Set<Cell>>();
            List<Cell> cells = cellgroups.Cells;
            CqlIdentifiers identifiers = cellgroups.Identifiers;
            ViewGenResults results = new ViewGenResults();
            ErrorLog errorLog = EnsureAllCSpaceContainerSetsAreMapped(cells, config, containerMapping);
            if (errorLog.Count > 0)
            {
                results.AddErrors(errorLog);
                Helpers.StringTraceLine(results.ErrorsToString());
                success = true;
                return results;
            }
            foreach (System.Data.Common.Utils.Set<Cell> set in list2)
            {
                if (DoesGroupContainExtent(set, entity))
                {
                    ViewGenerator generator = null;
                    ErrorLog log2 = new ErrorLog();
                    try
                    {
                        generator = new ViewGenerator(set, config, foreignKeyConstraints, containerMapping, workSpace);
                    }
                    catch (InternalMappingException exception)
                    {
                        log2 = exception.ErrorLog;
                    }
                    if (log2.Count > 0)
                    {
                        break;
                    }
                    ViewGenerationMode mode = includeSubtypes ? ViewGenerationMode.OfTypeViews : ViewGenerationMode.OfTypeOnlyViews;
                    log2 = generator.GenerateQueryViewForSingleExtent(results.Views, identifiers, entity, type, mode);
                    if (log2.Count != 0)
                    {
                        results.AddErrors(log2);
                    }
                }
            }
            success = true;
            return results;
        }

        private CellTreeNode GenerateSimplifiedView(CellTreeNode basicView, List<LeftCellWrapper> usedCells)
        {
            int count = usedCells.Count;
            for (int i = 0; i < count; i++)
            {
                usedCells[i].RightCellQuery.InitializeBoolExpressions(count, i);
            }
            return Simplifier.Simplify(basicView, false);
        }

        private QueryRewriter GenerateViewsForExtent(SchemaContext schemaContext, EntitySetBase extent, CqlIdentifiers identifiers, KeyToListMap<EntitySetBase, GeneratedView> views)
        {
            CellNormalizer normalizer = this.GetCellNormalizer(extent, schemaContext, identifiers);
            QueryRewriter rewriter = null;
            if (this.m_config.GenerateViewsForEachType)
            {
                foreach (EdmType type in MetadataHelper.GetTypeAndSubtypesOf(extent.ElementType, this.m_workSpace, false))
                {
                    if (this.m_config.IsViewTracing && !type.Equals(extent.ElementType))
                    {
                        Helpers.FormatTraceLine("CQL View for {0} and type {1}", new object[] { extent.Name, type.Name });
                    }
                    rewriter = this.GenerateViewsForExtentAndType(type, normalizer, identifiers, views, ViewGenerationMode.OfTypeViews);
                }
            }
            else
            {
                rewriter = this.GenerateViewsForExtentAndType(extent.ElementType, normalizer, identifiers, views, ViewGenerationMode.OfTypeViews);
            }
            if (schemaContext.ViewTarget == ViewTarget.QueryView)
            {
                this.m_config.SetTimeForFinishedActivity(PerfType.QueryViews);
            }
            else
            {
                this.m_config.SetTimeForFinishedActivity(PerfType.UpdateViews);
            }
            this.m_queryRewriterMap[extent] = rewriter;
            return rewriter;
        }

        private QueryRewriter GenerateViewsForExtentAndType(EdmType generatedType, CellNormalizer normalizer, CqlIdentifiers identifiers, KeyToListMap<EntitySetBase, GeneratedView> views, ViewGenerationMode mode)
        {
            QueryRewriter rewriter = new QueryRewriter(generatedType, normalizer, mode);
            rewriter.GenerateViewComponents();
            CellTreeNode basicView = rewriter.BasicView;
            if (this.m_config.IsNormalTracing)
            {
                Helpers.StringTrace("Basic View: ");
                Helpers.StringTraceLine(basicView.ToString());
            }
            CellTreeNode node2 = this.GenerateSimplifiedView(basicView, rewriter.UsedCells);
            if (this.m_config.IsNormalTracing)
            {
                Helpers.StringTraceLine(string.Empty);
                Helpers.StringTrace("Simplified View: ");
                Helpers.StringTraceLine(node2.ToString());
            }
            string cqlString = new CqlGenerator(node2, rewriter.CaseStatements, identifiers, normalizer.MemberMaps.ProjectedSlotMap, rewriter.UsedCells.Count, rewriter.TopLevelWhereClause, this.m_workSpace).GenerateCql();
            GeneratedView view = new GeneratedView(normalizer.Extent, generatedType, cqlString, this.m_workSpace, this.m_config);
            views.Add(normalizer.Extent, view);
            return rewriter;
        }

        internal static ViewGenResults GenerateViewsForSchema(StorageEntityContainerMapping containerMapping, MetadataWorkspace workSpace, ConfigViewGenerator config)
        {
            EntityUtil.CheckArgumentNull<StorageEntityContainerMapping>(containerMapping, "containerMapping");
            EntityUtil.CheckArgumentNull<ConfigViewGenerator>(config, "config");
            CellCreator creator = new CellCreator(containerMapping, workSpace);
            List<Cell> cells = creator.GenerateCells(config);
            CqlIdentifiers identifiers = creator.Identifiers;
            return GenerateViewsForSchemaCells(cells, workSpace, config, identifiers, containerMapping);
        }

        internal static ViewGenResults GenerateViewsForSchemaCells(List<Cell> cells, MetadataWorkspace workSpace, ConfigViewGenerator config, CqlIdentifiers identifiers, StorageEntityContainerMapping containerMapping)
        {
            EntityUtil.CheckArgumentNull<List<Cell>>(cells, "cells");
            EntityUtil.CheckArgumentNull<ConfigViewGenerator>(config, "config");
            EntityContainer storageEntityContainer = containerMapping.StorageEntityContainer;
            ViewGenResults results = new ViewGenResults();
            ErrorLog errorLog = EnsureAllCSpaceContainerSetsAreMapped(cells, config, containerMapping);
            if (errorLog.Count > 0)
            {
                results.AddErrors(errorLog);
                Helpers.StringTraceLine(results.ErrorsToString());
                return results;
            }
            List<ForeignConstraint> foreignConstraints = ForeignConstraint.GetForeignConstraints(storageEntityContainer, workSpace);
            CellPartitioner partitioner = new CellPartitioner(cells, foreignConstraints);
            foreach (System.Data.Common.Utils.Set<Cell> set in partitioner.GroupRelatedCells())
            {
                ViewGenerator generator = null;
                ErrorLog log2 = new ErrorLog();
                try
                {
                    generator = new ViewGenerator(set, config, foreignConstraints, containerMapping, workSpace);
                }
                catch (InternalMappingException exception)
                {
                    log2 = exception.ErrorLog;
                }
                if (log2.Count == 0)
                {
                    log2 = generator.GenerateAllViews(results.Views, identifiers);
                }
                if (log2.Count != 0)
                {
                    results.AddErrors(log2);
                }
            }
            return results;
        }

        private ErrorLog GenerateViewsForSchemaContext(SchemaContext schemaContext, CqlIdentifiers identifiers, KeyToListMap<EntitySetBase, GeneratedView> views)
        {
            bool flag = schemaContext.ViewTarget == ViewTarget.QueryView;
            KeyToListMap<EntitySetBase, Cell> map = GroupCellsByExtent(this.m_cellGroup, schemaContext.ViewTarget);
            ErrorLog log = new ErrorLog();
            foreach (EntitySetBase base2 in map.Keys)
            {
                if (this.m_config.IsViewTracing)
                {
                    Helpers.StringTraceLine(string.Empty);
                    Helpers.StringTraceLine(string.Empty);
                    Helpers.FormatTraceLine("================= Generating {0} View for: {1} ===========================", new object[] { flag ? "Query" : "Update", base2.Name });
                    Helpers.StringTraceLine(string.Empty);
                    Helpers.StringTraceLine(string.Empty);
                }
                try
                {
                    QueryRewriter rewriter = this.GenerateViewsForExtent(schemaContext, base2, identifiers, views);
                    if ((schemaContext.ViewTarget == ViewTarget.UpdateView) && this.m_config.IsValidationEnabled)
                    {
                        if (this.m_config.IsViewTracing)
                        {
                            Helpers.StringTraceLine(string.Empty);
                            Helpers.StringTraceLine(string.Empty);
                            Helpers.FormatTraceLine("----------------- Validation for generated update view for: {0} -----------------", new object[] { base2.Name });
                            Helpers.StringTraceLine(string.Empty);
                            Helpers.StringTraceLine(string.Empty);
                        }
                        new RewritingValidator(rewriter.Normalizer, rewriter.BasicView).Validate();
                    }
                }
                catch (InternalMappingException exception)
                {
                    log.Merge(exception.ErrorLog);
                }
            }
            return log;
        }

        private CellNormalizer GetCellNormalizer(EntitySetBase extent, SchemaContext schemaContext, CqlIdentifiers identifiers)
        {
            QueryRewriter rewriter;
            if (this.m_queryRewriterMap.TryGetValue(extent, out rewriter))
            {
                return rewriter.Normalizer;
            }
            List<Cell> extentCells = new List<Cell>();
            foreach (Cell cell in this.m_cellGroup)
            {
                if (extent == cell.GetLeftQuery(schemaContext.ViewTarget).Extent)
                {
                    extentCells.Add(cell);
                }
            }
            return new CellNormalizer(extent, extentCells, schemaContext, identifiers, this.m_config, this.m_queryDomainMap, this.m_updateDomainMap, this.m_entityContainerMapping, this.m_workSpace);
        }

        private static KeyToListMap<EntitySetBase, Cell> GroupCellsByExtent(IEnumerable<Cell> cells, ViewTarget viewTarget)
        {
            KeyToListMap<EntitySetBase, Cell> map = new KeyToListMap<EntitySetBase, Cell>(EqualityComparer<EntitySetBase>.Default);
            foreach (Cell cell in cells)
            {
                CellQuery leftQuery = cell.GetLeftQuery(viewTarget);
                map.Add(leftQuery.Extent, cell);
            }
            return map;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            Cell.CellsToBuilder(builder, this.m_cellGroup);
        }
    }
}

