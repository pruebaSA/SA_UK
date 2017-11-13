namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Data.Objects.Internal;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class ObjectStateManager : IEntityStateManager
    {
        private Dictionary<EntityKey, ObjectStateEntry> _addedEntityStore;
        private Dictionary<RelationshipWrapper, ObjectStateEntry> _addedRelationshipStore;
        private string _changingEntityMember;
        private string _changingMember;
        private object _changingObject;
        private object _changingOldValue;
        private EntityState _changingState;
        private System.Data.Objects.Internal.ComplexTypeMaterializer _complexTypeMaterializer;
        private Dictionary<EntityKey, ObjectStateEntry> _deletedEntityStore;
        private Dictionary<RelationshipWrapper, ObjectStateEntry> _deletedRelationshipStore;
        private bool _inRelationshipFixup;
        private Dictionary<object, ObjectStateEntry> _keylessEntityStore;
        private readonly Dictionary<EntitySetQualifiedType, StateManagerTypeMetadata> _metadataMapping;
        private readonly Dictionary<EdmType, StateManagerTypeMetadata> _metadataStore;
        private readonly System.Data.Metadata.Edm.MetadataWorkspace _metadataWorkspace;
        private Dictionary<EntityKey, ObjectStateEntry> _modifiedEntityStore;
        private Dictionary<object, ObjectStateEntry> _promotedKeyEntries;
        private Dictionary<RelatedEnd, IList<object>> _promotedRelationships;
        private bool _saveOriginalValues;
        private Dictionary<EntityKey, ObjectStateEntry> _unchangedEntityStore;
        private Dictionary<RelationshipWrapper, ObjectStateEntry> _unchangedRelationshipStore;

        internal event CollectionChangeEventHandler EntityDeleted;

        public event CollectionChangeEventHandler ObjectStateManagerChanged;

        public ObjectStateManager(System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace)
        {
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.MetadataWorkspace>(metadataWorkspace, "metadataWorkspace");
            this._metadataWorkspace = metadataWorkspace;
            this._metadataStore = new Dictionary<EdmType, StateManagerTypeMetadata>();
            this._metadataMapping = new Dictionary<EntitySetQualifiedType, StateManagerTypeMetadata>(EntitySetQualifiedType.EqualityComparer);
        }

        private void AddEntityEntryToDictionary(ObjectStateEntry entry, EntityState state)
        {
            Dictionary<EntityKey, ObjectStateEntry> dictionary = null;
            switch (state)
            {
                case EntityState.Unchanged:
                    if (this._unchangedEntityStore == null)
                    {
                        this._unchangedEntityStore = new Dictionary<EntityKey, ObjectStateEntry>();
                    }
                    dictionary = this._unchangedEntityStore;
                    break;

                case EntityState.Added:
                    if (this._addedEntityStore == null)
                    {
                        this._addedEntityStore = new Dictionary<EntityKey, ObjectStateEntry>();
                    }
                    dictionary = this._addedEntityStore;
                    break;

                case EntityState.Deleted:
                    if (this._deletedEntityStore == null)
                    {
                        this._deletedEntityStore = new Dictionary<EntityKey, ObjectStateEntry>();
                    }
                    dictionary = this._deletedEntityStore;
                    break;

                case EntityState.Modified:
                    if (this._modifiedEntityStore == null)
                    {
                        this._modifiedEntityStore = new Dictionary<EntityKey, ObjectStateEntry>();
                    }
                    dictionary = this._modifiedEntityStore;
                    break;
            }
            dictionary.Add(entry.EntityKey, entry);
            this.AddEntryToKeylessStore(entry.Entity, entry);
        }

        internal static void AddEntityToCollectionOrReference(MergeOption mergeOption, IEntityWithRelationships sourceEntity, AssociationEndMember sourceMember, IEntityWithRelationships targetEntity, AssociationEndMember targetMember, bool setIsLoaded, bool relationshipAlreadyExists, bool inKeyEntryPromotion)
        {
            RelatedEnd relatedEnd = (RelatedEnd) sourceEntity.RelationshipManager.GetRelatedEnd(sourceMember.DeclaringType.FullName, targetMember.Name);
            if (targetMember.RelationshipMultiplicity != RelationshipMultiplicity.Many)
            {
                switch (mergeOption)
                {
                    case MergeOption.AppendOnly:
                        if (inKeyEntryPromotion && !relatedEnd.IsEmpty())
                        {
                            throw EntityUtil.EntityConflictsWithKeyEntry();
                        }
                        break;

                    case MergeOption.OverwriteChanges:
                    case MergeOption.PreserveChanges:
                    {
                        IEntityWithRelationships entity = null;
                        foreach (IEntityWithRelationships relationships2 in relatedEnd)
                        {
                            entity = relationships2;
                        }
                        if (entity != null)
                        {
                            ObjectStateEntry entry = relatedEnd.FindRelationshipEntryInObjectStateManager(entity);
                            relatedEnd.RemoveAll();
                            if (entry.State == EntityState.Deleted)
                            {
                                entry.AcceptChanges();
                            }
                        }
                        break;
                    }
                    case MergeOption.NoTracking:
                        if (!relatedEnd.IsEmpty())
                        {
                            throw EntityUtil.CannotAddMoreThanOneEntityToEntityReferenceTryOtherMergeOption();
                        }
                        break;
                }
            }
            RelatedEnd otherEndOfRelationship = null;
            if (mergeOption == MergeOption.NoTracking)
            {
                otherEndOfRelationship = relatedEnd.GetOtherEndOfRelationship(targetEntity);
                if (otherEndOfRelationship.IsLoaded)
                {
                    throw EntityUtil.CannotFillTryDifferentMergeOption(otherEndOfRelationship.SourceRoleName, otherEndOfRelationship.RelationshipName);
                }
            }
            relatedEnd.Add(targetEntity, true, true, relationshipAlreadyExists);
            UpdateRelatedEnd(relatedEnd, targetEntity, setIsLoaded, mergeOption);
            if (otherEndOfRelationship == null)
            {
                otherEndOfRelationship = relatedEnd.GetOtherEndOfRelationship(targetEntity);
            }
            UpdateRelatedEnd(otherEndOfRelationship, sourceEntity, setIsLoaded, mergeOption);
            if (inKeyEntryPromotion && sourceEntity.RelationshipManager.Context.ObjectStateManager.IsAttachTracking)
            {
                sourceEntity.RelationshipManager.Context.ObjectStateManager.TrackPromotedRelationship(relatedEnd, targetEntity);
                sourceEntity.RelationshipManager.Context.ObjectStateManager.TrackPromotedRelationship(otherEndOfRelationship, sourceEntity);
            }
        }

        internal void AddEntry(object dataObject, EntityKey passedKey, EntitySet entitySet, string argumentName, bool isAdded)
        {
            ObjectStateEntry entry2;
            EntityKey entityKey = passedKey;
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(dataObject.GetType(), entitySet);
            EdmType edmType = orAddStateManagerTypeMetadata.CdmMetadata.EdmType;
            if (isAdded && !entitySet.ElementType.IsAssignableFrom(edmType))
            {
                throw EntityUtil.EntityTypeDoesNotMatchEntitySet(dataObject.GetType().Name, TypeHelpers.GetFullName(entitySet), argumentName);
            }
            IEntityWithKey entity = dataObject as IEntityWithKey;
            if (entity != null)
            {
                EntityKey key3 = entity.EntityKey;
                if (key3 != null)
                {
                    entityKey = key3;
                    EntityUtil.CheckEntityKeyNull(entityKey);
                    EntityUtil.CheckEntityKeysMatch(entity, entityKey);
                }
            }
            if (entityKey == null)
            {
                entityKey = new EntityKey(entitySet);
                EntityUtil.SetKeyOntoEntity(entity, entityKey);
            }
            ObjectStateEntry entry = new EntityEntry(dataObject, entityKey, entitySet, this, orAddStateManagerTypeMetadata, isAdded);
            if (isAdded && ((entry2 = this.FindObjectStateEntry(entityKey)) != null))
            {
                if (entry2.Entity != dataObject)
                {
                    throw EntityUtil.ObjectStateManagerContainsThisEntityKey();
                }
                if (entry2.State != EntityState.Added)
                {
                    throw EntityUtil.ObjectStateManagerDoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(entry2.State);
                }
            }
            else
            {
                if ((entry.State == EntityState.Added) && !entityKey.IsTemporary)
                {
                    throw EntityUtil.CannotAddEntityWithKeyAttached();
                }
                entry.AttachObjectStateManagerToEntity();
                this.AddEntityEntryToDictionary(entry, entry.State);
                this.OnObjectStateManagerChanged(CollectionChangeAction.Add, entry.Entity);
            }
        }

        private void AddEntryToKeylessStore(object entity, ObjectStateEntry entry)
        {
            if ((entry.Entity != null) && !(entry.Entity is IEntityWithKey))
            {
                if (this._keylessEntityStore == null)
                {
                    this._keylessEntityStore = new Dictionary<object, ObjectStateEntry>(new ObjectReferenceEqualityComparer());
                }
                if (!this._keylessEntityStore.ContainsKey(entry.Entity))
                {
                    this._keylessEntityStore.Add(entry.Entity, entry);
                }
            }
        }

        internal ObjectStateEntry AddKeyEntry(EntityKey entityKey, EntitySet entitySet)
        {
            if (this.FindObjectStateEntry(entityKey) != null)
            {
                throw EntityUtil.ObjectStateManagerContainsThisEntityKey();
            }
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(entitySet.ElementType);
            ObjectStateEntry entry = new EntityEntry(entityKey, entitySet, this, orAddStateManagerTypeMetadata);
            this.AddEntityEntryToDictionary(entry, entry.State);
            return entry;
        }

        internal ObjectStateEntry AddNewRelation(RelationshipWrapper wrapper, EntityState desiredState)
        {
            RelationshipEntry entry = new RelationshipEntry(this, desiredState, wrapper);
            this.AddRelationshipEntryToDictionary(entry, desiredState);
            this.AddRelationshipToLookup(entry);
            return entry;
        }

        internal ObjectStateEntry AddRelation(RelationshipWrapper wrapper, EntityState desiredState)
        {
            ObjectStateEntry entry = this.FindRelationship(wrapper);
            if (entry == null)
            {
                return this.AddNewRelation(wrapper, desiredState);
            }
            if (EntityState.Deleted != entry.State)
            {
                if (EntityState.Unchanged == desiredState)
                {
                    entry.AcceptChanges();
                    return entry;
                }
                if (EntityState.Deleted == desiredState)
                {
                    entry.AcceptChanges();
                    entry.Delete(false);
                }
                return entry;
            }
            if (EntityState.Deleted != desiredState)
            {
                entry.RevertDelete();
            }
            return entry;
        }

        private void AddRelationshipEndToLookup(EntityKey key, RelationshipEntry relationship)
        {
            ((EntityEntry) this.GetObjectStateEntry(key)).AddRelationshipEnd(relationship);
        }

        private void AddRelationshipEntryToDictionary(ObjectStateEntry entry, EntityState state)
        {
            Dictionary<RelationshipWrapper, ObjectStateEntry> dictionary = null;
            switch (state)
            {
                case EntityState.Unchanged:
                    if (this._unchangedRelationshipStore == null)
                    {
                        this._unchangedRelationshipStore = new Dictionary<RelationshipWrapper, ObjectStateEntry>();
                    }
                    dictionary = this._unchangedRelationshipStore;
                    break;

                case EntityState.Added:
                    if (this._addedRelationshipStore == null)
                    {
                        this._addedRelationshipStore = new Dictionary<RelationshipWrapper, ObjectStateEntry>();
                    }
                    dictionary = this._addedRelationshipStore;
                    break;

                case EntityState.Deleted:
                    if (this._deletedRelationshipStore == null)
                    {
                        this._deletedRelationshipStore = new Dictionary<RelationshipWrapper, ObjectStateEntry>();
                    }
                    dictionary = this._deletedRelationshipStore;
                    break;
            }
            dictionary.Add(entry.Wrapper, entry);
        }

        private void AddRelationshipToLookup(RelationshipEntry relationship)
        {
            this.AddRelationshipEndToLookup(relationship.Wrapper.Key0, relationship);
            if (!relationship.Wrapper.Key0.Equals(relationship.Wrapper.Key1))
            {
                this.AddRelationshipEndToLookup(relationship.Wrapper.Key1, relationship);
            }
        }

        private StateManagerTypeMetadata AddStateManagerTypeMetadata(EdmType edmType, ObjectTypeMapping mapping)
        {
            StateManagerTypeMetadata metadata = new StateManagerTypeMetadata(edmType, mapping);
            this._metadataStore.Add(edmType, metadata);
            return metadata;
        }

        private StateManagerTypeMetadata AddStateManagerTypeMetadata(EntitySet entitySet, ObjectTypeMapping mapping)
        {
            StateManagerTypeMetadata metadata;
            EdmType edmType = mapping.EdmType;
            if (!this._metadataStore.TryGetValue(edmType, out metadata))
            {
                metadata = new StateManagerTypeMetadata(edmType, mapping);
                this._metadataStore.Add(edmType, metadata);
            }
            this._metadataMapping.Add(new EntitySetQualifiedType(mapping.ClrType.ClrType, entitySet), metadata);
            return metadata;
        }

        [Conditional("DEBUG")]
        private void AssertKeyMatchesEntity(object entity, EntityKey entityKey, EntitySet entitySetForType)
        {
        }

        internal ObjectStateEntry AttachEntry(EntityKey entityKey, object dataObject, EntitySet entitySet, string argumentName)
        {
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(dataObject.GetType(), entitySet);
            this.CheckKeyMatchesEntity(dataObject, entityKey, entitySet);
            ObjectStateEntry entry = new EntityEntry(dataObject, entityKey, entitySet, this, orAddStateManagerTypeMetadata, true) {
                State = EntityState.Unchanged
            };
            entry.AttachObjectStateManagerToEntity();
            this.AddEntityEntryToDictionary(entry, entry.State);
            this.OnObjectStateManagerChanged(CollectionChangeAction.Add, entry.Entity);
            return entry;
        }

        internal void BeginAttachTracking()
        {
            this._promotedRelationships = new Dictionary<RelatedEnd, IList<object>>();
            this._promotedKeyEntries = new Dictionary<object, ObjectStateEntry>();
        }

        internal void ChangeState(ObjectStateEntry entry, EntityState oldState, EntityState newState)
        {
            bool flag = !entry.IsRelationship && !entry.IsKeyEntry;
            if (newState == EntityState.Detached)
            {
                if (entry.IsRelationship)
                {
                    this.DeleteRelationshipFromLookup((RelationshipEntry) entry);
                }
                else
                {
                    RelationshipEntry[] entryArray = this.CopyOfRelationshipsByKey(entry.EntityKey);
                    for (int i = 0; i < entryArray.Length; i++)
                    {
                        ObjectStateEntry entry2 = entryArray[i];
                        this.ChangeState(entry2, entry2.State, EntityState.Detached);
                    }
                }
                this.RemoveObjectStateEntryFromDictionary(entry, oldState);
                object entity = entry.Entity;
                entry.Reset();
                if (flag && (entity != null))
                {
                    this.OnEntityDeleted(CollectionChangeAction.Remove, entity);
                    this.OnObjectStateManagerChanged(CollectionChangeAction.Remove, entity);
                }
            }
            else
            {
                this.RemoveObjectStateEntryFromDictionary(entry, oldState);
                if (entry.IsRelationship)
                {
                    this.AddRelationshipEntryToDictionary(entry, newState);
                }
                else
                {
                    this.AddEntityEntryToDictionary(entry, newState);
                }
            }
            if (flag && (newState == EntityState.Deleted))
            {
                this.OnEntityDeleted(CollectionChangeAction.Remove, entry.Entity);
                this.OnObjectStateManagerChanged(CollectionChangeAction.Remove, entry.Entity);
            }
        }

        private void CheckKeyMatchesEntity(object entity, EntityKey entityKey, EntitySet entitySetForType)
        {
            EntitySet entitySet = entityKey.GetEntitySet(this.MetadataWorkspace);
            if (entitySet == null)
            {
                throw EntityUtil.InvalidKey();
            }
            if ((entitySetForType.Name != entitySet.Name) || (entitySetForType.EntityContainer.Name != entitySet.EntityContainer.Name))
            {
                throw EntityUtil.EntityTypeDoesntMatchEntitySetFromKey();
            }
            entityKey.ValidateEntityKey(entitySet);
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(entity.GetType(), entitySetForType);
            for (int i = 0; i < entitySet.ElementType.KeyMembers.Count; i++)
            {
                EdmMember member = entitySet.ElementType.KeyMembers[i];
                int ordinalforCLayerMemberName = orAddStateManagerTypeMetadata.GetOrdinalforCLayerMemberName(member.Name);
                if (ordinalforCLayerMemberName < 0)
                {
                    throw EntityUtil.InvalidKey();
                }
                object objA = orAddStateManagerTypeMetadata.Member(ordinalforCLayerMemberName).GetValue(entity);
                object objB = entityKey.FindValueByName(member.Name);
                if (!object.Equals(objA, objB))
                {
                    throw EntityUtil.KeyPropertyDoesntMatchValueInKey();
                }
            }
        }

        internal RelationshipEntry[] CopyOfRelationshipsByKey(EntityKey key) => 
            this.FindRelationshipsByKey(key).ToArray();

        internal void DegradePromotedRelationships()
        {
            foreach (KeyValuePair<RelatedEnd, IList<object>> pair in this.PromotedRelationships)
            {
                foreach (object obj2 in pair.Value)
                {
                    if (pair.Key.RemoveEntityFromLocallyCachedCollection(obj2 as IEntityWithRelationships, false))
                    {
                        pair.Key.OnAssociationChanged(CollectionChangeAction.Remove, obj2);
                    }
                }
            }
        }

        internal void DeleteKeyEntry(ObjectStateEntry keyEntry)
        {
            if ((keyEntry != null) && keyEntry.IsKeyEntry)
            {
                this.ChangeState(keyEntry, keyEntry.State, EntityState.Detached);
            }
        }

        internal ObjectStateEntry DeleteRelationship(RelationshipSet relationshipSet, KeyValuePair<string, EntityKey> roleAndKey1, KeyValuePair<string, EntityKey> roleAndKey2)
        {
            ObjectStateEntry entry = this.FindRelationship(relationshipSet, roleAndKey1, roleAndKey2);
            if (entry != null)
            {
                entry.Delete(false);
            }
            return entry;
        }

        private void DeleteRelationshipEndFromLookup(EntityKey key, RelationshipEntry relationship)
        {
            ((EntityEntry) this.GetObjectStateEntry(key)).RemoveRelationshipEnd(relationship);
        }

        private void DeleteRelationshipFromLookup(RelationshipEntry relationship)
        {
            this.DeleteRelationshipEndFromLookup(relationship.Wrapper.Key0, relationship);
            if (!relationship.Wrapper.Key0.Equals(relationship.Wrapper.Key1))
            {
                this.DeleteRelationshipEndFromLookup(relationship.Wrapper.Key1, relationship);
            }
        }

        internal void EndAttachTracking()
        {
            this._promotedRelationships = null;
            this._promotedKeyEntries = null;
        }

        internal static EntityKey FindKeyOnEntityWithRelationships(IEntityWithRelationships entity)
        {
            IEntityWithKey key = entity as IEntityWithKey;
            if (key != null)
            {
                return key.EntityKey;
            }
            RelationshipManager relationshipManager = entity.RelationshipManager;
            if ((relationshipManager != null) && (relationshipManager.Context != null))
            {
                return ObjectContext.FindEntityKey(entity, relationshipManager.Context);
            }
            return null;
        }

        internal ObjectStateEntry FindObjectStateEntry(EntityKey key)
        {
            ObjectStateEntry entry = null;
            if (key != null)
            {
                this.TryGetObjectStateEntry(key, out entry);
            }
            return entry;
        }

        internal ObjectStateEntry FindObjectStateEntry(object entity)
        {
            ObjectStateEntry entry = null;
            IEntityWithKey key = entity as IEntityWithKey;
            if (key != null)
            {
                EntityKey entityKey = key.EntityKey;
                if (entityKey != null)
                {
                    this.TryGetObjectStateEntry(entityKey, out entry);
                }
            }
            else
            {
                this.TryGetEntryFromKeylessStore(entity, out entry);
            }
            if ((entry != null) && !object.ReferenceEquals(entity, entry.Entity))
            {
                entry = null;
            }
            return entry;
        }

        internal ObjectStateEntry FindRelationship(RelationshipWrapper relationshipWrapper)
        {
            ObjectStateEntry entry = null;
            if ((this._unchangedRelationshipStore == null) || !this._unchangedRelationshipStore.TryGetValue(relationshipWrapper, out entry))
            {
                if ((this._deletedRelationshipStore != null) && this._deletedRelationshipStore.TryGetValue(relationshipWrapper, out entry))
                {
                    return entry;
                }
                if (this._addedRelationshipStore != null)
                {
                    this._addedRelationshipStore.TryGetValue(relationshipWrapper, out entry);
                }
            }
            return entry;
        }

        internal ObjectStateEntry FindRelationship(RelationshipSet relationshipSet, KeyValuePair<string, EntityKey> roleAndKey1, KeyValuePair<string, EntityKey> roleAndKey2)
        {
            if ((roleAndKey1.Value != null) && (roleAndKey2.Value != null))
            {
                return this.FindRelationship(new RelationshipWrapper((AssociationSet) relationshipSet, roleAndKey1, roleAndKey2));
            }
            return null;
        }

        internal EntityEntry.RelationshipEndEnumerable FindRelationshipsByKey(EntityKey key) => 
            new EntityEntry.RelationshipEndEnumerable((EntityEntry) this.FindObjectStateEntry(key));

        internal void FixupKey(EntityEntry entry)
        {
            EntityKey entityKey = entry.EntityKey;
            EntityKey key = new EntityKey((EntitySet) entry.EntitySet, entry.CurrentValues);
            ObjectStateEntry keyEntry = this.FindObjectStateEntry(key);
            if (keyEntry != null)
            {
                if (!keyEntry.IsKeyEntry)
                {
                    throw EntityUtil.CannotFixUpKeyToExistingValues();
                }
                key = keyEntry.EntityKey;
            }
            RelationshipEntry[] entryArray = entry.GetRelationshipEnds().ToArray();
            foreach (RelationshipEntry entry3 in entryArray)
            {
                this.RemoveObjectStateEntryFromDictionary(entry3, entry3.State);
            }
            this.RemoveObjectStateEntryFromDictionary(entry, EntityState.Added);
            this.ResetEntityKey(entry, key);
            entry.UpdateRelationshipEnds(entityKey, (EntityEntry) keyEntry);
            foreach (RelationshipEntry entry4 in entryArray)
            {
                this.AddRelationshipEntryToDictionary(entry4, entry4.State);
            }
            if (keyEntry != null)
            {
                this.PromoteKeyEntry(keyEntry, entry.Entity, null, true, false, false, "AcceptChanges");
            }
            else
            {
                this.AddEntityEntryToDictionary(entry, EntityState.Unchanged);
            }
        }

        internal EntityKey GetEntityKey(object entity)
        {
            EntityKey key = null;
            if (!this.TryGetEntityKey(entity, out key))
            {
                throw EntityUtil.ObjectDoesNotHaveAKey(entity);
            }
            return key;
        }

        public IEnumerable<ObjectStateEntry> GetObjectStateEntries(EntityState state)
        {
            if ((EntityState.Detached & state) != 0)
            {
                throw EntityUtil.DetachedObjectStateEntriesDoesNotExistInObjectStateManager();
            }
            return this.GetObjectStateEntriesInternal(state);
        }

        internal int GetObjectStateEntriesCount(EntityState state)
        {
            int num = 0;
            if ((EntityState.Added & state) != 0)
            {
                num += (this._addedRelationshipStore != null) ? this._addedRelationshipStore.Count : 0;
                num += (this._addedEntityStore != null) ? this._addedEntityStore.Count : 0;
            }
            if ((EntityState.Modified & state) != 0)
            {
                num += (this._modifiedEntityStore != null) ? this._modifiedEntityStore.Count : 0;
            }
            if ((EntityState.Deleted & state) != 0)
            {
                num += (this._deletedRelationshipStore != null) ? this._deletedRelationshipStore.Count : 0;
                num += (this._deletedEntityStore != null) ? this._deletedEntityStore.Count : 0;
            }
            if ((EntityState.Unchanged & state) != 0)
            {
                num += (this._unchangedRelationshipStore != null) ? this._unchangedRelationshipStore.Count : 0;
                num += (this._unchangedEntityStore != null) ? this._unchangedEntityStore.Count : 0;
            }
            return num;
        }

        private ObjectStateEntry[] GetObjectStateEntriesInternal(EntityState state)
        {
            ObjectStateEntry[] array = new ObjectStateEntry[this.GetObjectStateEntriesCount(state)];
            int index = 0;
            if (((EntityState.Added & state) != 0) && (this._addedRelationshipStore != null))
            {
                this._addedRelationshipStore.Values.CopyTo(array, index);
                index += this._addedRelationshipStore.Count;
            }
            if (((EntityState.Deleted & state) != 0) && (this._deletedRelationshipStore != null))
            {
                this._deletedRelationshipStore.Values.CopyTo(array, index);
                index += this._deletedRelationshipStore.Count;
            }
            if (((EntityState.Unchanged & state) != 0) && (this._unchangedRelationshipStore != null))
            {
                this._unchangedRelationshipStore.Values.CopyTo(array, index);
                index += this._unchangedRelationshipStore.Count;
            }
            if (((EntityState.Added & state) != 0) && (this._addedEntityStore != null))
            {
                this._addedEntityStore.Values.CopyTo(array, index);
                index += this._addedEntityStore.Count;
            }
            if (((EntityState.Modified & state) != 0) && (this._modifiedEntityStore != null))
            {
                this._modifiedEntityStore.Values.CopyTo(array, index);
                index += this._modifiedEntityStore.Count;
            }
            if (((EntityState.Deleted & state) != 0) && (this._deletedEntityStore != null))
            {
                this._deletedEntityStore.Values.CopyTo(array, index);
                index += this._deletedEntityStore.Count;
            }
            if (((EntityState.Unchanged & state) != 0) && (this._unchangedEntityStore != null))
            {
                this._unchangedEntityStore.Values.CopyTo(array, index);
                index += this._unchangedEntityStore.Count;
            }
            return array;
        }

        public ObjectStateEntry GetObjectStateEntry(EntityKey key)
        {
            ObjectStateEntry entry;
            if (!this.TryGetObjectStateEntry(key, out entry))
            {
                throw EntityUtil.NoEntryExistForEntityKey();
            }
            return entry;
        }

        public ObjectStateEntry GetObjectStateEntry(object entity)
        {
            ObjectStateEntry entry;
            if (!this.TryGetObjectStateEntry(entity, out entry))
            {
                throw EntityUtil.NoEntryExistsForObject(entity);
            }
            return entry;
        }

        internal StateManagerTypeMetadata GetOrAddStateManagerTypeMetadata(EdmType edmType)
        {
            StateManagerTypeMetadata metadata;
            if (!this._metadataStore.TryGetValue(edmType, out metadata))
            {
                metadata = this.AddStateManagerTypeMetadata(edmType, (ObjectTypeMapping) this.MetadataWorkspace.GetMap(edmType, DataSpace.OCSpace));
            }
            return metadata;
        }

        internal StateManagerTypeMetadata GetOrAddStateManagerTypeMetadata(Type entityType, EntitySet entitySet)
        {
            StateManagerTypeMetadata metadata;
            if (!this._metadataMapping.TryGetValue(new EntitySetQualifiedType(entityType, entitySet), out metadata))
            {
                metadata = this.AddStateManagerTypeMetadata(entitySet, (ObjectTypeMapping) this.MetadataWorkspace.GetMap(entityType.FullName, DataSpace.OSpace, DataSpace.OCSpace));
            }
            return metadata;
        }

        private void OnEntityDeleted(CollectionChangeAction action, object obj)
        {
            if (this.onEntityDeletedDelegate != null)
            {
                this.onEntityDeletedDelegate(this, new CollectionChangeEventArgs(action, obj));
            }
        }

        internal void OnObjectStateManagerChanged(CollectionChangeAction action, object obj)
        {
            if (this.onObjectStateManagerChangedDelegate != null)
            {
                this.onObjectStateManagerChangedDelegate(this, new CollectionChangeEventArgs(action, obj));
            }
        }

        internal void PromoteKeyEntry(ObjectStateEntry keyEntry, object entity, IExtendedDataRecord shadowValues, bool replacingEntry, bool setIsLoaded, bool keyEntryInitialized, string argumentName)
        {
            if (!keyEntryInitialized)
            {
                this.PromoteKeyEntryInitialization(keyEntry, entity, shadowValues, replacingEntry);
            }
            bool flag = true;
            try
            {
                RelationshipEntry[] entryArray = this.CopyOfRelationshipsByKey(keyEntry.EntityKey);
                for (int i = 0; i < entryArray.Length; i++)
                {
                    ObjectStateEntry relationshipEntry = entryArray[i];
                    if (relationshipEntry.State != EntityState.Deleted)
                    {
                        AssociationEndMember associationEndMember = keyEntry.GetAssociationEndMember(relationshipEntry);
                        AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(associationEndMember);
                        ObjectStateEntry otherEndOfRelationship = keyEntry.GetOtherEndOfRelationship(relationshipEntry);
                        AddEntityToCollectionOrReference(MergeOption.AppendOnly, entity as IEntityWithRelationships, associationEndMember, otherEndOfRelationship.Entity as IEntityWithRelationships, otherAssociationEnd, setIsLoaded, true, true);
                    }
                }
                flag = false;
            }
            finally
            {
                if (flag)
                {
                    keyEntry.DetachObjectStateManagerFromEntity();
                    this.RemoveEntryFromKeylessStore(entity);
                    keyEntry.DegradeEntry();
                }
            }
            if (this.IsAttachTracking)
            {
                this.PromotedKeyEntries.Add(entity, keyEntry);
            }
        }

        internal void PromoteKeyEntryInitialization(ObjectStateEntry keyEntry, object entity, IExtendedDataRecord shadowValues, bool replacingEntry)
        {
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(entity.GetType(), (EntitySet) keyEntry.EntitySet);
            keyEntry.PromoteKeyEntry(entity, shadowValues, orAddStateManagerTypeMetadata);
            this.AddEntryToKeylessStore(entity, keyEntry);
            if (replacingEntry)
            {
                EntityUtil.SetChangeTrackerOntoEntity(entity, null);
            }
            keyEntry.AttachObjectStateManagerToEntity();
            this.OnObjectStateManagerChanged(CollectionChangeAction.Add, keyEntry.Entity);
        }

        internal void RemoveEntryFromKeylessStore(object entity)
        {
            if ((entity != null) && !(entity is IEntityWithKey))
            {
                this._keylessEntityStore.Remove(entity);
            }
        }

        private void RemoveObjectStateEntryFromDictionary(ObjectStateEntry entry, EntityState state)
        {
            Dictionary<EntityKey, ObjectStateEntry> dictionary2;
            if (!entry.IsRelationship)
            {
                dictionary2 = null;
                switch (state)
                {
                    case EntityState.Unchanged:
                        dictionary2 = this._unchangedEntityStore;
                        break;

                    case EntityState.Added:
                        dictionary2 = this._addedEntityStore;
                        break;

                    case EntityState.Deleted:
                        dictionary2 = this._deletedEntityStore;
                        break;

                    case EntityState.Modified:
                        dictionary2 = this._modifiedEntityStore;
                        break;
                }
            }
            else
            {
                Dictionary<RelationshipWrapper, ObjectStateEntry> dictionary = null;
                switch (state)
                {
                    case EntityState.Unchanged:
                        dictionary = this._unchangedRelationshipStore;
                        break;

                    case EntityState.Added:
                        dictionary = this._addedRelationshipStore;
                        break;

                    case EntityState.Deleted:
                        dictionary = this._deletedRelationshipStore;
                        break;
                }
                dictionary.Remove(entry.Wrapper);
                if (dictionary.Count != 0)
                {
                    return;
                }
                switch (state)
                {
                    case EntityState.Unchanged:
                        this._unchangedRelationshipStore = null;
                        return;

                    case (EntityState.Unchanged | EntityState.Detached):
                        return;

                    case EntityState.Added:
                        this._addedRelationshipStore = null;
                        return;

                    case EntityState.Deleted:
                        this._deletedRelationshipStore = null;
                        return;

                    default:
                        return;
                }
            }
            dictionary2.Remove(entry.EntityKey);
            this.RemoveEntryFromKeylessStore(entry.Entity);
            if (dictionary2.Count == 0)
            {
                switch (state)
                {
                    case EntityState.Unchanged:
                        this._unchangedEntityStore = null;
                        return;

                    case (EntityState.Unchanged | EntityState.Detached):
                        return;

                    case EntityState.Added:
                        this._addedEntityStore = null;
                        return;

                    case EntityState.Deleted:
                        this._deletedEntityStore = null;
                        return;

                    case EntityState.Modified:
                        this._modifiedEntityStore = null;
                        return;
                }
            }
        }

        internal static void RemoveRelatedEndsAndDetachRelationship(ObjectStateEntry relationshipToRemove, bool setIsLoaded)
        {
            if (setIsLoaded)
            {
                UnloadReferenceRelatedEnds(relationshipToRemove);
            }
            if (relationshipToRemove.State != EntityState.Deleted)
            {
                relationshipToRemove.Delete();
            }
            if (relationshipToRemove.State != EntityState.Detached)
            {
                relationshipToRemove.AcceptChanges();
            }
        }

        internal static void RemoveRelationships(ObjectContext context, MergeOption mergeOption, AssociationSet associationSet, EntityKey sourceKey, AssociationEndMember sourceMember)
        {
            List<ObjectStateEntry> list = new List<ObjectStateEntry>();
            if (mergeOption == MergeOption.OverwriteChanges)
            {
                foreach (ObjectStateEntry entry in context.ObjectStateManager.FindRelationshipsByKey(sourceKey))
                {
                    if (entry.IsSameAssociationSetAndRole(associationSet, sourceKey, sourceMember))
                    {
                        list.Add(entry);
                    }
                }
            }
            else if (mergeOption == MergeOption.PreserveChanges)
            {
                foreach (ObjectStateEntry entry2 in context.ObjectStateManager.FindRelationshipsByKey(sourceKey))
                {
                    if (entry2.IsSameAssociationSetAndRole(associationSet, sourceKey, sourceMember) && (entry2.State != EntityState.Added))
                    {
                        list.Add(entry2);
                    }
                }
            }
            foreach (ObjectStateEntry entry3 in list)
            {
                RemoveRelatedEndsAndDetachRelationship(entry3, true);
            }
        }

        private void ResetEntityKey(ObjectStateEntry entry, EntityKey value)
        {
            IEntityWithKey entity = entry.Entity as IEntityWithKey;
            if (entity != null)
            {
                EntityKey entityKey = entity.EntityKey;
                if ((entityKey == null) || value.Equals(entityKey))
                {
                    throw EntityUtil.AcceptChangesEntityKeyIsNotValid();
                }
                try
                {
                    this._inRelationshipFixup = true;
                    entity.EntityKey = value;
                    EntityUtil.CheckEntityKeysMatch(entity, value);
                }
                finally
                {
                    this._inRelationshipFixup = false;
                }
            }
            entry.EntityKey = value;
        }

        IEnumerable<IEntityStateEntry> IEntityStateManager.FindRelationshipsByKey(EntityKey key) => 
            this.FindRelationshipsByKey(key);

        IEnumerable<IEntityStateEntry> IEntityStateManager.GetEntityStateEntries(EntityState state)
        {
            foreach (ObjectStateEntry iteratorVariable0 in this.GetObjectStateEntriesInternal(state))
            {
                yield return iteratorVariable0;
            }
        }

        IEntityStateEntry IEntityStateManager.GetEntityStateEntry(EntityKey key) => 
            this.GetObjectStateEntry(key);

        bool IEntityStateManager.TryGetEntityStateEntry(EntityKey key, out IEntityStateEntry entry)
        {
            ObjectStateEntry entry2;
            bool flag = this.TryGetObjectStateEntry(key, out entry2);
            entry = entry2;
            return flag;
        }

        internal void TrackPromotedRelationship(RelatedEnd relatedEnd, object entity)
        {
            IList<object> list;
            if (!this.PromotedRelationships.TryGetValue(relatedEnd, out list))
            {
                list = new List<object>();
                this.PromotedRelationships.Add(relatedEnd, list);
            }
            list.Add(entity);
        }

        internal bool TryGetEntityKey(object entity, out EntityKey key)
        {
            key = null;
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            IEntityWithKey key2 = entity as IEntityWithKey;
            if (key2 != null)
            {
                key = key2.EntityKey;
            }
            else
            {
                ObjectStateEntry entry;
                if (this.TryGetEntryFromKeylessStore(entity, out entry))
                {
                    key = entry.EntityKey;
                }
            }
            return (null != key);
        }

        private bool TryGetEntryFromKeylessStore(object entity, out ObjectStateEntry entryRef)
        {
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            entryRef = null;
            if ((this._keylessEntityStore != null) && this._keylessEntityStore.TryGetValue(entity, out entryRef))
            {
                return true;
            }
            entryRef = null;
            return false;
        }

        public bool TryGetObjectStateEntry(EntityKey key, out ObjectStateEntry entry)
        {
            entry = null;
            EntityUtil.CheckArgumentNull<EntityKey>(key, "key");
            if (key.IsTemporary)
            {
                return ((this._addedEntityStore != null) && this._addedEntityStore.TryGetValue(key, out entry));
            }
            return ((((this._unchangedEntityStore != null) && this._unchangedEntityStore.TryGetValue(key, out entry)) || ((this._modifiedEntityStore != null) && this._modifiedEntityStore.TryGetValue(key, out entry))) || ((this._deletedEntityStore != null) && this._deletedEntityStore.TryGetValue(key, out entry)));
        }

        public bool TryGetObjectStateEntry(object entity, out ObjectStateEntry entry)
        {
            entry = null;
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            entry = this.FindObjectStateEntry(entity);
            return (entry != null);
        }

        internal static bool TryUpdateExistingRelationships(ObjectContext context, MergeOption mergeOption, AssociationSet associationSet, AssociationEndMember sourceMember, EntityKey sourceKey, IEntityWithRelationships sourceEntity, AssociationEndMember targetMember, EntityKey targetKey, bool setIsLoaded, out EntityState newEntryState)
        {
            newEntryState = EntityState.Unchanged;
            bool flag = true;
            ObjectStateManager objectStateManager = context.ObjectStateManager;
            List<ObjectStateEntry> list = null;
            List<ObjectStateEntry> list2 = null;
            foreach (ObjectStateEntry entry in objectStateManager.FindRelationshipsByKey(sourceKey))
            {
                if (entry.IsSameAssociationSetAndRole(associationSet, sourceKey, sourceMember))
                {
                    if (targetKey == entry.Wrapper.GetOtherEntityKey(sourceKey))
                    {
                        if (list2 == null)
                        {
                            list2 = new List<ObjectStateEntry>();
                        }
                        list2.Add(entry);
                    }
                    else
                    {
                        switch (targetMember.RelationshipMultiplicity)
                        {
                            case RelationshipMultiplicity.ZeroOrOne:
                            case RelationshipMultiplicity.One:
                                switch (mergeOption)
                                {
                                    case MergeOption.AppendOnly:
                                        goto Label_00A8;

                                    case MergeOption.OverwriteChanges:
                                        goto Label_00B6;

                                    case MergeOption.PreserveChanges:
                                        goto Label_00C9;
                                }
                                break;
                        }
                    }
                }
                continue;
            Label_00A8:
                if (entry.State != EntityState.Deleted)
                {
                    flag = false;
                }
                continue;
            Label_00B6:
                if (list == null)
                {
                    list = new List<ObjectStateEntry>();
                }
                list.Add(entry);
                continue;
            Label_00C9:
                switch (entry.State)
                {
                    case EntityState.Unchanged:
                        if (list == null)
                        {
                            list = new List<ObjectStateEntry>();
                        }
                        list.Add(entry);
                        break;

                    case EntityState.Added:
                        newEntryState = EntityState.Deleted;
                        break;

                    case EntityState.Deleted:
                        newEntryState = EntityState.Deleted;
                        if (list == null)
                        {
                            list = new List<ObjectStateEntry>();
                        }
                        list.Add(entry);
                        break;
                }
            }
            if (list != null)
            {
                foreach (ObjectStateEntry entry2 in list)
                {
                    if (entry2.State != EntityState.Detached)
                    {
                        RemoveRelatedEndsAndDetachRelationship(entry2, setIsLoaded);
                    }
                }
            }
            if (list2 != null)
            {
                foreach (ObjectStateEntry entry3 in list2)
                {
                    flag = false;
                    switch (mergeOption)
                    {
                        case MergeOption.AppendOnly:
                        {
                            continue;
                        }
                        case MergeOption.OverwriteChanges:
                        {
                            if (entry3.State != EntityState.Added)
                            {
                                break;
                            }
                            entry3.AcceptChanges();
                            continue;
                        }
                        case MergeOption.PreserveChanges:
                        {
                            if (entry3.State == EntityState.Added)
                            {
                                entry3.AcceptChanges();
                            }
                            continue;
                        }
                        default:
                        {
                            continue;
                        }
                    }
                    if (entry3.State == EntityState.Deleted)
                    {
                        ObjectStateEntry objectStateEntry = objectStateManager.GetObjectStateEntry(targetKey);
                        if (objectStateEntry.State != EntityState.Deleted)
                        {
                            if (!objectStateEntry.IsKeyEntry)
                            {
                                AddEntityToCollectionOrReference(mergeOption, sourceEntity, sourceMember, (IEntityWithRelationships) objectStateEntry.Entity, targetMember, setIsLoaded, true, false);
                            }
                            entry3.RevertDelete();
                        }
                    }
                }
            }
            return !flag;
        }

        private static void UnloadReferenceRelatedEnds(ObjectStateEntry entry)
        {
            RelationshipEntry relationshipEntry = entry as RelationshipEntry;
            ObjectStateManager objectStateManager = relationshipEntry.ObjectStateManager;
            ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = relationshipEntry.Wrapper.AssociationEndMembers;
            UnloadReferenceRelatedEnds(objectStateManager, relationshipEntry, relationshipEntry.Wrapper.GetEntityKey(0), associationEndMembers[1].Name);
            UnloadReferenceRelatedEnds(objectStateManager, relationshipEntry, relationshipEntry.Wrapper.GetEntityKey(1), associationEndMembers[0].Name);
        }

        private static void UnloadReferenceRelatedEnds(ObjectStateManager cache, RelationshipEntry relationshipEntry, EntityKey sourceEntityKey, string targetRoleName)
        {
            IEntityWithRelationships entity = cache.GetObjectStateEntry(sourceEntityKey).Entity as IEntityWithRelationships;
            if (entity != null)
            {
                EntityReference relatedEnd = entity.RelationshipManager.GetRelatedEnd(((AssociationSet) relationshipEntry.EntitySet).ElementType.FullName, targetRoleName) as EntityReference;
                if (relatedEnd != null)
                {
                    relatedEnd.SetIsLoaded(false);
                }
            }
        }

        private static void UpdateRelatedEnd(RelatedEnd relatedEnd, IEntityWithRelationships relatedEntity, bool setIsLoaded, MergeOption mergeOption)
        {
            AssociationEndMember toEndMember = (AssociationEndMember) relatedEnd.ToEndMember;
            if ((toEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One) || (toEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne))
            {
                if (setIsLoaded)
                {
                    relatedEnd.SetIsLoaded(true);
                }
                if (mergeOption == MergeOption.NoTracking)
                {
                    IEntityWithKey key = relatedEntity as IEntityWithKey;
                    if (key != null)
                    {
                        EntityKey entityKey = key.EntityKey;
                        EntityUtil.CheckEntityKeyNull(entityKey);
                        ((EntityReference) relatedEnd).DetachedEntityKey = entityKey;
                    }
                }
            }
        }

        internal static int UpdateRelationships(ObjectContext context, MergeOption mergeOption, AssociationSet associationSet, AssociationEndMember sourceMember, EntityKey sourceKey, IEntityWithRelationships sourceEntity, AssociationEndMember targetMember, IList targetEntities, bool setIsLoaded)
        {
            int num = 0;
            if (targetEntities != null)
            {
                if (mergeOption == MergeOption.NoTracking)
                {
                    RelatedEnd relatedEnd = (RelatedEnd) sourceEntity.RelationshipManager.GetRelatedEnd(sourceMember.DeclaringType.FullName, targetMember.Name);
                    if (!relatedEnd.IsEmpty())
                    {
                        throw EntityUtil.CannotFillTryDifferentMergeOption(relatedEnd.SourceRoleName, relatedEnd.RelationshipName);
                    }
                }
                foreach (IEntityWithRelationships relationships in targetEntities)
                {
                    EntityState state;
                    num++;
                    if (mergeOption == MergeOption.NoTracking)
                    {
                        AddEntityToCollectionOrReference(MergeOption.NoTracking, sourceEntity, sourceMember, relationships, targetMember, setIsLoaded, true, false);
                        continue;
                    }
                    ObjectStateManager objectStateManager = context.ObjectStateManager;
                    EntityKey entityKey = objectStateManager.GetEntityKey(relationships);
                    if (!TryUpdateExistingRelationships(context, mergeOption, associationSet, sourceMember, sourceKey, sourceEntity, targetMember, entityKey, setIsLoaded, out state))
                    {
                        bool flag = true;
                        switch (sourceMember.RelationshipMultiplicity)
                        {
                            case RelationshipMultiplicity.ZeroOrOne:
                            case RelationshipMultiplicity.One:
                                flag = !TryUpdateExistingRelationships(context, mergeOption, associationSet, targetMember, entityKey, relationships, sourceMember, sourceKey, setIsLoaded, out state);
                                break;
                        }
                        if (flag)
                        {
                            if (state != EntityState.Deleted)
                            {
                                AddEntityToCollectionOrReference(mergeOption, sourceEntity, sourceMember, relationships, targetMember, setIsLoaded, false, false);
                            }
                            else
                            {
                                RelationshipWrapper wrapper = new RelationshipWrapper(associationSet, sourceMember.Name, sourceKey, targetMember.Name, entityKey);
                                objectStateManager.AddNewRelation(wrapper, EntityState.Deleted);
                            }
                        }
                    }
                }
            }
            return num;
        }

        [Conditional("DEBUG")]
        private void ValidateKeylessEntityStore()
        {
            if (this._keylessEntityStore != null)
            {
                foreach (ObjectStateEntry entry in this._keylessEntityStore.Values)
                {
                    ObjectStateEntry entry2;
                    bool flag = false;
                    if (this._addedEntityStore != null)
                    {
                        flag = this._addedEntityStore.TryGetValue(entry.EntityKey, out entry2);
                    }
                    if (this._modifiedEntityStore != null)
                    {
                        flag |= this._modifiedEntityStore.TryGetValue(entry.EntityKey, out entry2);
                    }
                    if (this._deletedEntityStore != null)
                    {
                        flag |= this._deletedEntityStore.TryGetValue(entry.EntityKey, out entry2);
                    }
                    if (this._unchangedEntityStore != null)
                    {
                        flag |= this._unchangedEntityStore.TryGetValue(entry.EntityKey, out entry2);
                    }
                }
            }
            Dictionary<EntityKey, ObjectStateEntry>[] dictionaryArray = new Dictionary<EntityKey, ObjectStateEntry>[] { this._unchangedEntityStore, this._modifiedEntityStore, this._addedEntityStore, this._deletedEntityStore };
            foreach (Dictionary<EntityKey, ObjectStateEntry> dictionary in dictionaryArray)
            {
                if (dictionary != null)
                {
                    foreach (ObjectStateEntry entry3 in dictionary.Values)
                    {
                        if ((entry3.Entity != null) && !(entry3.Entity is IEntityWithKey))
                        {
                            ObjectStateEntry entry4;
                            this._keylessEntityStore.TryGetValue(entry3.Entity, out entry4);
                        }
                    }
                }
            }
        }

        internal string ChangingEntityMember
        {
            get => 
                this._changingEntityMember;
            set
            {
                this._changingEntityMember = value;
            }
        }

        internal string ChangingMember
        {
            get => 
                this._changingMember;
            set
            {
                this._changingMember = value;
            }
        }

        internal object ChangingObject
        {
            get => 
                this._changingObject;
            set
            {
                this._changingObject = value;
            }
        }

        internal object ChangingOldValue
        {
            get => 
                this._changingOldValue;
            set
            {
                this._changingOldValue = value;
            }
        }

        internal EntityState ChangingState
        {
            get => 
                this._changingState;
            set
            {
                this._changingState = value;
            }
        }

        internal System.Data.Objects.Internal.ComplexTypeMaterializer ComplexTypeMaterializer
        {
            get
            {
                if (this._complexTypeMaterializer == null)
                {
                    this._complexTypeMaterializer = new System.Data.Objects.Internal.ComplexTypeMaterializer(this.MetadataWorkspace);
                }
                return this._complexTypeMaterializer;
            }
        }

        internal bool InRelationshipFixup =>
            this._inRelationshipFixup;

        internal bool IsAttachTracking =>
            (this._promotedRelationships != null);

        public System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this._metadataWorkspace;

        internal Dictionary<object, ObjectStateEntry> PromotedKeyEntries =>
            this._promotedKeyEntries;

        internal Dictionary<RelatedEnd, IList<object>> PromotedRelationships =>
            this._promotedRelationships;

        internal bool SaveOriginalValues
        {
            get => 
                this._saveOriginalValues;
            set
            {
                this._saveOriginalValues = value;
            }
        }

    }
}

