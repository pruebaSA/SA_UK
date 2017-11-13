namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.EntitySql;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Data.Query.PlanCompiler;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class GeneratedView : InternalBase
    {
        private DbQueryCommandTree m_commandTree;
        private ConfigViewGenerator m_config;
        private string m_cqlString;
        private System.Data.Mapping.ViewGeneration.DiscriminatorMap m_discriminatorMap;
        private EntitySetBase m_extent;
        private Node m_internalTreeNode;
        private EdmType m_type;

        internal GeneratedView(EntitySetBase extent, EdmType type, string cqlString, MetadataWorkspace workSpace, ConfigViewGenerator config)
        {
            this.m_extent = extent;
            this.m_type = type;
            this.m_cqlString = cqlString;
            this.m_config = config;
            if (this.m_config.IsViewTracing)
            {
                if (EqualityComparer<EdmType>.Default.Equals(type, extent.ElementType))
                {
                    Helpers.FormatTraceLine("CQL view for {0}", new object[] { extent.Name });
                }
                else
                {
                    Helpers.FormatTraceLine("CQL view for OFTYPE({0}, {1}.{2})", new object[] { extent.Name, type.NamespaceName, type.Name });
                }
                Helpers.StringTraceLine(cqlString);
            }
            if (this.m_config.IsViewTracing)
            {
                this.ParseView(workSpace, false);
            }
        }

        internal DbQueryCommandTree GetCommandTree(MetadataWorkspace workspace)
        {
            if (this.m_commandTree == null)
            {
                this.m_commandTree = this.ParseView(workspace, false);
            }
            return this.m_commandTree;
        }

        internal Node GetInternalTree(MetadataWorkspace workspace)
        {
            if (this.m_internalTreeNode == null)
            {
                Command command = ITreeGenerator.Generate(this.ParseView(workspace, false), this.DiscriminatorMap);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(command.Root.Op.OpType == OpType.PhysicalProject, "Expected a physical projectOp at the root of the tree - found " + command.Root.Op.OpType);
                command.DisableVarVecEnumCaching();
                this.m_internalTreeNode = command.Root.Child0;
            }
            return this.m_internalTreeNode;
        }

        internal IEnumerable<EdmSchemaError> ParseUserSpecifiedView(MetadataWorkspace workspace, StoreItemCollection storeItemCollection, StorageSetMapping setMapping, EntityTypeBase elementType, bool includeSubtypes)
        {
            this.m_commandTree = this.ParseView(workspace, true);
            foreach (EdmSchemaError iteratorVariable0 in ViewValidator.ValidateQueryView(this.m_commandTree, storeItemCollection, setMapping, elementType, includeSubtypes))
            {
                yield return iteratorVariable0;
            }
            CollectionType edmType = this.m_commandTree.Query.ResultType.EdmType as CollectionType;
            if ((edmType == null) || !setMapping.Set.ElementType.IsAssignableFrom(edmType.TypeUsage.EdmType))
            {
                EdmSchemaError iteratorVariable2 = new EdmSchemaError(Strings.Mapping_Invalid_QueryView_Type_1(setMapping.Set.Name), 0x815, EdmSchemaErrorSeverity.Error, setMapping.EntityContainerMapping.SourceLocation, setMapping.StartLineNumber, setMapping.StartLinePosition);
                yield return iteratorVariable2;
            }
        }

        private DbQueryCommandTree ParseView(MetadataWorkspace workspace, bool isUserSpecified)
        {
            System.Data.Mapping.ViewGeneration.DiscriminatorMap map;
            this.m_config.StartSingleWatch(PerfType.ViewParsing);
            ParserOptions.CompilationMode restrictedViewGenerationMode = ParserOptions.CompilationMode.RestrictedViewGenerationMode;
            if (isUserSpecified)
            {
                restrictedViewGenerationMode = ParserOptions.CompilationMode.UserViewGenerationMode;
            }
            DbQueryCommandTree view = (DbQueryCommandTree) ExternalCalls.CompileView(this.CqlString, workspace, restrictedViewGenerationMode);
            if (!isUserSpecified)
            {
                view = ViewSimplifier.SimplifyView(view);
            }
            if ((this.m_extent.BuiltInTypeKind == BuiltInTypeKind.EntitySet) && System.Data.Mapping.ViewGeneration.DiscriminatorMap.TryCreateDiscriminatorMap((EntitySet) this.m_extent, view.Query, out map))
            {
                this.m_discriminatorMap = map;
            }
            this.m_config.StopSingleWatch(PerfType.ViewParsing);
            return view;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("OFTYPE(").Append(this.Extent.Name).Append(", ").Append(this.EntityType.Name).Append(") = ").Append(this.CqlString);
        }

        internal string CqlString =>
            this.m_cqlString;

        internal System.Data.Mapping.ViewGeneration.DiscriminatorMap DiscriminatorMap =>
            this.m_discriminatorMap;

        internal EdmType EntityType =>
            this.m_type;

        internal EntitySetBase Extent =>
            this.m_extent;

    }
}

