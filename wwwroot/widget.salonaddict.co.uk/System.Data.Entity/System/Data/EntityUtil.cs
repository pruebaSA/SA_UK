namespace System.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.EntitySql;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Data.Query.InternalTrees;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    internal static class EntityUtil
    {
        private static readonly Type AccessViolationType = typeof(AccessViolationException);
        private const int AssemblyQualifiedNameIndex = 3;
        private static readonly Type CommandCompilationType = typeof(EntityCommandCompilationException);
        private static readonly Type CommandExecutionType = typeof(EntityCommandExecutionException);
        internal static Dictionary<string, string> COMPILER_VERSION;
        private const int InvariantNameIndex = 2;
        private static readonly Type NullReferenceType = typeof(NullReferenceException);
        private static readonly Type OutOfMemoryType = typeof(OutOfMemoryException);
        private static readonly Type QueryType = typeof(EntitySqlException);
        private static readonly Type SecurityType = typeof(SecurityException);
        private static readonly Type StackOverflowType = typeof(StackOverflowException);
        private static readonly Type ThreadAbortType = typeof(ThreadAbortException);

        static EntityUtil()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "CompilerVersion",
                    "V3.5"
                }
            };
            COMPILER_VERSION = dictionary;
        }

        internal static InvalidOperationException AcceptAllChangesFailure(Exception e) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_AcceptAllChangesFailure(e.Message));

        internal static InvalidOperationException AcceptChangesEntityKeyIsNotValid() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_AcceptChangesEntityKeyIsNotValid);

        internal static InvalidOperationException AddedEntityAlreadyExists(EntityKey key) => 
            InvalidOperation(System.Data.Entity.Strings.Materializer_AddedEntityAlreadyExists(key.ConcatKeyValue()));

        internal static InvalidOperationException AddNewOperationNotAllowedOnAbstractBindingList() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectView_AddNewOperationNotAllowedOnAbstractBindingList);

        internal static ArgumentException Argument(string error)
        {
            ArgumentException e = new ArgumentException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException Argument(string error, Exception inner)
        {
            ArgumentException e = new ArgumentException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException Argument(string error, string parameter)
        {
            ArgumentException e = new ArgumentException(error, parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException Argument(string error, string parameter, Exception inner)
        {
            ArgumentException e = new ArgumentException(error, parameter, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentNullException ArgumentNull(string parameter)
        {
            ArgumentNullException e = new ArgumentNullException(parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName)
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(parameterName);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(parameterName, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException ArrayTooSmall(string parameter) => 
            Argument(System.Data.Entity.Strings.ArrayTooSmall, parameter);

        internal static ArgumentException AssociationInvalidMembers() => 
            Argument(System.Data.Entity.Strings.AssociationInvalidMembers);

        internal static ArgumentException AssociationSetNotInCSpace(string name) => 
            Argument(System.Data.Entity.Strings.EntitySetNotInCSPace(name));

        internal static void AttachContext(IEntityWithRelationships entityWithRelationships, ObjectContext context, EntitySet entitySet, MergeOption option)
        {
            RelationshipManager relationshipManager = entityWithRelationships.RelationshipManager;
            ValidateRelationshipManager(entityWithRelationships, relationshipManager);
            relationshipManager.AttachContext(context, entitySet, option);
        }

        internal static void BoolExprAssert(bool condition, string message)
        {
            if (!condition)
            {
                throw InternalError(InternalErrorCode.BoolExprAssert, 0, message);
            }
        }

        internal static ArgumentException BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet(string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet(facetName, typeName));

        internal static InvalidOperationException CannotAccessKeyEntryValues() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CannotAccessKeyEntryValues);

        internal static InvalidOperationException CannotAddEntityWithKeyAttached() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_CannotAddEntityWithKeyAttached);

        internal static InvalidOperationException CannotAddMoreThanOneEntityToEntityReference() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference);

        internal static InvalidOperationException CannotAddMoreThanOneEntityToEntityReferenceTryOtherMergeOption() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_TryDifferentMergeOption(System.Data.Entity.Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference));

        internal static InvalidOperationException CannotAttachEntityWithoutKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_CannotAttachEntityWithoutKey);

        internal static InvalidOperationException CannotAttachEntityWithTemporaryKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_CannotAttachEntityWithTemporaryKey);

        internal static InvalidOperationException CannotCallDeleteOnKeyEntry() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CannotDeleteOnKeyEntry);

        internal static InvalidOperationException CannotChangeEntityKey() => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_CannotChangeKey);

        internal static InvalidOperationException CannotChangeReferentialConstraintProperty() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_CannotChangeReferentialConstraintProperty);

        internal static ProviderIncompatibleException CannotCloneStoreProvider() => 
            ProviderIncompatible(System.Data.Entity.Strings.EntityClient_CannotCloneStoreProvider);

        internal static InvalidOperationException CannotCreateObjectStateEntryDbDataRecord() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntryDbDataRecord_CantCreate);

        internal static InvalidOperationException CannotCreateObjectStateEntryDbUpdatableDataRecord() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntryDbUpdatableDataRecord_CantCreate);

        internal static InvalidOperationException CannotCreateObjectStateEntryOriginalDbUpdatableDataRecord() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntryOriginalDbUpdatableDataRecord_CantCreate);

        internal static InvalidOperationException CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(string roleName) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(roleName));

        internal static InvalidOperationException CannotCreateRelationshipEntitiesInDifferentContexts() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts);

        internal static InvalidOperationException CannotDeleteEntityNotInObjectStateManager() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_CannotDeleteEntityNotInObjectStateManager);

        internal static InvalidOperationException CannotDetachEntityNotInObjectStateManager() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_CannotDetachEntityNotInObjectStateManager);

        internal static InvalidOperationException CannotFillTryDifferentMergeOption(string relationshipName, string roleName) => 
            InvalidOperation(System.Data.Entity.Strings.Collections_CannotFillTryDifferentMergeOption(relationshipName, roleName));

        internal static InvalidOperationException CannotFixUpKeyToExistingValues() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_CannotFixUpKeyToExistingValues);

        internal static InvalidOperationException CannotModifyKeyEntryState() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CannotModifyKeyEntryState);

        internal static InvalidOperationException CannotModifyKeyProperty(string fieldName) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CannotModifyKeyProperty(fieldName));

        internal static InvalidOperationException CannotReEnumerateQueryResults() => 
            InvalidOperation(System.Data.Entity.Strings.Materializer_CannotReEnumerateQueryResults);

        internal static InvalidOperationException CannotRemergeCollections() => 
            InvalidOperation(System.Data.Entity.Strings.Collections_UnableToMergeCollections);

        internal static InvalidOperationException CannotReplacetheEntityorRow() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectView_CannotReplacetheEntityorRow);

        internal static InvalidOperationException CannotResolveTheEntitySetforGivenEntity(Type type) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectView_CannotResolveTheEntitySet(type.FullName));

        internal static InvalidOperationException CannotSetDefaultContainerName() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_CannotSetDefaultContainerName);

        internal static ArgumentException CannotSetSpecialKeys() => 
            Argument(System.Data.Entity.Strings.EntityReference_CannotSetSpecialKeys, "value");

        internal static InvalidOperationException CantModifyDetachedDeletedEntries()
        {
            throw InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CantModifyDetachedDeletedEntries);
        }

        internal static InvalidOperationException CantModifyRelationState() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CantModifyRelationState);

        internal static InvalidOperationException CantModifyRelationValues() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CantModifyRelationValues);

        internal static InvalidOperationException CantSetEntityKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CantSetEntityKey);

        internal static InvalidOperationException ChangedInDifferentStateFromChanging(EntityState currentState, EntityState previousState) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_ChangedInDifferentStateFromChanging(currentState, previousState));

        internal static ArgumentException ChangeOnUnmappedComplexProperty(string complexPropertyName) => 
            Argument(System.Data.Entity.Strings.ObjectStateEntry_ChangeOnUnmappedComplexProperty(complexPropertyName));

        internal static ArgumentException ChangeOnUnmappedProperty(string entityPropertyName) => 
            Argument(System.Data.Entity.Strings.ObjectStateEntry_ChangeOnUnmappedProperty(entityPropertyName));

        internal static IEnumerable<T> CheckArgumentContainsNull<T>(ref IEnumerable<T> enumerableArgument, string argumentName) where T: class
        {
            GetCheapestSafeEnumerableAsCollection<T>(ref enumerableArgument);
            foreach (T local in enumerableArgument)
            {
                if (local == null)
                {
                    throw Argument(System.Data.Entity.Strings.CheckArgumentContainsNullFailed(argumentName));
                }
            }
            return enumerableArgument;
        }

        internal static IEnumerable<T> CheckArgumentEmpty<T>(ref IEnumerable<T> enumerableArgument, Func<string, string> errorMessage, string argumentName)
        {
            int num;
            GetCheapestSafeCountOfEnumerable<T>(ref enumerableArgument, out num);
            if (num <= 0)
            {
                throw Argument(errorMessage(argumentName));
            }
            return enumerableArgument;
        }

        internal static void CheckArgumentMergeOption(MergeOption mergeOption)
        {
            switch (mergeOption)
            {
                case MergeOption.AppendOnly:
                case MergeOption.OverwriteChanges:
                case MergeOption.PreserveChanges:
                case MergeOption.NoTracking:
                    return;
            }
            throw InvalidMergeOption(mergeOption);
        }

        internal static T CheckArgumentNull<T>(T value, string parameterName) where T: class
        {
            if (value == null)
            {
                ThrowArgumentNullException(parameterName);
            }
            return value;
        }

        internal static T CheckArgumentOutOfRange<T>(T[] values, int index, string parameterName)
        {
            if (values.Length <= index)
            {
                ThrowArgumentOutOfRangeException(parameterName);
            }
            return values[index];
        }

        internal static void CheckArgumentRefreshMode(RefreshMode refreshMode)
        {
            switch (refreshMode)
            {
                case RefreshMode.StoreWins:
                case RefreshMode.ClientWins:
                    return;
            }
            throw InvalidRefreshMode(refreshMode);
        }

        internal static void CheckContextNull(ObjectContext context)
        {
            if (context == null)
            {
                throw UnexpectedNullContext();
            }
        }

        internal static void CheckEntityKeyNull(EntityKey entityKey)
        {
            if (entityKey == null)
            {
                throw UnexpectedNullEntityKey();
            }
        }

        internal static void CheckEntityKeysMatch(IEntityWithKey entity, EntityKey key)
        {
            if (entity.EntityKey != key)
            {
                throw EntityKeyDoesntMatchKeySetOnEntity(entity);
            }
        }

        internal static void CheckKeyForRelationship(object owner, MergeOption mergeOption)
        {
            if ((MergeOption.NoTracking == mergeOption) && !(owner is IEntityWithKey))
            {
                throw InvalidOperation(System.Data.Entity.Strings.RelationshipManager_CannotNavigateRelationshipForDetachedEntityWithoutKey(owner));
            }
        }

        internal static void CheckStringArgument(string value, string parameterName)
        {
            CheckArgumentNull<string>(value, parameterName);
            if (value.Length == 0)
            {
                throw InvalidStringArgument(parameterName);
            }
        }

        internal static InvalidOperationException CircularRelationshipsWithReferentialConstraints() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_CircularRelationshipsWithReferentialConstraints);

        internal static InvalidOperationException ClientEntityRemovedFromStore(string entitiesKeys) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_ClientEntityRemovedFromStore(entitiesKeys));

        internal static InvalidOperationException ClosedDataReaderError() => 
            InvalidOperation(System.Data.Entity.Strings.ADP_ClosedDataReaderError);

        internal static InvalidOperationException CollectionAlreadyInitialized() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_CollectionAlreadyInitialized(System.Data.Entity.Strings.RelationshipManager_CollectionInitializeIsForDeserialization));

        internal static ArgumentException CollectionParameterElementIsNull(string parameterName) => 
            Argument(System.Data.Entity.Strings.ADP_CollectionParameterElementIsNull(parameterName));

        internal static ArgumentException CollectionParameterElementIsNullOrEmpty(string parameterName) => 
            Argument(System.Data.Entity.Strings.ADP_CollectionParameterElementIsNullOrEmpty(parameterName));

        internal static InvalidOperationException CollectionRelationshipManagerAttached() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_CollectionRelationshipManagerAttached(System.Data.Entity.Strings.RelationshipManager_CollectionInitializeIsForDeserialization));

        internal static EntityCommandCompilationException CommandCompilation(string message, Exception innerException)
        {
            EntityCommandCompilationException e = new EntityCommandCompilationException(message, innerException);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntityCommandExecutionException CommandExecution(string message)
        {
            EntityCommandExecutionException e = new EntityCommandExecutionException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntityCommandExecutionException CommandExecution(string message, Exception innerException)
        {
            EntityCommandExecutionException e = new EntityCommandExecutionException(message, innerException);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntityCommandExecutionException CommandExecutionDataReaderFieldCountForPrimitiveType() => 
            CommandExecution(System.Data.Entity.Strings.ADP_InvalidDataReaderFieldCountForPrimitiveType);

        internal static EntityCommandExecutionException CommandExecutionDataReaderMissinDiscriminatorColumn(string columnName, EdmFunction functionImport) => 
            CommandExecution(System.Data.Entity.Strings.ADP_InvalidDataReaderMissingDiscriminatorColumn(columnName, functionImport.FullName));

        internal static EntityCommandExecutionException CommandExecutionDataReaderMissingColumnForType(EdmMember member) => 
            CommandExecution(System.Data.Entity.Strings.ADP_InvalidDataReaderMissingColumnForType(member.DeclaringType.FullName, member.Name));

        internal static ArgumentException ComplexChangeRequestedOnScalarProperty(string propertyName) => 
            Argument(System.Data.Entity.Strings.ComplexObject_ComplexChangeRequestedOnScalarProperty(propertyName));

        internal static InvalidOperationException ComplexObjectAlreadyAttachedToParent() => 
            InvalidOperation(System.Data.Entity.Strings.ComplexObject_ComplexObjectAlreadyAttachedToParent);

        internal static ArgumentException ComplexTypeInvalidMembers() => 
            Argument(System.Data.Entity.Strings.ComplexTypeInvalidMembers);

        internal static ConstraintException Constraint(string message)
        {
            ConstraintException e = new ConstraintException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException ContainerQualifiedEntitySetNameRequired(string argument) => 
            Argument(System.Data.Entity.Strings.ObjectContext_ContainerQualifiedEntitySetNameRequired, argument);

        internal static InvalidOperationException ContextMetadataHasChanged() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_MetadataHasChanged);

        private static string ConvertCardinalityToString(int? cardinality)
        {
            if (!cardinality.HasValue)
            {
                return "*";
            }
            return cardinality.Value.ToString(CultureInfo.CurrentUICulture);
        }

        internal static InvalidOperationException CurrentValuesDoesNotExist() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_CurrentValuesDoesNotExist);

        internal static DataException Data(string message)
        {
            DataException e = new DataException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException DataReaderClosed(string method) => 
            InvalidOperation(System.Data.Entity.Strings.ADP_DataReaderClosed(method));

        internal static InvalidOperationException DataRecordMustBeEntity() => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_DataRecordMustBeEntity);

        internal static ArgumentException DetachedObjectStateEntriesDoesNotExistInObjectStateManager() => 
            Argument(System.Data.Entity.Strings.ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager);

        internal static ArgumentException EmptyIdentity(string parameter) => 
            Argument(System.Data.Entity.Strings.EmptyIdentity, parameter);

        internal static ArgumentException EntitiesHaveDifferentType(string originalEntityTypeName, string changedEntityTypeName) => 
            Argument(System.Data.Entity.Strings.ObjectContext_EntitiesHaveDifferentType(originalEntityTypeName, changedEntityTypeName));

        internal static InvalidOperationException EntityAlreadyExistsInObjectStateManager() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_EntityAlreadyExistsInObjectStateManager);

        internal static InvalidOperationException EntityCantHaveMultipleChangeTrackers() => 
            InvalidOperation(System.Data.Entity.Strings.Entity_EntityCantHaveMultipleChangeTrackers);

        internal static InvalidOperationException EntityConflictsWithKeyEntry() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_EntityConflictsWithKeyEntry);

        internal static InvalidOperationException EntityContainterNotFoundForName(string entityContainerName) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_EntityContainerNotFoundForName(entityContainerName));

        internal static InvalidOperationException EntityIsNotPartOfRelationship() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_EntityIsNotPartOfRelationship);

        internal static InvalidOperationException EntityKeyDoesntMatchKeySetOnEntity(IEntityWithKey entity) => 
            new InvalidOperationException(System.Data.Entity.Strings.EntityKey_DoesntMatchKeyOnEntity(entity.GetType().FullName));

        internal static ArgumentException EntityKeyInvalidName(string invalidName) => 
            Argument(System.Data.Entity.Strings.EntityKey_InvalidName(invalidName));

        internal static ArgumentException EntityKeyMustHaveValues(string argument) => 
            Argument(System.Data.Entity.Strings.EntityKey_EntityKeyMustHaveValues, argument);

        internal static InvalidOperationException EntityKeyValueMismatch() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_EntityKeyValueMismatch);

        internal static InvalidOperationException EntityMustBeUnchangedOrModified(EntityState state) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_EntityMustBeUnchangedOrModified(state.ToString()));

        internal static ArgumentException EntitySetDoesNotMatch(string argument, string entitySetName) => 
            Argument(System.Data.Entity.Strings.EntityKey_EntitySetDoesNotMatch(entitySetName), argument);

        internal static InvalidOperationException EntitySetForNonEntityType() => 
            InvalidOperation(System.Data.Entity.Strings.ADP_EntitySetForNonEntityType);

        internal static ArgumentException EntitySetInAnotherContainer(string parameter) => 
            Argument(System.Data.Entity.Strings.EntitySetInAnotherContainer, parameter);

        internal static InvalidOperationException EntitySetIsNotValidForRelationship(string entitySetContainerName, string entitySetName, string roleName, string associationSetContainerName, string associationSetName) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_EntitySetIsNotValidForRelationship(entitySetContainerName, entitySetName, roleName, associationSetContainerName, associationSetName));

        internal static InvalidOperationException EntitySetNameOrEntityKeyRequired() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_EntitySetNameOrEntityKeyRequired);

        internal static InvalidOperationException EntitySetNotFoundForName(string entitySetName) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_EntitySetNotFoundForName(entitySetName));

        internal static ArgumentException EntitySetNotInCSpace(string name) => 
            Argument(System.Data.Entity.Strings.EntitySetNotInCSPace(name));

        internal static EntitySqlException EntitySqlError(string message)
        {
            EntitySqlException e = new EntitySqlException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntitySqlException EntitySqlError(ErrorContext errCtx, string message)
        {
            EntitySqlException e = EntitySqlException.Create(errCtx, message, null);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntitySqlException EntitySqlError(string message, Exception innerException)
        {
            EntitySqlException e = new EntitySqlException(message, innerException);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntitySqlException EntitySqlError(ErrorContext errCtx, string message, Exception innerException)
        {
            EntitySqlException e = EntitySqlException.Create(errCtx, message, null);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntitySqlException EntitySqlError(string queryText, string errorMessage, int errorPosition)
        {
            EntitySqlException e = EntitySqlException.Create(queryText, errorMessage, errorPosition, null, false, null);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntitySqlException EntitySqlError(string queryText, string errorMessage, int errorPosition, string additionalErrorInformation, bool loadContextInfoFromResource)
        {
            EntitySqlException e = EntitySqlException.Create(queryText, errorMessage, errorPosition, additionalErrorInformation, loadContextInfoFromResource, null);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException EntityTypeDoesNotMatchEntitySet(string entityType, string entitysetName, string argument) => 
            Argument(System.Data.Entity.Strings.ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType(entityType, entitysetName), argument);

        internal static InvalidOperationException EntityTypeDoesntMatchEntitySetFromKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_EntityTypeDoesntMatchEntitySetFromKey);

        internal static ArgumentException EntityTypeInvalidMembers() => 
            Argument(System.Data.Entity.Strings.EntityTypeInvalidMembers);

        internal static InvalidOperationException EntityTypesDoNotMatch(string recordType, string entitySetType) => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_EntityTypesDoNotMatch(recordType, entitySetType));

        internal static InvalidOperationException EntityValueChangedWithoutEntityValueChanging() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging);

        internal static InvalidOperationException ExecuteFunctionCalledWithNonReaderFunction(EdmFunction functionImport)
        {
            string str;
            if (functionImport.ReturnParameter == null)
            {
                str = System.Data.Entity.Strings.ObjectContext_ExecuteFunctionCalledWithNonQueryFunction(functionImport.Name);
            }
            else
            {
                str = System.Data.Entity.Strings.ObjectContext_ExecuteFunctionCalledWithScalarFunction(functionImport.ReturnParameter.TypeUsage.EdmType.FullName, functionImport.Name);
            }
            return InvalidOperation(str);
        }

        internal static InvalidOperationException ExecuteFunctionTypeMismatch(Type typeArgument, EdmType expectedElementType) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_ExecuteFunctionTypeMismatch(typeArgument.FullName, expectedElementType.FullName));

        internal static InvalidOperationException ExpectedCollectionGotReference(string typeName, string roleName, string relationshipName) => 
            InvalidOperation(System.Data.Entity.Strings.Collections_ExpectedCollectionGotReference(typeName, roleName, relationshipName));

        internal static InvalidOperationException ExpectedReferenceGotCollection(string typeName, string roleName, string relationshipName) => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_ExpectedReferenceGotCollection(typeName, roleName, relationshipName));

        internal static ArgumentException FacetValueHasIncorrectType(string facetName, Type expectedType, Type actualType, string parameter) => 
            Argument(System.Data.Entity.Strings.FacetValueHasIncorrectType(facetName, expectedType.FullName, actualType.FullName), parameter);

        internal static InvalidOperationException FoundMoreThanOneRelatedEnd() => 
            InvalidOperation(System.Data.Entity.Strings.Collections_FoundMoreThanOneRelatedEnd);

        internal static T GenericCheckArgumentNull<T>(T value, string parameterName) where T: class => 
            CheckArgumentNull<T>(value, parameterName);

        [SecurityCritical, SecurityTreatAsSafe, FileIOPermission(SecurityAction.Assert, AllLocalFiles=FileIOPermissionAccess.PathDiscovery)]
        internal static AssemblyName GetAssemblyName(Assembly assembly) => 
            assembly?.GetName();

        private static void GetCheapestSafeCountOfEnumerable<T>(ref IEnumerable<T> enumerable, out int count)
        {
            ICollection<T> cheapestSafeEnumerableAsCollection = GetCheapestSafeEnumerableAsCollection<T>(ref enumerable);
            count = cheapestSafeEnumerableAsCollection.Count;
        }

        private static ICollection<T> GetCheapestSafeEnumerableAsCollection<T>(ref IEnumerable<T> enumerable)
        {
            ICollection<T> is2 = enumerable as ICollection<T>;
            if (is2 != null)
            {
                return is2;
            }
            return new List<T>(enumerable);
        }

        internal static RelationshipManager GetRelationshipManager(object entity)
        {
            RelationshipManager manager = null;
            IEntityWithRelationships entityWithRelationships = entity as IEntityWithRelationships;
            if (entityWithRelationships != null)
            {
                ValidateRelationshipManager(entityWithRelationships, manager = entityWithRelationships.RelationshipManager);
            }
            return manager;
        }

        internal static InvalidOperationException ImplicitlyClosedDataReaderError() => 
            InvalidOperation(System.Data.Entity.Strings.ADP_ImplicitlyClosedDataReaderError);

        internal static ArgumentException IncompatibleArgument() => 
            Argument(System.Data.Entity.Strings.ObjectView_IncompatibleArgument);

        internal static InvalidOperationException InconsistentReferentialConstraintProperties() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_InconsistentReferentialConstraintProperties);

        internal static ArgumentException IncorrectNumberOfKeyValuePairs(string argument, string typeName, int expectedNumFields, int actualNumFields) => 
            Argument(System.Data.Entity.Strings.EntityKey_IncorrectNumberOfKeyValuePairs(typeName, expectedNumFields, actualNumFields), argument);

        internal static InvalidOperationException IncorrectNumberOfKeyValuePairsInvalidOperation(string typeName, int expectedNumFields, int actualNumFields) => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_IncorrectNumberOfKeyValuePairs(typeName, expectedNumFields, actualNumFields));

        internal static ArgumentException IncorrectValueType(string argument, string keyField, string expectedTypeName, string actualTypeName) => 
            Argument(System.Data.Entity.Strings.EntityKey_IncorrectValueType(keyField, expectedTypeName, actualTypeName), argument);

        internal static InvalidOperationException IncorrectValueTypeInvalidOperation(string keyField, string expectedTypeName, string actualTypeName) => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_IncorrectValueType(keyField, expectedTypeName, actualTypeName));

        internal static NotSupportedException IndexBasedInsertIsNotSupported() => 
            NotSupported(System.Data.Entity.Strings.ObjectView_IndexBasedInsertIsNotSupported);

        internal static IndexOutOfRangeException IndexOutOfRange(string error)
        {
            IndexOutOfRangeException e = new IndexOutOfRangeException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static Exception InternalError(InternalErrorCode internalError) => 
            InvalidOperation(System.Data.Entity.Strings.ADP_InternalProviderError((int) internalError));

        internal static Exception InternalError(InternalErrorCode internalError, int location) => 
            InternalError(internalError, location, null);

        internal static Exception InternalError(InternalErrorCode internalError, int location, object additionalInfo)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}, {1}", (int) internalError, location);
            if (additionalInfo != null)
            {
                builder.AppendFormat(", {0}", additionalInfo);
            }
            return InvalidOperation(System.Data.Entity.Strings.ADP_InternalProviderError(builder.ToString()));
        }

        internal static ArgumentException InvalidBaseTypeLoop(string parameter) => 
            Argument(System.Data.Entity.Strings.InvalidBaseTypeLoop, parameter);

        internal static IndexOutOfRangeException InvalidBufferSizeOrIndex(int numBytes, int bufferIndex) => 
            IndexOutOfRange(System.Data.Entity.Strings.ADP_InvalidBufferSizeOrIndex(numBytes.ToString(CultureInfo.InvariantCulture), bufferIndex.ToString(CultureInfo.InvariantCulture)));

        internal static System.Data.MetadataException InvalidCollectionForMapping(DataSpace space) => 
            Metadata(System.Data.Entity.Strings.InvalidCollectionForMapping(space.ToString()));

        internal static InvalidOperationException InvalidCollectionSpecified(DataSpace space) => 
            InvalidOperation(System.Data.Entity.Strings.InvalidCollectionSpecified(space));

        internal static ArgumentException InvalidCommandTimeout(string argument) => 
            Argument(System.Data.Entity.Strings.ObjectContext_InvalidCommandTimeout, argument);

        internal static ArgumentException InvalidComplexDataRecordObject(string propertyName) => 
            Argument(System.Data.Entity.Strings.ComplexObject_InvalidComplexDataRecordObject(propertyName));

        internal static ArgumentException InvalidConnection(bool isConnectionConstructor, Exception innerException)
        {
            if (isConnectionConstructor)
            {
                return InvalidConnection("connection", innerException);
            }
            return InvalidConnectionString("connectionString", innerException);
        }

        internal static ArgumentException InvalidConnection(string parameter, Exception inner) => 
            Argument(System.Data.Entity.Strings.ObjectContext_InvalidConnection, parameter, inner);

        internal static ArgumentException InvalidConnectionString(string parameter, Exception inner) => 
            Argument(System.Data.Entity.Strings.ObjectContext_InvalidConnectionString, parameter, inner);

        internal static InvalidOperationException InvalidContainedTypeCollection(string entityType, string relatedEndType) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidContainedType_Collection(entityType, relatedEndType));

        internal static InvalidOperationException InvalidContainedTypeReference(string entityType, string relatedEndType) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidContainedType_Reference(entityType, relatedEndType));

        internal static InvalidOperationException InvalidDataAdapter() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_InvalidDataAdapter);

        internal static IndexOutOfRangeException InvalidDataLength(long length) => 
            IndexOutOfRange(System.Data.Entity.Strings.ADP_InvalidDataLength(length.ToString(CultureInfo.InvariantCulture)));

        internal static ArgumentException InvalidDataType(TypeCode typecode) => 
            Argument(System.Data.Entity.Strings.ADP_InvalidDataType(typecode.ToString()));

        internal static ArgumentException InvalidDefaultContainerName(string parameter, string defaultContainerName) => 
            Argument(System.Data.Entity.Strings.ObjectContext_InvalidDefaultContainerName(defaultContainerName), parameter);

        internal static ArgumentOutOfRangeException InvalidDestinationBufferIndex(int maxLen, int dstOffset, string parameterName) => 
            ArgumentOutOfRange(System.Data.Entity.Strings.ADP_InvalidDestinationBufferIndex(maxLen.ToString(CultureInfo.InvariantCulture), dstOffset.ToString(CultureInfo.InvariantCulture)), parameterName);

        internal static InvalidOperationException InvalidEntityContextForAttach() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidEntityContextForAttach);

        internal static InvalidOperationException InvalidEntitySetInKey(string keyContainer, string keyEntitySet, string expectedContainer, string expectedEntitySet) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_InvalidEntitySetInKey(keyContainer, keyEntitySet, expectedContainer, expectedEntitySet));

        internal static InvalidOperationException InvalidEntitySetInKeyFromName(string keyContainer, string keyEntitySet, string expectedContainer, string expectedEntitySet, string argument) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_InvalidEntitySetInKeyFromName(keyContainer, keyEntitySet, expectedContainer, expectedEntitySet, argument));

        internal static ArgumentException InvalidEntitySetName(string name) => 
            Argument(System.Data.Entity.Strings.InvalidEntitySetName(name));

        internal static ArgumentException InvalidEntitySetOnEntity(string entitySetName, Type entityType, string parameter) => 
            Argument(System.Data.Entity.Strings.ObjectContext_InvalidEntitySetOnEntity(entitySetName, entityType), parameter);

        internal static InvalidOperationException InvalidEntityStateForAttach() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidEntityStateForAttach);

        internal static InvalidOperationException InvalidEntityStateLoad(string relatedEndType) => 
            InvalidOperation(System.Data.Entity.Strings.Collections_InvalidEntityStateLoad(relatedEndType));

        internal static InvalidOperationException InvalidEntityStateSource() => 
            InvalidOperation(System.Data.Entity.Strings.Collections_InvalidEntityStateSource);

        internal static ArgumentException InvalidEntityType(string argument, Type type)
        {
            if (typeof(IEntityWithChangeTracker).IsAssignableFrom(type))
            {
                return Argument(System.Data.Entity.Strings.ObjectContext_NoMappingForEntityType(type.FullName), argument);
            }
            return Argument(System.Data.Entity.Strings.ObjectContext_NonEntityType(type.FullName), argument);
        }

        internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value) => 
            ArgumentOutOfRange(System.Data.Entity.Strings.ADP_InvalidEnumerationValue(type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);

        internal static InvalidOperationException InvalidKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_InvalidKey);

        internal static ArgumentOutOfRangeException InvalidMergeOption(MergeOption value) => 
            InvalidEnumerationValue(typeof(MergeOption), (int) value);

        internal static ArgumentException InvalidModeInParameterInFunction(string parameterName) => 
            Argument(System.Data.Entity.Strings.InvalidModeInParameterInFunction, parameterName);

        internal static ArgumentException InvalidModeInReturnParameterInFunction(string parameterName) => 
            Argument(System.Data.Entity.Strings.InvalidModeInReturnParameterInFunction, parameterName);

        internal static ArgumentException InvalidModifiedPropertyName(string propertyName) => 
            Argument(System.Data.Entity.Strings.ObjectStateEntry_SetModifiedOnInvalidProperty(propertyName));

        internal static InvalidOperationException InvalidNthElementContextForAttach(int index) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidNthElementContextForAttach(index));

        internal static InvalidOperationException InvalidNthElementNullForAttach(int index) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidNthElementNullForAttach(index));

        internal static InvalidOperationException InvalidNthElementStateForAttach(int index) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidNthElementStateForAttach(index));

        internal static InvalidOperationException InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException InvalidOperation(string error, Exception inner)
        {
            InvalidOperationException e = new InvalidOperationException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException InvalidOwnerStateForAttach() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_InvalidOwnerStateForAttach);

        internal static ArgumentException InvalidQualifiedEntitySetName() => 
            Argument(System.Data.Entity.Strings.EntityKey_InvalidQualifiedEntitySetName, "qualifiedEntitySetName");

        internal static ArgumentOutOfRangeException InvalidRefreshMode(RefreshMode value) => 
            InvalidEnumerationValue(typeof(RefreshMode), (int) value);

        internal static InvalidOperationException InvalidRelationshipManagerOwner() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_InvalidRelationshipManagerOwner);

        internal static ArgumentException InvalidRelationshipSetName(string name) => 
            Argument(System.Data.Entity.Strings.InvalidRelationshipSetName(name));

        internal static System.Data.MetadataException InvalidSchemaEncountered(string errors) => 
            Metadata(string.Format(CultureInfo.CurrentCulture, EntityRes.GetString("InvalidSchemaEncountered"), new object[] { errors }));

        internal static ArgumentOutOfRangeException InvalidSourceBufferIndex(int maxLen, long srcOffset, string parameterName) => 
            ArgumentOutOfRange(System.Data.Entity.Strings.ADP_InvalidSourceBufferIndex(maxLen.ToString(CultureInfo.InvariantCulture), srcOffset.ToString(CultureInfo.InvariantCulture)), parameterName);

        internal static ArgumentException InvalidStringArgument(string parameterName) => 
            Argument(System.Data.Entity.Strings.InvalidStringArgument(parameterName), parameterName);

        internal static ArgumentException InvalidTargetRole(string relationshipName, string targetRoleName, string parameterName) => 
            Argument(System.Data.Entity.Strings.RelationshipManager_InvalidTargetRole(relationshipName, targetRoleName), parameterName);

        internal static bool IsCatchableEntityExceptionType(Exception e)
        {
            Type type = e.GetType();
            return (((IsCatchableExceptionType(e) && (type != CommandExecutionType)) && (type != CommandCompilationType)) && (type != QueryType));
        }

        internal static bool IsCatchableExceptionType(Exception e)
        {
            Type c = e.GetType();
            return (((((c != StackOverflowType) && (c != OutOfMemoryType)) && ((c != ThreadAbortType) && (c != NullReferenceType))) && (c != AccessViolationType)) && !SecurityType.IsAssignableFrom(c));
        }

        internal static bool IsNull(object value)
        {
            if ((value == null) || (DBNull.Value == value))
            {
                return true;
            }
            INullable nullable = value as INullable;
            return ((nullable != null) && nullable.IsNull);
        }

        internal static InvalidOperationException ItemCollectionAlreadyRegistered(DataSpace space) => 
            InvalidOperation(System.Data.Entity.Strings.ItemCollectionAlreadyRegistered(space.ToString()));

        internal static ArgumentException ItemDuplicateIdentity(string identity, string parameter, Exception inner) => 
            Argument(System.Data.Entity.Strings.ItemDuplicateIdentity(identity), parameter, inner);

        internal static ArgumentException ItemInvalidIdentity(string identity, string parameter) => 
            Argument(System.Data.Entity.Strings.ItemInvalidIdentity(identity), parameter);

        internal static InvalidOperationException KeyPropertyDoesntMatchValueInKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_KeyPropertyDoesntMatchValueInKey);

        internal static NotSupportedException KeysRequiredForJoinOverNest(Op op) => 
            NotSupported(System.Data.Entity.Strings.ADP_KeysRequiredForJoinOverNest(op.OpType.ToString()));

        internal static NotSupportedException KeysRequiredForNesting() => 
            NotSupported(System.Data.Entity.Strings.ADP_KeysRequiredForNesting);

        internal static ArgumentException KeywordNotSupported(string keyword) => 
            Argument(System.Data.Entity.Strings.EntityClient_KeywordNotSupported(keyword));

        internal static InvalidOperationException LessThanExpectedRelatedEntitiesFound() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_LessThanExpectedRelatedEntitiesFound);

        internal static InvalidOperationException LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd);

        internal static InvalidOperationException LoadCalledOnNonEmptyNoTrackedRelatedEnd() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd);

        internal static NotSupportedException MaterializerUnsupportedType() => 
            NotSupported(System.Data.Entity.Strings.Materializer_UnsupportedType);

        internal static ArgumentException MemberAlreadyBelongsToType(string parameter) => 
            Argument(System.Data.Entity.Strings.MemberAlreadyBelongsToType, parameter);

        internal static ArgumentException MemberInvalidIdentity(string identity, string parameter) => 
            Argument(System.Data.Entity.Strings.MemberInvalidIdentity(identity), parameter);

        internal static System.Data.MetadataException Metadata(string message)
        {
            System.Data.MetadataException e = new System.Data.MetadataException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static System.Data.MetadataException Metadata(string message, Exception inner)
        {
            System.Data.MetadataException e = new System.Data.MetadataException(message, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static System.Data.MetadataException MetadataGeneralError() => 
            Metadata(System.Data.Entity.Strings.Metadata_General_Error);

        internal static ArgumentException MinAndMaxMustBePositive(string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.MinAndMaxMustBePositive(facetName, typeName));

        internal static ArgumentException MinAndMaxValueMustBeDifferentForNonConstantFacet(string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.MinAndMaxValueMustBeDifferentForNonConstantFacet(facetName, typeName));

        internal static ArgumentException MinAndMaxValueMustBeSameForConstantFacet(string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.MinAndMaxValueMustBeSameForConstantFacet(facetName, typeName));

        internal static ArgumentException MinMustBeLessThanMax(string minimumValue, string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.MinMustBeLessThanMax(minimumValue, facetName, typeName));

        internal static InvalidOperationException MismatchedMergeOptionOnLoad(MergeOption mergeOption) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_MismatchedMergeOptionOnLoad(mergeOption));

        internal static ArgumentException MissingDefaultValueForConstantFacet(string facetName, string typeName) => 
            Argument(System.Data.Entity.Strings.MissingDefaultValueForConstantFacet(facetName, typeName));

        internal static ArgumentException MissingKeyValue(string argument, string keyField, string typeName) => 
            MissingKeyValue(argument, keyField, typeName, null);

        internal static ArgumentException MissingKeyValue(string argument, string keyField, string typeName, Exception inner) => 
            Argument(System.Data.Entity.Strings.EntityKey_MissingKeyValue(keyField, typeName), argument);

        internal static InvalidOperationException MissingKeyValueInvalidOperation(string keyField, string typeName) => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_MissingKeyValue(keyField, typeName));

        internal static MissingMethodException MissingMethod(string methodName) => 
            new MissingMethodException(System.Data.Entity.Strings.CodeGen_MissingMethod(methodName));

        internal static InvalidOperationException MissingQualifiedEntitySetName() => 
            InvalidOperation(System.Data.Entity.Strings.EntityKey_MissingEntitySetName);

        internal static InvalidOperationException MoreThanExpectedRelatedEntitiesFound() => 
            InvalidOperation(System.Data.Entity.Strings.EntityReference_MoreThanExpectedRelatedEntitiesFound);

        internal static InvalidOperationException MoreThanOneItemMatchesIdentity(string identity) => 
            InvalidOperation(System.Data.Entity.Strings.MoreThanOneItemMatchesIdentity(identity));

        internal static InvalidOperationException MustUseSequentialAccess() => 
            InvalidOperation(System.Data.Entity.Strings.ADP_MustUseSequentialAccess);

        internal static NotSupportedException NestingNotSupported(Op parentOp, Op childOp) => 
            NotSupported(System.Data.Entity.Strings.ADP_NestingNotSupported(parentOp.OpType.ToString(), childOp.OpType.ToString()));

        internal static InvalidOperationException NoCollectionForSpace(DataSpace space) => 
            InvalidOperation(System.Data.Entity.Strings.NoCollectionForSpace(space.ToString()));

        internal static InvalidOperationException NoData() => 
            InvalidOperation(System.Data.Entity.Strings.ADP_NoData);

        internal static InvalidOperationException NoEntryExistForEntityKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_NoEntryExistForEntityKey);

        internal static InvalidOperationException NoEntryExistsForObject(object entity) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_NoEntryExistsForObject(entity.GetType().FullName));

        internal static InvalidOperationException NonSequentialChunkAccess(long badIndex, long currIndex, string method) => 
            InvalidOperation(System.Data.Entity.Strings.ADP_NonSequentialChunkAccess(badIndex.ToString(CultureInfo.InvariantCulture), currIndex.ToString(CultureInfo.InvariantCulture), method));

        internal static InvalidOperationException NonSequentialColumnAccess(int badCol, int currCol) => 
            InvalidOperation(System.Data.Entity.Strings.ADP_NonSequentialColumnAccess(badCol.ToString(CultureInfo.InvariantCulture), currCol.ToString(CultureInfo.InvariantCulture)));

        internal static ArgumentException NoNullsAllowedInKeyValuePairs(string argument) => 
            Argument(System.Data.Entity.Strings.EntityKey_NoNullsAllowedInKeyValuePairs, argument);

        internal static InvalidOperationException NoRelationshipSetMatched(string relationshipName) => 
            InvalidOperation(System.Data.Entity.Strings.Collections_NoRelationshipSetMatched(relationshipName));

        internal static ArgumentException NotBinaryTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotBinaryTypeForTypeUsage);

        internal static ArgumentException NotDateTimeOffsetTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotDateTimeOffsetTypeForTypeUsage);

        internal static ArgumentException NotDateTimeTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotDateTimeTypeForTypeUsage);

        internal static ArgumentException NotDecimalTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotDecimalTypeForTypeUsage);

        internal static InvalidOperationException NotIEntityWithChangeTracker(object entity) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_DoesNotImplementIEntityWithChangeTracker(entity));

        internal static ArgumentException NotStringTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotStringTypeForTypeUsage);

        internal static NotSupportedException NotSupported()
        {
            NotSupportedException e = new NotSupportedException();
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static NotSupportedException NotSupported(string error)
        {
            NotSupportedException e = new NotSupportedException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException NotTimeTypeForTypeUsage() => 
            Argument(System.Data.Entity.Strings.NotTimeTypeForTypeUsage);

        internal static InvalidOperationException NthElementInAddedState(int i) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_NthElementInAddedState(i));

        internal static InvalidOperationException NthElementIsDuplicate(int i) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_NthElementIsDuplicate(i));

        internal static InvalidOperationException NthElementIsNull(int i) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_NthElementIsNull(i));

        internal static InvalidOperationException NthElementNotInObjectStateManager(int i) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_NthElementNotInObjectStateManager(i));

        internal static InvalidOperationException NullableComplexTypesNotSupported(string propertyName) => 
            InvalidOperation(System.Data.Entity.Strings.ComplexObject_NullableComplexTypesNotSupported(propertyName));

        internal static ObjectDisposedException ObjectContextDisposed() => 
            ObjectDisposed(System.Data.Entity.Strings.ObjectContext_ObjectDisposed);

        internal static ObjectDisposedException ObjectDisposed(string error)
        {
            ObjectDisposedException e = new ObjectDisposedException(null, error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException ObjectDoesNotHaveAKey(object entity) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_GetEntityKeyRequiresObjectToHaveAKey(entity.GetType().FullName));

        internal static ObjectNotFoundException ObjectNotFound() => 
            ObjectNotFound(System.Data.Entity.Strings.ObjectContext_ObjectNotFound);

        internal static ObjectNotFoundException ObjectNotFound(string error)
        {
            ObjectNotFoundException e = new ObjectNotFoundException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException ObjectStateEntryinInvalidState() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_InvalidState);

        internal static InvalidOperationException ObjectStateManagerContainsThisEntityKey() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_ObjectStateManagerContainsThisEntityKey);

        internal static InvalidOperationException ObjectStateManagerDoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(EntityState state) => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(state));

        internal static NotSupportedException OperationActionNotSupported() => 
            NotSupported(System.Data.Entity.Strings.OperationActionNotSupported);

        internal static InvalidOperationException OperationOnReadOnlyCollection() => 
            InvalidOperation(System.Data.Entity.Strings.OperationOnReadOnlyCollection);

        internal static InvalidOperationException OperationOnReadOnlyItem() => 
            InvalidOperation(System.Data.Entity.Strings.OperationOnReadOnlyItem);

        internal static InvalidOperationException OriginalValuesDoesNotExist() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_OriginalValuesDoesNotExist);

        internal static InvalidOperationException OwnerIsNotSourceType(string ownerType, string sourceRoleType, string sourceRoleName, string relationshipName) => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_OwnerIsNotSourceType(ownerType, sourceRoleType, sourceRoleName, relationshipName));

        internal static InvalidOperationException OwnerIsNull() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_OwnerIsNull);

        private static IEnumerable<ObjectStateEntry> ProcessStateEntries(IEnumerable<IEntityStateEntry> stateEntries) => 
            stateEntries.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>();

        internal static EntityException Provider(Exception inner)
        {
            EntityException e = new EntityException(System.Data.Entity.Strings.EntityClient_ProviderGeneralError, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntityException Provider(string error)
        {
            EntityException e = new EntityException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static EntityException Provider(string parameter, Exception inner)
        {
            EntityException e = new EntityException(System.Data.Entity.Strings.EntityClient_ProviderSpecificError(parameter), inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static NotSupportedException ProviderDoesNotSupportCommandTrees() => 
            NotSupported(System.Data.Entity.Strings.ADP_ProviderDoesNotSupportCommandTrees);

        internal static EntityException ProviderExceptionWithMessage(string message, Exception inner)
        {
            EntityException e = new EntityException(message, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ProviderIncompatibleException ProviderIncompatible(string error)
        {
            ProviderIncompatibleException e = new ProviderIncompatibleException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ProviderIncompatibleException ProviderIncompatible(string error, Exception innerException)
        {
            ProviderIncompatibleException e = new ProviderIncompatibleException(error, innerException);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException QualfiedEntitySetName(string parameterName) => 
            Argument(System.Data.Entity.Strings.ObjectContext_QualfiedEntitySetName, parameterName);

        internal static NotSupportedException RecyclingEntity(EntityKey key, Type newEntityType, Type existingEntityType) => 
            NotSupported(System.Data.Entity.Strings.Materializer_RecyclingEntity(TypeHelpers.GetFullName(key.EntityContainerName, key.EntitySetName), newEntityType.FullName, existingEntityType.FullName, key.ConcatKeyValue()));

        internal static InvalidOperationException ReferenceAlreadyInitialized() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_ReferenceAlreadyInitialized(System.Data.Entity.Strings.RelationshipManager_InitializeIsForDeserialization));

        internal static InvalidOperationException RelatedEndNotAttachedToContext(string relatedEndType) => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_RelatedEndNotAttachedToContext(relatedEndType));

        internal static InvalidOperationException RelatedEndNotFound() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_RelatedEndNotFound);

        internal static InvalidOperationException RelationshipManagerAttached() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_RelationshipManagerAttached(System.Data.Entity.Strings.RelationshipManager_InitializeIsForDeserialization));

        internal static InvalidOperationException RequiredMetadataNotAvailable() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectContext_RequiredMetadataNotAvailble);

        internal static ArgumentException RowTypeInvalidMembers() => 
            Argument(System.Data.Entity.Strings.RowTypeInvalidMembers);

        internal static void SetChangeTrackerOntoEntity(object entity, IEntityChangeTracker tracker)
        {
            IEntityWithChangeTracker tracker2 = entity as IEntityWithChangeTracker;
            if (tracker2 == null)
            {
                throw NotIEntityWithChangeTracker(entity);
            }
            tracker2.SetChangeTracker(tracker);
        }

        internal static void SetKeyOntoEntity(object entity, EntityKey entityKey)
        {
            IEntityWithKey key = entity as IEntityWithKey;
            if (key != null)
            {
                key.EntityKey = entityKey;
                CheckEntityKeysMatch(key, entityKey);
            }
        }

        internal static InvalidOperationException SetModifiedStates()
        {
            throw InvalidOperation(System.Data.Entity.Strings.ObjectStateEntry_SetModifiedStates);
        }

        internal static ArgumentException SpanPathSyntaxError() => 
            Argument(System.Data.Entity.Strings.ObjectQuery_Span_SpanPathSyntaxError);

        internal static bool? ThreeValuedAnd(bool? left, bool? right)
        {
            if (left.HasValue && right.HasValue)
            {
                return new bool?(!left.Value ? false : right.Value);
            }
            if (!left.HasValue && !right.HasValue)
            {
                return null;
            }
            if (left.HasValue)
            {
                return (left.Value ? null : ((bool?) false));
            }
            return (right.Value ? null : ((bool?) false));
        }

        internal static bool? ThreeValuedNot(bool? operand)
        {
            if (!operand.HasValue)
            {
                return null;
            }
            return new bool?(!operand.Value);
        }

        internal static bool? ThreeValuedOr(bool? left, bool? right)
        {
            if (left.HasValue && right.HasValue)
            {
                return new bool?(left.Value ? true : right.Value);
            }
            if (!left.HasValue && !right.HasValue)
            {
                return null;
            }
            if (left.HasValue)
            {
                return (left.Value ? ((bool?) true) : null);
            }
            return (right.Value ? ((bool?) true) : null);
        }

        internal static void ThrowArgumentNullException(string parameterName)
        {
            throw ArgumentNull(parameterName);
        }

        internal static void ThrowArgumentOutOfRangeException(string parameterName)
        {
            throw ArgumentOutOfRange(parameterName);
        }

        internal static void ThrowPropertyIsNotNullable()
        {
            throw Constraint(System.Data.Entity.Strings.Materializer_PropertyIsNotNullable);
        }

        internal static void ThrowPropertyIsNotNullable(string propertyName)
        {
            throw Constraint(System.Data.Entity.Strings.Materializer_PropertyIsNotNullableWithName(propertyName));
        }

        internal static void ThrowSetInvalidValue(object value, Type destinationType, string className, string propertyName)
        {
            if (value == null)
            {
                throw Constraint(System.Data.Entity.Strings.Materializer_SetInvalidValue((Nullable.GetUnderlyingType(destinationType) ?? destinationType).Name, className, propertyName, "null"));
            }
            throw InvalidOperation(System.Data.Entity.Strings.Materializer_SetInvalidValue((Nullable.GetUnderlyingType(destinationType) ?? destinationType).Name, className, propertyName, value.GetType().Name));
        }

        private static void TraceException(string trace, Exception e)
        {
            if (e != null)
            {
                EntityBid.Trace(trace, e.ToString());
            }
        }

        internal static void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.EntityUtil.TraceException|ERR|THROW> '%ls'\n", e);
        }

        internal static void TraceExceptionForCapture(Exception e)
        {
            TraceException("<comm.EntityUtil.TraceException|ERR|CATCH> '%ls'\n", e);
        }

        internal static bool TryGetProviderInvariantName(DbProviderFactory providerFactory, out string invariantName)
        {
            foreach (DataRow row in DbProviderFactories.GetFactoryClasses().Rows)
            {
                if (((string) row[3]).Equals(providerFactory.GetType().AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase))
                {
                    invariantName = (string) row[2];
                    return true;
                }
            }
            invariantName = null;
            return false;
        }

        internal static ArgumentException TypeNotInAssociationSet(string setName, string rootEntityTypeName, string typeName) => 
            Argument(System.Data.Entity.Strings.TypeNotInAssociationSet(typeName, rootEntityTypeName, setName));

        internal static ArgumentException TypeNotInEntitySet(string entitySetName, string rootEntityTypeName, string entityTypeName) => 
            Argument(System.Data.Entity.Strings.TypeNotInEntitySet(entityTypeName, rootEntityTypeName, entitySetName));

        internal static ArgumentException TypeUsageHasNoEdmType(string parameter) => 
            Argument(System.Data.Entity.Strings.TypeUsageHasNoEdmType, parameter);

        internal static InvalidOperationException UnableToAddToDisconnectedRelatedEnd() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_UnableToAddEntity);

        internal static ArgumentException UnableToFindRelationshipTypeInMetadata(string relationshipName, string parameterName) => 
            Argument(System.Data.Entity.Strings.RelationshipManager_UnableToFindRelationshipTypeInMetadata(relationshipName), parameterName);

        internal static InvalidOperationException UnableToRemoveFromDisconnectedRelatedEnd() => 
            InvalidOperation(System.Data.Entity.Strings.RelatedEnd_UnableToRemoveEntity);

        internal static InvalidOperationException UnableToRetrieveReferentialConstraintProperties() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_UnableToRetrieveReferentialConstraintProperties);

        internal static InvalidOperationException UnexpectedMetadataType(EdmType metadata) => 
            InvalidOperation(System.Data.Entity.Strings.Materializer_UnexpectedMetdataType(metadata.GetType()));

        internal static InvalidOperationException UnexpectedNullContext() => 
            InvalidOperation(System.Data.Entity.Strings.RelationshipManager_UnexpectedNullContext);

        internal static InvalidOperationException UnexpectedNullEntityKey() => 
            new InvalidOperationException(System.Data.Entity.Strings.EntityKey_UnexpectedNull);

        internal static InvalidOperationException UnexpectedNullRelationshipManager() => 
            new InvalidOperationException(System.Data.Entity.Strings.RelationshipManager_UnexpectedNull);

        internal static ArgumentException UnknownDataTypeCode(Type dataType, TypeCode typeCode)
        {
            int num = (int) typeCode;
            return Argument(System.Data.Entity.Strings.ADP_UnknownDataTypeCode(num.ToString(CultureInfo.InvariantCulture), dataType.FullName));
        }

        internal static UpdateException Update(string message, Exception innerException, IEnumerable<IEntityStateEntry> stateEntries)
        {
            UpdateException e = new UpdateException(message, innerException, ProcessStateEntries(stateEntries));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static UpdateException Update(string message, Exception innerException, params IEntityStateEntry[] stateEntries) => 
            Update(message, innerException, (IEnumerable<IEntityStateEntry>) stateEntries);

        internal static OptimisticConcurrencyException UpdateConcurrency(int rowsAffected, Exception innerException, IEnumerable<IEntityStateEntry> stateEntries)
        {
            OptimisticConcurrencyException e = new OptimisticConcurrencyException(System.Data.Entity.Strings.Update_ConcurrencyError(rowsAffected), innerException, ProcessStateEntries(stateEntries));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static UpdateException UpdateEntityMissingConstraintViolation(string relationshipSetName, string endName, IEntityStateEntry stateEntry) => 
            Update(System.Data.Entity.Strings.Update_MissingRequiredEntity(relationshipSetName, endName), null, new IEntityStateEntry[] { stateEntry });

        internal static UpdateException UpdateMissingEntity(string relationshipSetName, string entitySetName) => 
            Update(System.Data.Entity.Strings.Update_MissingEntity(relationshipSetName, entitySetName), null, new IEntityStateEntry[0]);

        internal static UpdateException UpdateRelationshipCardinalityConstraintViolation(string relationshipSetName, int minimumCount, int? maximumCount, string entitySetName, int actualCount, string otherEndPluralName, IEntityStateEntry stateEntry)
        {
            string str = ConvertCardinalityToString(new int?(minimumCount));
            string str2 = ConvertCardinalityToString(maximumCount);
            string str3 = ConvertCardinalityToString(new int?(actualCount));
            if ((minimumCount == 1) && (str == str2))
            {
                return Update(System.Data.Entity.Strings.Update_RelationshipCardinalityConstraintViolationSingleValue(entitySetName, relationshipSetName, str3, otherEndPluralName, str), null, new IEntityStateEntry[] { stateEntry });
            }
            return Update(System.Data.Entity.Strings.Update_RelationshipCardinalityConstraintViolation(entitySetName, relationshipSetName, str3, otherEndPluralName, str, str2), null, new IEntityStateEntry[] { stateEntry });
        }

        internal static void ValidateEntitySetInKey(EntityKey key, EntitySet entitySet)
        {
            ValidateEntitySetInKey(key, entitySet, null);
        }

        internal static void ValidateEntitySetInKey(EntityKey key, EntitySet entitySet, string argument)
        {
            string entityContainerName = key.EntityContainerName;
            string entitySetName = key.EntitySetName;
            string name = entitySet.EntityContainer.Name;
            string y = entitySet.Name;
            if (!StringComparer.Ordinal.Equals(entityContainerName, name) || !StringComparer.Ordinal.Equals(entitySetName, y))
            {
                if (string.IsNullOrEmpty(argument))
                {
                    throw InvalidEntitySetInKey(entityContainerName, entitySetName, name, y);
                }
                throw InvalidEntitySetInKeyFromName(entityContainerName, entitySetName, name, y, argument);
            }
        }

        internal static void ValidateRelationshipManager(IEntityWithRelationships entityWithRelationships)
        {
            ValidateRelationshipManager(entityWithRelationships, entityWithRelationships.RelationshipManager);
        }

        internal static void ValidateRelationshipManager(IEntityWithRelationships entityWithRelationships, RelationshipManager relationshipManager)
        {
            if (relationshipManager == null)
            {
                throw UnexpectedNullRelationshipManager();
            }
            if (!relationshipManager.IsOwner(entityWithRelationships))
            {
                throw InvalidRelationshipManagerOwner();
            }
        }

        internal static InvalidOperationException ValueInvalidCast(Type valueType, Type destinationType)
        {
            if ((destinationType.IsValueType && destinationType.IsGenericType) && (typeof(Nullable<>) == destinationType.GetGenericTypeDefinition()))
            {
                return InvalidOperation(System.Data.Entity.Strings.Materializer_InvalidCastNullable(valueType, destinationType.GetGenericArguments()[0]));
            }
            return InvalidOperation(System.Data.Entity.Strings.Materializer_InvalidCastReference(valueType, destinationType));
        }

        internal static InvalidOperationException ValueNullReferenceCast(Type destinationType) => 
            InvalidOperation(System.Data.Entity.Strings.Materializer_NullReferenceCast(destinationType.Name));

        internal static InvalidOperationException WriteOperationNotAllowedOnReadOnlyBindingList() => 
            InvalidOperation(System.Data.Entity.Strings.ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList);

        internal static IEnumerable<KeyValuePair<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            if ((first != null) && (second != null))
            {
                using (IEnumerator<T1> iteratorVariable0 = first.GetEnumerator())
                {
                    using (IEnumerator<T2> iteratorVariable1 = second.GetEnumerator())
                    {
                        while (iteratorVariable0.MoveNext() && iteratorVariable1.MoveNext())
                        {
                            yield return new KeyValuePair<T1, T2>(iteratorVariable0.Current, iteratorVariable1.Current);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Zip>d__0<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>, IEnumerable, IEnumerator<KeyValuePair<T1, T2>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private KeyValuePair<T1, T2> <>2__current;
            public IEnumerable<T1> <>3__first;
            public IEnumerable<T2> <>3__second;
            private int <>l__initialThreadId;
            public IEnumerator<T1> <firstEnumerator>5__1;
            public IEnumerator<T2> <secondEnumerator>5__2;
            public IEnumerable<T1> first;
            public IEnumerable<T2> second;

            [DebuggerHidden]
            public <Zip>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally3()
            {
                this.<>1__state = -1;
                if (this.<firstEnumerator>5__1 != null)
                {
                    this.<firstEnumerator>5__1.Dispose();
                }
            }

            private void <>m__Finally4()
            {
                this.<>1__state = 1;
                if (this.<secondEnumerator>5__2 != null)
                {
                    this.<secondEnumerator>5__2.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            if ((this.first != null) && (this.second != null))
                            {
                                this.<firstEnumerator>5__1 = this.first.GetEnumerator();
                                this.<>1__state = 1;
                                this.<secondEnumerator>5__2 = this.second.GetEnumerator();
                                this.<>1__state = 2;
                                while (this.<firstEnumerator>5__1.MoveNext() && this.<secondEnumerator>5__2.MoveNext())
                                {
                                    this.<>2__current = new KeyValuePair<T1, T2>(this.<firstEnumerator>5__1.Current, this.<secondEnumerator>5__2.Current);
                                    this.<>1__state = 3;
                                    return true;
                                Label_0092:
                                    this.<>1__state = 2;
                                }
                                this.<>m__Finally4();
                                this.<>m__Finally3();
                            }
                            break;

                        case 3:
                            goto Label_0092;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<T1, T2>> IEnumerable<KeyValuePair<T1, T2>>.GetEnumerator()
            {
                EntityUtil.<Zip>d__0<T1, T2> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (EntityUtil.<Zip>d__0<T1, T2>) this;
                }
                else
                {
                    d__ = new EntityUtil.<Zip>d__0<T1, T2>(0);
                }
                d__.first = this.<>3__first;
                d__.second = this.<>3__second;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<T1,T2>>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                    case 3:
                        try
                        {
                            switch (this.<>1__state)
                            {
                                case 2:
                                case 3:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        this.<>m__Finally4();
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            this.<>m__Finally3();
                        }
                        break;

                    default:
                        return;
                }
            }

            KeyValuePair<T1, T2> IEnumerator<KeyValuePair<T1, T2>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        internal enum InternalErrorCode
        {
            AssertionFailed = 0x3ec,
            BoolExprAssert = 0x403,
            CodeGen_NoSuchProperty = 0x3f3,
            CollectionWithNoColumns = 0x400,
            ColumnCountMismatch = 0x3eb,
            CommandTreeOnStoredProcedureEntityCommand = 0x402,
            EntityKeyMissingKeyValue = 0x3fa,
            ExtentWithoutEntity = 0x3ef,
            InvalidInternalTree = 0x3f5,
            InvalidParserState1 = 0x3f7,
            InvalidParserState2 = 0x3f8,
            InvalidPrimitiveTypeKind = 0x3fd,
            InvalidStateEntry = 0x3fc,
            JoinOverSingleStreamNest = 0x3f4,
            NameValuePairNext = 0x3f6,
            NestOverNest = 0x3ea,
            SqlGenParametersNotPermitted = 0x3f9,
            UnexpectedLinqLambdaExpressionFormat = 0x401,
            UnknownColumnMapKind = 0x3e9,
            UnknownLinqNodeType = 0x3ff,
            UnknownVar = 0x3ed,
            UnnestMultipleCollections = 0x3f1,
            UnnestWithoutInput = 0x3f0,
            UpdatePipelineResultRequestInvalid = 0x3fb,
            WrongNumberOfKeys = 0x3e8,
            WrongVarType = 0x3ee
        }
    }
}

