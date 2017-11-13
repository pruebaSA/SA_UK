namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Reflection;

    internal abstract class Shaper
    {
        public readonly ObjectContext Context;
        public readonly System.Data.Objects.MergeOption MergeOption;
        public readonly DbDataReader Reader;
        public readonly object[] State;
        public readonly MetadataWorkspace Workspace;

        internal Shaper(DbDataReader reader, ObjectContext context, MetadataWorkspace workspace, System.Data.Objects.MergeOption mergeOption, int stateCount)
        {
            this.Reader = reader;
            this.MergeOption = mergeOption;
            this.State = new object[stateCount];
            this.Context = context;
            this.Workspace = workspace;
        }

        private void CheckClearedEntryOnSpan(EntityKey sourceKey, IEntityWithRelationships sourceEntity, AssociationEndMember targetMember)
        {
            EntitySet set;
            AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
            AssociationSet associationSet = MetadataHelper.GetAssociationsForEntitySetAndAssociationType(this.Context.MetadataWorkspace.GetEntityContainer(sourceKey.EntityContainerName, DataSpace.CSpace), sourceKey.EntitySetName, (AssociationType) otherAssociationEnd.DeclaringType, otherAssociationEnd.Name, out set);
            if (associationSet != null)
            {
                ObjectStateManager.RemoveRelationships(this.Context, this.MergeOption, associationSet, sourceKey, otherAssociationEnd);
            }
        }

        private void CheckClearedEntryOnSpan(object targetValue, IEntityWithRelationships sourceEntity, EntityKey sourceKey, AssociationEndMember targetMember)
        {
            if (((sourceKey != null) && (targetValue == null)) && ((this.MergeOption == System.Data.Objects.MergeOption.PreserveChanges) || (this.MergeOption == System.Data.Objects.MergeOption.OverwriteChanges)))
            {
                TypeUsage usage;
                EdmType elementType = ((RefType) MetadataHelper.GetOtherAssociationEnd(targetMember).TypeUsage.EdmType).ElementType;
                if ((!this.Context.Perspective.TryGetType(sourceEntity.GetType(), out usage) || usage.EdmType.EdmEquals(elementType)) || TypeSemantics.IsSubTypeOf(usage.EdmType, elementType))
                {
                    this.CheckClearedEntryOnSpan(sourceKey, sourceEntity, targetMember);
                }
            }
        }

        public TElement Discriminate<TElement>(object[] discriminatorValues, Func<object[], EntityType> discriminate, KeyValuePair<EntityType, Func<Shaper, TElement>>[] elementDelegates)
        {
            EntityType type = discriminate(discriminatorValues);
            Func<Shaper, TElement> func = null;
            foreach (KeyValuePair<EntityType, Func<Shaper, TElement>> pair in elementDelegates)
            {
                if (pair.Key == type)
                {
                    func = pair.Value;
                }
            }
            return func(this);
        }

        private void FullSpanAction<T_TargetEntity>(IEntityWithRelationships sourceEntity, IList<T_TargetEntity> spannedEntities, AssociationEndMember targetMember)
        {
            IRelatedEnd end;
            EntityUtil.CheckKeyForRelationship(sourceEntity, this.MergeOption);
            EntityKey sourceKey = ObjectStateManager.FindKeyOnEntityWithRelationships(sourceEntity);
            if ((sourceEntity != null) && sourceEntity.RelationshipManager.TryGetRelatedEnd(targetMember.DeclaringType.FullName, targetMember.Name, out end))
            {
                AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
                int num = ObjectStateManager.UpdateRelationships(this.Context, this.MergeOption, (AssociationSet) end.RelationshipSet, otherAssociationEnd, sourceKey, sourceEntity, targetMember, (List<T_TargetEntity>) spannedEntities, true);
                this.SetIsLoadedForSpan((RelatedEnd) end, num > 0);
            }
        }

        public TColumn GetColumnValueWithErrorHandling<TColumn>(int ordinal) => 
            new ColumnErrorHandlingValueReader<TColumn>().GetValue(this.Reader, ordinal);

        public TProperty GetPropertyValueWithErrorHandling<TProperty>(int ordinal, string propertyName, string typeName) => 
            new PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName).GetValue(this.Reader, ordinal);

        public TEntity HandleEntity<TEntity>(TEntity entity, EntityKey entityKey, EntitySet entitySet)
        {
            TEntity local = entity;
            if (entityKey != null)
            {
                ObjectStateEntry existingEntry = this.Context.ObjectStateManager.FindObjectStateEntry(entityKey);
                if ((existingEntry != null) && !existingEntry.IsKeyEntry)
                {
                    this.UpdateEntry<TEntity>(entity, existingEntry);
                    return (TEntity) existingEntry.Entity;
                }
                if (existingEntry == null)
                {
                    this.Context.ObjectStateManager.AddEntry(entity, entityKey, entitySet, "HandleEntity", false);
                    return local;
                }
                this.Context.ObjectStateManager.PromoteKeyEntry(existingEntry, entity, null, false, true, false, "HandleEntity");
            }
            return local;
        }

        public TEntity HandleEntityAppendOnly<TEntity>(Func<Shaper, TEntity> constructEntityDelegate, EntityKey entityKey, EntitySet entitySet)
        {
            if (entityKey == null)
            {
                return constructEntityDelegate(this);
            }
            ObjectStateEntry keyEntry = this.Context.ObjectStateManager.FindObjectStateEntry(entityKey);
            if ((keyEntry != null) && !keyEntry.IsKeyEntry)
            {
                if (typeof(TEntity) != keyEntry.Entity.GetType())
                {
                    throw EntityUtil.RecyclingEntity(keyEntry.EntityKey, typeof(TEntity), keyEntry.Entity.GetType());
                }
                if (EntityState.Added == keyEntry.State)
                {
                    throw EntityUtil.AddedEntityAlreadyExists(keyEntry.EntityKey);
                }
                return (TEntity) keyEntry.Entity;
            }
            TEntity dataObject = constructEntityDelegate(this);
            if (keyEntry == null)
            {
                this.Context.ObjectStateManager.AddEntry(dataObject, entityKey, entitySet, "HandleEntity", false);
                return dataObject;
            }
            this.Context.ObjectStateManager.PromoteKeyEntry(keyEntry, dataObject, null, false, true, false, "HandleEntity");
            return dataObject;
        }

        public T_SourceEntity HandleFullSpanCollection<T_SourceEntity, T_TargetEntity>(T_SourceEntity entity, Coordinator<T_TargetEntity> coordinator, AssociationEndMember targetMember)
        {
            Action<Shaper, List<T_TargetEntity>> closeHandler = null;
            IEntityWithRelationships sourceEntity = entity as IEntityWithRelationships;
            if (sourceEntity != null)
            {
                if (closeHandler == null)
                {
                    closeHandler = delegate (Shaper state, List<T_TargetEntity> spannedEntities) {
                        this.FullSpanAction<T_TargetEntity>(sourceEntity, spannedEntities, targetMember);
                    };
                }
                coordinator.RegisterCloseHandler(closeHandler);
            }
            return entity;
        }

        public T_SourceEntity HandleFullSpanElement<T_SourceEntity, T_TargetEntity>(T_SourceEntity entity, T_TargetEntity spannedEntity, AssociationEndMember targetMember)
        {
            IEntityWithRelationships relationships = entity as IEntityWithRelationships;
            if (relationships != null)
            {
                List<T_TargetEntity> spannedEntities = null;
                if (spannedEntity != null)
                {
                    spannedEntities = new List<T_TargetEntity>(1) {
                        spannedEntity
                    };
                }
                else
                {
                    EntityKey sourceKey = ObjectStateManager.FindKeyOnEntityWithRelationships(relationships);
                    this.CheckClearedEntryOnSpan(spannedEntity, relationships, sourceKey, targetMember);
                }
                this.FullSpanAction<T_TargetEntity>(relationships, spannedEntities, targetMember);
            }
            return entity;
        }

        public TEntity HandleIEntityWithKey<TEntity>(TEntity entity, EntitySet entitySet) where TEntity: IEntityWithKey => 
            this.HandleEntity<TEntity>(entity, entity.EntityKey, entitySet);

        public TEntity HandleIEntityWithRelationships<TEntity>(TEntity entity, EntitySet entitySet) where TEntity: IEntityWithRelationships
        {
            if (entitySet != null)
            {
                EntityUtil.AttachContext(entity, this.Context, entitySet, (this.MergeOption == System.Data.Objects.MergeOption.NoTracking) ? System.Data.Objects.MergeOption.NoTracking : System.Data.Objects.MergeOption.AppendOnly);
            }
            return entity;
        }

        public T_SourceEntity HandleRelationshipSpan<T_SourceEntity>(T_SourceEntity entity, EntityKey targetKey, AssociationEndMember targetMember)
        {
            IEntityWithRelationships owner = entity as IEntityWithRelationships;
            if (owner != null)
            {
                EntitySet set;
                EntityState state;
                EntityUtil.CheckKeyForRelationship(owner, this.MergeOption);
                EntityKey sourceKey = ObjectStateManager.FindKeyOnEntityWithRelationships(owner);
                AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
                this.CheckClearedEntryOnSpan(targetKey, owner, sourceKey, targetMember);
                if (targetKey == null)
                {
                    IRelatedEnd end;
                    if (owner.RelationshipManager.TryGetRelatedEnd(otherAssociationEnd.DeclaringType.FullName, targetMember.Name, out end))
                    {
                        this.SetIsLoadedForSpan((RelatedEnd) end, false);
                    }
                    return entity;
                }
                AssociationSet associationSet = MetadataHelper.GetAssociationsForEntitySetAndAssociationType(this.Context.MetadataWorkspace.GetEntityContainer(targetKey.EntityContainerName, DataSpace.CSpace), targetKey.EntitySetName, (AssociationType) targetMember.DeclaringType, targetMember.Name, out set);
                ObjectStateManager objectStateManager = this.Context.ObjectStateManager;
                if (ObjectStateManager.TryUpdateExistingRelationships(this.Context, this.MergeOption, associationSet, otherAssociationEnd, sourceKey, owner, targetMember, targetKey, true, out state))
                {
                    return entity;
                }
                ObjectStateEntry entry = null;
                if (!objectStateManager.TryGetObjectStateEntry(targetKey, out entry))
                {
                    entry = objectStateManager.AddKeyEntry(targetKey, set);
                }
                bool flag = true;
                switch (otherAssociationEnd.RelationshipMultiplicity)
                {
                    case RelationshipMultiplicity.ZeroOrOne:
                    case RelationshipMultiplicity.One:
                        flag = !ObjectStateManager.TryUpdateExistingRelationships(this.Context, this.MergeOption, associationSet, targetMember, targetKey, entry.Entity as IEntityWithRelationships, otherAssociationEnd, sourceKey, true, out state);
                        if (entry.State == EntityState.Detached)
                        {
                            entry = objectStateManager.AddKeyEntry(targetKey, set);
                        }
                        break;
                }
                if (flag)
                {
                    if (entry.IsKeyEntry || (state == EntityState.Deleted))
                    {
                        RelationshipWrapper wrapper = new RelationshipWrapper(associationSet, otherAssociationEnd.Name, sourceKey, targetMember.Name, targetKey);
                        objectStateManager.AddNewRelation(wrapper, state);
                        return entity;
                    }
                    if (entry.State != EntityState.Deleted)
                    {
                        ObjectStateManager.AddEntityToCollectionOrReference(this.MergeOption, owner, otherAssociationEnd, entry.Entity as IEntityWithRelationships, targetMember, true, false, false);
                        return entity;
                    }
                    RelationshipWrapper wrapper2 = new RelationshipWrapper(associationSet, otherAssociationEnd.Name, sourceKey, targetMember.Name, targetKey);
                    objectStateManager.AddNewRelation(wrapper2, EntityState.Deleted);
                }
            }
            return entity;
        }

        public bool SetColumnValue(int recordStateSlotNumber, int ordinal, object value)
        {
            ((RecordState) this.State[recordStateSlotNumber]).SetColumnValue(ordinal, value);
            return true;
        }

        public bool SetEntityRecordInfo(int recordStateSlotNumber, EntityKey entityKey, EntitySet entitySet)
        {
            ((RecordState) this.State[recordStateSlotNumber]).SetEntityRecordInfo(entityKey, entitySet);
            return true;
        }

        private void SetIsLoadedForSpan(RelatedEnd relatedEnd, bool forceToTrue)
        {
            if (!forceToTrue)
            {
                forceToTrue = relatedEnd.IsEmpty();
                EntityReference reference = relatedEnd as EntityReference;
                if (reference != null)
                {
                    forceToTrue &= reference.EntityKey == null;
                }
            }
            if (forceToTrue || (this.MergeOption == System.Data.Objects.MergeOption.OverwriteChanges))
            {
                relatedEnd.SetIsLoaded(true);
            }
        }

        public bool SetState<T>(int ordinal, T value)
        {
            this.State[ordinal] = value;
            return true;
        }

        public T SetStatePassthrough<T>(int ordinal, T value)
        {
            this.State[ordinal] = value;
            return value;
        }

        private void UpdateEntry<TEntity>(TEntity entity, ObjectStateEntry existingEntry)
        {
            Type newEntityType = typeof(TEntity);
            if (newEntityType != existingEntry.Entity.GetType())
            {
                throw EntityUtil.RecyclingEntity(existingEntry.EntityKey, newEntityType, existingEntry.Entity.GetType());
            }
            if (EntityState.Added == existingEntry.State)
            {
                throw EntityUtil.AddedEntityAlreadyExists(existingEntry.EntityKey);
            }
            if (this.MergeOption != System.Data.Objects.MergeOption.AppendOnly)
            {
                if (System.Data.Objects.MergeOption.OverwriteChanges == this.MergeOption)
                {
                    if (EntityState.Deleted == existingEntry.State)
                    {
                        existingEntry.RevertDelete();
                    }
                    UpdateRecord(entity, existingEntry.CurrentValues);
                    existingEntry.AcceptChanges();
                }
                else if (EntityState.Unchanged == existingEntry.State)
                {
                    UpdateRecord(entity, existingEntry.CurrentValues);
                    existingEntry.AcceptChanges();
                }
                else
                {
                    UpdateRecord(entity, existingEntry.EditableOriginalValues);
                }
            }
        }

        internal static void UpdateRecord(object value, CurrentValueRecord current)
        {
            StateManagerTypeMetadata metadata = current._metadata;
            DataRecordInfo dataRecordInfo = metadata.DataRecordInfo;
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(dataRecordInfo.RecordType);
            foreach (FieldMetadata metadata2 in dataRecordInfo.FieldMetadata)
            {
                int index = allStructuralMembers.IndexOf(metadata2.FieldType);
                object obj2 = metadata.Member(index).GetValue(value) ?? DBNull.Value;
                if (Helper.IsComplexType(metadata2.FieldType.TypeUsage.EdmType))
                {
                    object obj3 = current.GetValue(index);
                    if (obj3 == DBNull.Value)
                    {
                        throw EntityUtil.NullableComplexTypesNotSupported(metadata2.FieldType.Name);
                    }
                    if (obj2 != DBNull.Value)
                    {
                        UpdateRecord(obj2, (CurrentValueRecord) obj3);
                    }
                }
                else
                {
                    object obj4 = current.GetValue(index) ?? DBNull.Value;
                    if ((obj4 != obj2) && (((DBNull.Value == obj2) || (DBNull.Value == obj4)) || !obj4.Equals(obj2)))
                    {
                        current.SetValue(index, obj2);
                    }
                }
            }
        }

        private class ColumnErrorHandlingValueReader<TColumn> : Shaper.ErrorHandlingValueReader<TColumn>
        {
            internal ColumnErrorHandlingValueReader()
            {
            }

            protected override Exception CreateNullValueException() => 
                EntityUtil.ValueNullReferenceCast(typeof(TColumn));

            protected override Exception CreateWrongTypeException(Type resultType) => 
                EntityUtil.ValueInvalidCast(resultType, typeof(TColumn));
        }

        private abstract class ErrorHandlingValueReader<T>
        {
            protected ErrorHandlingValueReader()
            {
            }

            protected abstract Exception CreateNullValueException();
            protected abstract Exception CreateWrongTypeException(Type resultType);
            internal T GetValue(DbDataReader reader, int ordinal)
            {
                T local;
                bool flag;
                MethodInfo readerMethod = Translator.GetReaderMethod(typeof(T), out flag);
                if (reader.IsDBNull(ordinal))
                {
                    try
                    {
                        return null;
                    }
                    catch (NullReferenceException)
                    {
                        throw this.CreateNullValueException();
                    }
                }
                try
                {
                    local = (T) readerMethod.Invoke(reader, new object[] { ordinal });
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        Type c = reader.GetValue(ordinal)?.GetType();
                        if (!typeof(T).IsAssignableFrom(c))
                        {
                            throw this.CreateWrongTypeException(c);
                        }
                    }
                    throw;
                }
                return local;
            }
        }

        private class PropertyErrorHandlingValueReader<TProperty> : Shaper.ErrorHandlingValueReader<TProperty>
        {
            private readonly string _propertyName;
            private readonly string _typeName;

            internal PropertyErrorHandlingValueReader(string propertyName, string typeName)
            {
                this._propertyName = propertyName;
                this._typeName = typeName;
            }

            protected override Exception CreateNullValueException() => 
                EntityUtil.Constraint(Strings.Materializer_SetInvalidValue((Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty)).Name, this._typeName, this._propertyName, "null"));

            protected override Exception CreateWrongTypeException(Type resultType) => 
                EntityUtil.InvalidOperation(Strings.Materializer_SetInvalidValue((Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty)).Name, this._typeName, this._propertyName, resultType.Name));
        }
    }
}

