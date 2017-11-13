namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Common.QueryCache;
    using System.Data.Entity;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    internal sealed class ObjectQueryExecutionPlan
    {
        private readonly EntitySet _singleEntitySet;
        internal readonly DbCommandDefinition CommandDefinition;
        internal readonly System.Data.Objects.MergeOption MergeOption;
        internal readonly ShaperFactory ResultShaperFactory;
        internal readonly TypeUsage ResultType;

        internal ObjectQueryExecutionPlan(DbCommandDefinition commandDefinition, ShaperFactory resultShaperFactory, TypeUsage resultType, System.Data.Objects.MergeOption mergeOption, EntitySet singleEntitySet)
        {
            this.CommandDefinition = commandDefinition;
            this.ResultShaperFactory = resultShaperFactory;
            this.ResultType = resultType;
            this.MergeOption = mergeOption;
            this._singleEntitySet = singleEntitySet;
        }

        internal ObjectResult<TResultType> Execute<TResultType>(ObjectContext context, ObjectParameterCollection parameterValues)
        {
            DbDataReader reader = null;
            ObjectResult<TResultType> result;
            try
            {
                TypeUsage typeUsage;
                EntityCommandDefinition commandDefinition = (EntityCommandDefinition) this.CommandDefinition;
                EntityCommand entityCommand = new EntityCommand((EntityConnection) context.Connection, commandDefinition);
                if (context.CommandTimeout.HasValue)
                {
                    entityCommand.CommandTimeout = context.CommandTimeout.Value;
                }
                if (parameterValues != null)
                {
                    foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) parameterValues)
                    {
                        int index = entityCommand.Parameters.IndexOf(parameter.Name);
                        if (index != -1)
                        {
                            entityCommand.Parameters[index].Value = parameter.Value ?? DBNull.Value;
                        }
                    }
                }
                reader = commandDefinition.ExecuteStoreCommands(entityCommand, CommandBehavior.Default);
                Shaper<TResultType> shaper = ((ShaperFactory<TResultType>) this.ResultShaperFactory).Create(reader, context, context.MetadataWorkspace, this.MergeOption);
                if (this.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
                {
                    typeUsage = ((CollectionType) this.ResultType.EdmType).TypeUsage;
                }
                else
                {
                    typeUsage = this.ResultType;
                }
                result = new ObjectResult<TResultType>(shaper, this._singleEntitySet, typeUsage);
            }
            catch (Exception)
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                throw;
            }
            return result;
        }

        internal static ObjectResult<TResultType> ExecuteCommandTree<TResultType>(ObjectContext context, DbQueryCommandTree query, System.Data.Objects.MergeOption mergeOption) => 
            Prepare(context, query, typeof(TResultType), mergeOption, null).Execute<TResultType>(context, null);

        internal static ObjectQueryExecutionPlan Prepare(ObjectContext context, DbQueryCommandTree tree, Type elementType, System.Data.Objects.MergeOption mergeOption, Span span)
        {
            SpanIndex index;
            TypeUsage resultType = tree.Query.ResultType;
            DbExpression newQuery = null;
            if (ObjectSpanRewriter.TryRewrite(tree.Query, span, mergeOption, out newQuery, out index))
            {
                tree.Query = newQuery;
            }
            else
            {
                index = null;
            }
            DbConnection connection = context.Connection;
            DbCommandDefinition commandDefinition = null;
            if (connection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.ObjectQuery_InvalidConnection);
            }
            DbProviderServices providerServices = DbProviderServices.GetProviderServices(connection);
            try
            {
                commandDefinition = providerServices.CreateCommandDefinition(tree);
            }
            catch (EntityCommandCompilationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.CommandCompilation(Strings.EntityClient_CommandDefinitionPreparationFailed, exception);
                }
                throw;
            }
            if (commandDefinition == null)
            {
                throw EntityUtil.ProviderDoesNotSupportCommandTrees();
            }
            EntityCommandDefinition definition2 = (EntityCommandDefinition) commandDefinition;
            QueryCacheManager queryCacheManager = context.Perspective.MetadataWorkspace.GetQueryCacheManager();
            ShaperFactory resultShaperFactory = ShaperFactory.Create(elementType, queryCacheManager, definition2.CreateColumnMap(null), context.MetadataWorkspace, index, mergeOption, false);
            EntitySet singleEntitySet = null;
            if ((resultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType) && (definition2.EntitySets != null))
            {
                foreach (EntitySet set2 in definition2.EntitySets)
                {
                    if ((set2 != null) && set2.ElementType.IsAssignableFrom(((CollectionType) resultType.EdmType).TypeUsage.EdmType))
                    {
                        if (singleEntitySet == null)
                        {
                            singleEntitySet = set2;
                        }
                        else
                        {
                            singleEntitySet = null;
                            break;
                        }
                    }
                }
            }
            return new ObjectQueryExecutionPlan(commandDefinition, resultShaperFactory, resultType, mergeOption, singleEntitySet);
        }

        internal string ToTraceString()
        {
            string str = string.Empty;
            EntityCommandDefinition commandDefinition = this.CommandDefinition as EntityCommandDefinition;
            if (commandDefinition != null)
            {
                str = commandDefinition.ToTraceString();
            }
            return str;
        }
    }
}

