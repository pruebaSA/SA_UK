namespace System.Data.EntityClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Data.Query.PlanCompiler;
    using System.Data.Query.ResultAssembly;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class EntityCommandDefinition : DbCommandDefinition
    {
        private readonly IColumnMapGenerator _columnMapGenerator;
        private readonly Set<EntitySet> _entitySets;
        private readonly List<DbCommandDefinition> _mappedCommandDefinitions;
        private static int _objectTypeCount;
        private readonly System.Collections.ObjectModel.ReadOnlyCollection<EntityParameter> _parameters;
        internal readonly int ObjectID = Interlocked.Increment(ref _objectTypeCount);

        internal EntityCommandDefinition(DbProviderFactory storeProviderFactory, DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<DbProviderFactory>(storeProviderFactory, "storeProviderFactory");
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            EntityBid.Trace("<ec.EntityCommandDefinition.ctor|ADV> %d# Constructing. commandTree=%d#\n", this.ObjectID, commandTree.ObjectId);
            DbProviderServices providerServices = DbProviderServices.GetProviderServices(storeProviderFactory);
            try
            {
                if (commandTree.CommandTreeKind == DbCommandTreeKind.Query)
                {
                    ColumnMap map;
                    int num;
                    List<ProviderCommandInfo> providerCommands = new List<ProviderCommandInfo>();
                    System.Data.Query.PlanCompiler.PlanCompiler.Compile(commandTree, out providerCommands, out map, out num, out this._entitySets);
                    this._columnMapGenerator = new ConstantColumnMapGenerator(map, num);
                    this._mappedCommandDefinitions = new List<DbCommandDefinition>(providerCommands.Count);
                    foreach (ProviderCommandInfo info in providerCommands)
                    {
                        DbCommandDefinition item = providerServices.CreateCommandDefinition(info.CommandTree);
                        if (item == null)
                        {
                            throw EntityUtil.ProviderIncompatible(Strings.ProviderReturnedNullForCreateCommandDefinition);
                        }
                        this._mappedCommandDefinitions.Add(item);
                    }
                }
                else
                {
                    DbFunctionCommandTree functionCommandTree = (DbFunctionCommandTree) commandTree;
                    FunctionImportMapping targetFunctionMapping = GetTargetFunctionMapping(functionCommandTree);
                    TypeUsage resultType = this.DetermineStoreResultType(functionCommandTree.MetadataWorkspace, targetFunctionMapping, out this._columnMapGenerator);
                    DbFunctionCommandTree tree2 = new DbFunctionCommandTree(functionCommandTree.MetadataWorkspace, DataSpace.SSpace, targetFunctionMapping.TargetFunction, resultType);
                    foreach (KeyValuePair<string, TypeUsage> pair in functionCommandTree.Parameters)
                    {
                        tree2.AddParameter(pair.Key, pair.Value);
                    }
                    DbCommandDefinition definition2 = providerServices.CreateCommandDefinition(tree2);
                    List<DbCommandDefinition> list2 = new List<DbCommandDefinition>(1) {
                        definition2
                    };
                    this._mappedCommandDefinitions = list2;
                    if (targetFunctionMapping.FunctionImport.EntitySet != null)
                    {
                        this._entitySets = new Set<EntitySet>();
                        this._entitySets.Add(targetFunctionMapping.FunctionImport.EntitySet);
                        this._entitySets.MakeReadOnly();
                    }
                }
                List<EntityParameter> list3 = new List<EntityParameter>();
                foreach (KeyValuePair<string, TypeUsage> pair2 in commandTree.Parameters)
                {
                    EntityParameter parameter = CreateEntityParameterFromQueryParameter(pair2);
                    list3.Add(parameter);
                }
                this._parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EntityParameter>(list3);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.CommandCompilation(Strings.EntityClient_CommandDefinitionPreparationFailed, exception);
                }
                throw;
            }
        }

        internal ColumnMap CreateColumnMap(DbDataReader storeDataReader) => 
            this._columnMapGenerator.CreateColumnMap(storeDataReader);

        public override DbCommand CreateCommand()
        {
            EntityBid.Trace("<ec.EntityCommandDefinition.CreateCommand|ADV> %d#\n", this.ObjectID);
            return new EntityCommand(this);
        }

        private static EntityParameter CreateEntityParameterFromQueryParameter(KeyValuePair<string, TypeUsage> queryParameter)
        {
            EntityParameter parameter = new EntityParameter {
                ParameterName = queryParameter.Key
            };
            DbCommandDefinition.PopulateParameterFromTypeUsage(parameter, queryParameter.Value, false);
            return parameter;
        }

        private TypeUsage DetermineStoreResultType(MetadataWorkspace workspace, FunctionImportMapping mapping, out IColumnMapGenerator columnMapGenerator)
        {
            TypeUsage typeUsage;
            EntityType type;
            EdmFunction functionImport = mapping.FunctionImport;
            if (MetadataHelper.TryGetFunctionImportReturnEntityType(functionImport, out type))
            {
                EntitySet entitySet = functionImport.EntitySet;
                columnMapGenerator = new FunctionColumnMapGenerator(mapping, entitySet, type);
                return mapping.GetExpectedTargetResultType(workspace);
            }
            if ((functionImport.ReturnParameter != null) && (functionImport.ReturnParameter.TypeUsage != null))
            {
                typeUsage = functionImport.ReturnParameter.TypeUsage;
                ScalarColumnMap elementMap = new ScalarColumnMap(((CollectionType) typeUsage.EdmType).TypeUsage, string.Empty, 0, 0);
                SimpleCollectionColumnMap columnMap = new SimpleCollectionColumnMap(typeUsage, string.Empty, elementMap, null, null, null);
                columnMapGenerator = new ConstantColumnMapGenerator(columnMap, 1);
                return typeUsage;
            }
            typeUsage = null;
            columnMapGenerator = new ConstantColumnMapGenerator(null, 0);
            return typeUsage;
        }

        internal DbDataReader Execute(EntityCommand entityCommand, CommandBehavior behavior)
        {
            IntPtr ptr;
            DbDataReader reader3;
            EntityBid.ScopeEnter(out ptr, "<ec.EntityCommandDefinition.Execute|ADV> %d#\n", this.ObjectID);
            try
            {
                if (CommandBehavior.SequentialAccess != (behavior & CommandBehavior.SequentialAccess))
                {
                    throw EntityUtil.MustUseSequentialAccess();
                }
                DbDataReader reader = this.ExecuteStoreCommands(entityCommand, behavior);
                DbDataReader reader2 = null;
                if (reader != null)
                {
                    try
                    {
                        if (this._columnMapGenerator.CreateColumnMap(reader) == null)
                        {
                            CommandHelper.ConsumeReader(reader);
                            reader2 = reader;
                        }
                        else
                        {
                            reader2 = BridgeDataReader.Create(reader, this._columnMapGenerator.CreateColumnMap(reader), entityCommand.Connection.GetMetadataWorkspace());
                        }
                    }
                    catch
                    {
                        reader.Dispose();
                        throw;
                    }
                }
                reader3 = reader2;
            }
            finally
            {
                EntityBid.ScopeLeave(ref ptr);
            }
            return reader3;
        }

        internal DbDataReader ExecuteStoreCommands(EntityCommand entityCommand, CommandBehavior behavior)
        {
            if (1 != this._mappedCommandDefinitions.Count)
            {
                throw EntityUtil.NotSupported("MARS");
            }
            EntityTransaction entityTransaction = CommandHelper.GetEntityTransaction(entityCommand);
            DbCommand storeProviderCommand = this._mappedCommandDefinitions[0].CreateCommand();
            CommandHelper.SetStoreProviderCommandState(entityCommand, entityTransaction, storeProviderCommand);
            bool flag = false;
            if (storeProviderCommand.Parameters != null)
            {
                foreach (DbParameter parameter in storeProviderCommand.Parameters)
                {
                    int index = entityCommand.Parameters.IndexOf(parameter.ParameterName);
                    if (-1 != index)
                    {
                        EntityParameter entityParameter = entityCommand.Parameters[index];
                        SyncParameterProperties(entityParameter, parameter);
                        if (parameter.Direction != ParameterDirection.Input)
                        {
                            flag = true;
                        }
                    }
                }
            }
            if (flag)
            {
                entityCommand.SetStoreProviderCommand(storeProviderCommand);
            }
            DbDataReader reader = null;
            try
            {
                reader = storeProviderCommand.ExecuteReader(behavior & ~CommandBehavior.SequentialAccess);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.CommandExecution(Strings.EntityClient_CommandDefinitionExecutionFailed, exception);
                }
                throw;
            }
            return reader;
        }

        private static FunctionImportMapping GetTargetFunctionMapping(DbFunctionCommandTree functionCommandTree)
        {
            FunctionImportMapping mapping;
            if (!functionCommandTree.MetadataWorkspace.TryGetFunctionImportMapping(functionCommandTree.EdmFunction, out mapping))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_UnmappedFunctionImport);
            }
            return mapping;
        }

        private static void SyncParameterProperties(EntityParameter entityParameter, DbParameter storeParameter)
        {
            IDbDataParameter parameter = storeParameter;
            if (entityParameter.IsDirectionSpecified)
            {
                storeParameter.Direction = entityParameter.Direction;
            }
            if (entityParameter.IsIsNullableSpecified)
            {
                storeParameter.IsNullable = entityParameter.IsNullable;
            }
            if (entityParameter.IsSizeSpecified)
            {
                storeParameter.Size = entityParameter.Size;
            }
            if (entityParameter.IsPrecisionSpecified)
            {
                parameter.Precision = entityParameter.Precision;
            }
            if (entityParameter.IsScaleSpecified)
            {
                parameter.Scale = entityParameter.Scale;
            }
            storeParameter.Value = entityParameter.Value;
        }

        internal string ToTraceString()
        {
            if (this._mappedCommandDefinitions == null)
            {
                return string.Empty;
            }
            if (this._mappedCommandDefinitions.Count == 1)
            {
                return this._mappedCommandDefinitions[0].CreateCommand().CommandText;
            }
            StringBuilder builder = new StringBuilder();
            foreach (DbCommandDefinition definition in this._mappedCommandDefinitions)
            {
                DbCommand command = definition.CreateCommand();
                builder.Append(command.CommandText);
            }
            return builder.ToString();
        }

        internal Set<EntitySet> EntitySets =>
            this._entitySets;

        internal IEnumerable<string> MappedCommands
        {
            get
            {
                List<string> list = new List<string>();
                foreach (DbCommandDefinition definition in this._mappedCommandDefinitions)
                {
                    DbCommand command = definition.CreateCommand();
                    list.Add(command.CommandText);
                }
                return list;
            }
        }

        internal IEnumerable<EntityParameter> Parameters =>
            this._parameters;

        private sealed class ConstantColumnMapGenerator : EntityCommandDefinition.IColumnMapGenerator
        {
            private readonly ColumnMap _columnMap;
            private readonly int _fieldsRequired;

            internal ConstantColumnMapGenerator(ColumnMap columnMap, int fieldsRequired)
            {
                this._columnMap = columnMap;
                this._fieldsRequired = fieldsRequired;
            }

            ColumnMap EntityCommandDefinition.IColumnMapGenerator.CreateColumnMap(DbDataReader reader)
            {
                if ((reader != null) && (reader.FieldCount < this._fieldsRequired))
                {
                    throw EntityUtil.CommandExecution(Strings.EntityClient_TooFewColumns);
                }
                return this._columnMap;
            }
        }

        private sealed class FunctionColumnMapGenerator : EntityCommandDefinition.IColumnMapGenerator
        {
            private readonly EntityType _baseEntityType;
            private readonly EntitySet _entitySet;
            private readonly FunctionImportMapping _mapping;

            internal FunctionColumnMapGenerator(FunctionImportMapping mapping, EntitySet entitySet, EntityType baseEntityType)
            {
                this._mapping = mapping;
                this._entitySet = entitySet;
                this._baseEntityType = baseEntityType;
            }

            ColumnMap EntityCommandDefinition.IColumnMapGenerator.CreateColumnMap(DbDataReader reader) => 
                ColumnMapFactory.CreateFunctionImportEntityColumnMap(reader, this._mapping, this._entitySet, this._baseEntityType);
        }

        private interface IColumnMapGenerator
        {
            ColumnMap CreateColumnMap(DbDataReader reader);
        }
    }
}

