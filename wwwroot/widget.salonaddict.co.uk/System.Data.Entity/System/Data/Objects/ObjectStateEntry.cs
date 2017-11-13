namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public abstract class ObjectStateEntry : IEntityStateEntry, IEntityChangeTracker
    {
        private System.Data.Objects.ObjectStateManager _cache;
        private StateManagerTypeMetadata _cacheTypeMetadata;
        private object _entity;
        private System.Data.EntityKey _entityKey;
        private EntitySetBase _entitySet;
        private BitArray _modifiedFields;
        private List<StateManagerValue> _originalValues;
        private EntityState _state;
        private RelationshipWrapper _wrapper;

        internal ObjectStateEntry(System.Data.Objects.ObjectStateManager cache, EntityState state, RelationshipWrapper wrapper)
        {
            this._cache = cache;
            this._entitySet = wrapper.AssociationSet;
            this._wrapper = wrapper;
            this._state = state;
        }

        internal ObjectStateEntry(System.Data.EntityKey entityKey, System.Data.Metadata.Edm.EntitySet extent, System.Data.Objects.ObjectStateManager cache, StateManagerTypeMetadata typeMetadata)
        {
            this._cache = cache;
            this._entityKey = entityKey;
            this._entitySet = extent;
            this._cacheTypeMetadata = typeMetadata;
            this.State = EntityState.Unchanged;
        }

        internal ObjectStateEntry(object entity, System.Data.EntityKey entityKey, System.Data.Metadata.Edm.EntitySet extent, System.Data.Objects.ObjectStateManager cache, StateManagerTypeMetadata typeMetadata, bool isAdded)
        {
            this._cache = cache;
            this._entity = entity;
            this._entitySet = extent;
            this._cacheTypeMetadata = typeMetadata;
            this._entityKey = entityKey;
            this.State = isAdded ? EntityState.Added : EntityState.Unchanged;
        }

        public void AcceptChanges()
        {
            this.ValidateState();
            switch (this.State)
            {
                case EntityState.Unchanged:
                case (EntityState.Unchanged | EntityState.Detached):
                    return;

                case EntityState.Added:
                    if (this.IsRelationship)
                    {
                        this._cache.ChangeState(this, EntityState.Added, EntityState.Unchanged);
                        break;
                    }
                    this.RetrieveAndCheckReferentialConstraintValues();
                    this._cache.FixupKey((EntityEntry) this);
                    break;

                case EntityState.Deleted:
                    this.CascadeAcceptChanges();
                    if (this._cache != null)
                    {
                        this._cache.ChangeState(this, EntityState.Deleted, EntityState.Detached);
                    }
                    return;

                case EntityState.Modified:
                    this._cache.ChangeState(this, EntityState.Modified, EntityState.Unchanged);
                    this._modifiedFields = null;
                    this._originalValues = null;
                    this.State = EntityState.Unchanged;
                    return;

                default:
                    return;
            }
            this._modifiedFields = null;
            this._originalValues = null;
            this.State = EntityState.Unchanged;
        }

        private void AddOriginalValue(StateManagerMemberMetadata memberMetadata, object userObject, object value)
        {
            if (this._originalValues == null)
            {
                this._originalValues = new List<StateManagerValue>();
            }
            this._originalValues.Add(new StateManagerValue(memberMetadata, userObject, value));
        }

        internal static void AddOrIncreaseCounter(Dictionary<string, KeyValuePair<object, IntBox>> properties, string propertyName, object propertyValue)
        {
            if (properties.ContainsKey(propertyName))
            {
                KeyValuePair<object, IntBox> pair = properties[propertyName];
                if (!pair.Key.Equals(propertyValue))
                {
                    throw EntityUtil.InconsistentReferentialConstraintProperties();
                }
                pair.Value.Value++;
            }
            else
            {
                properties[propertyName] = new KeyValuePair<object, IntBox>(propertyValue, new IntBox(1));
            }
        }

        internal void AttachObjectStateManagerToEntity()
        {
            EntityUtil.SetChangeTrackerOntoEntity(this._entity, this);
        }

        private void CascadeAcceptChanges()
        {
            if (!this.IsRelationship)
            {
                RelationshipEntry[] entryArray = this._cache.CopyOfRelationshipsByKey(this.EntityKey);
                for (int i = 0; i < entryArray.Length; i++)
                {
                    entryArray[i].AcceptChanges();
                }
            }
            else
            {
                this.DeleteUnnecessaryKeyEntries();
            }
        }

        private void CheckReferentialConstraintPropertiesInDependents()
        {
            foreach (ObjectStateEntry entry in this._cache.FindRelationshipsByKey(this.EntityKey))
            {
                ObjectStateEntry otherEndOfRelationship = this.GetOtherEndOfRelationship(entry);
                if ((otherEndOfRelationship.State == EntityState.Unchanged) || (otherEndOfRelationship.State == EntityState.Modified))
                {
                    AssociationSet entitySet = (AssociationSet) entry.EntitySet;
                    foreach (ReferentialConstraint constraint in entitySet.ElementType.ReferentialConstraints)
                    {
                        string name = this.GetAssociationEndMember(entry).Name;
                        if (constraint.FromRole.Name == name)
                        {
                            foreach (EntityKeyMember member in otherEndOfRelationship.EntityKey.EntityKeyValues)
                            {
                                for (int i = 0; i < constraint.FromProperties.Count; i++)
                                {
                                    if ((constraint.ToProperties[i].Name == member.Key) && !this.GetCurrentEntityValue(constraint.FromProperties[i].Name).Equals(member.Value))
                                    {
                                        throw EntityUtil.InconsistentReferentialConstraintProperties();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void CompareKeyProperties(object changed)
        {
            StateManagerTypeMetadata metadata = this._cacheTypeMetadata;
            int fieldCount = this.GetFieldCount(metadata);
            for (int i = 0; i < fieldCount; i++)
            {
                StateManagerMemberMetadata metadata2 = metadata.Member(i);
                if (metadata2.IsPartOfKey)
                {
                    object objA = metadata2.GetValue(changed);
                    object objB = metadata2.GetValue(this._entity);
                    if (!object.Equals(objA, objB))
                    {
                        throw EntityUtil.CannotModifyKeyProperty(metadata2.CLayerName);
                    }
                }
            }
        }

        internal void DegradeEntry()
        {
            this._entityKey = this.EntityKey;
            EntityUtil.SetChangeTrackerOntoEntity(this._entity, null);
            this._modifiedFields = null;
            this._originalValues = null;
            if (this.State == EntityState.Added)
            {
                EntityUtil.SetKeyOntoEntity(this._entity, null);
                this._entityKey = null;
            }
            if (this.State != EntityState.Unchanged)
            {
                this._cache.ChangeState(this, this.State, EntityState.Unchanged);
                this.State = EntityState.Unchanged;
            }
            this._cache.RemoveEntryFromKeylessStore(this._entity);
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(this._entity);
            if (relationshipManager != null)
            {
                relationshipManager.DetachContext();
            }
            object obj2 = this._entity;
            this._entity = null;
            this._cache.OnObjectStateManagerChanged(CollectionChangeAction.Remove, obj2);
        }

        public void Delete()
        {
            this.Delete(true);
        }

        internal void Delete(bool doFixup)
        {
            this.ValidateState();
            if (this.IsKeyEntry)
            {
                throw EntityUtil.CannotCallDeleteOnKeyEntry();
            }
            if (this.IsRelationship && doFixup)
            {
                if (this.State != EntityState.Deleted)
                {
                    ObjectStateEntry objectStateEntry = this._cache.GetObjectStateEntry((System.Data.EntityKey) this.GetCurrentRelationValue(0));
                    IEntityWithRelationships entity = objectStateEntry.Entity as IEntityWithRelationships;
                    ObjectStateEntry entry2 = this._cache.GetObjectStateEntry((System.Data.EntityKey) this.GetCurrentRelationValue(1));
                    IEntityWithRelationships relationships2 = entry2.Entity as IEntityWithRelationships;
                    if ((entity != null) && (relationships2 != null))
                    {
                        ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = this._wrapper.AssociationEndMembers;
                        string name = associationEndMembers[0].Name;
                        string to = associationEndMembers[1].Name;
                        RelationshipNavigation navigation = new RelationshipNavigation(((AssociationSet) this._entitySet).ElementType.FullName, name, to);
                        entity.RelationshipManager.RemoveEntity(navigation, relationships2);
                    }
                    else
                    {
                        System.Data.EntityKey entityKey = null;
                        RelationshipManager relationshipManager = null;
                        if (entity == null)
                        {
                            entityKey = objectStateEntry.EntityKey;
                            relationshipManager = EntityUtil.GetRelationshipManager(relationships2);
                        }
                        else
                        {
                            entityKey = entry2.EntityKey;
                            relationshipManager = EntityUtil.GetRelationshipManager(entity);
                        }
                        AssociationEndMember associationEndMember = this.Wrapper.GetAssociationEndMember(entityKey);
                        EntityReference relatedEnd = (EntityReference) relationshipManager.GetRelatedEnd(associationEndMember.DeclaringType.FullName, associationEndMember.Name);
                        relatedEnd.DetachedEntityKey = null;
                        if (this.State == EntityState.Added)
                        {
                            this.DeleteUnnecessaryKeyEntries();
                            this.DetachRelationshipEntry();
                        }
                        else
                        {
                            this._cache.ChangeState(this, this.State, EntityState.Deleted);
                            this.State = EntityState.Deleted;
                        }
                    }
                }
            }
            else
            {
                switch (this.State)
                {
                    case EntityState.Unchanged:
                        if (!doFixup)
                        {
                            if (!this.IsRelationship)
                            {
                                this.DeleteRelationshipsThatReferenceKeys(null, null);
                            }
                            break;
                        }
                        this.FixupRelationships();
                        break;

                    case (EntityState.Unchanged | EntityState.Detached):
                        return;

                    case EntityState.Added:
                        if (doFixup)
                        {
                            this.FixupRelationships();
                        }
                        if (this.State != EntityState.Detached)
                        {
                            if (this.IsRelationship)
                            {
                                this.DeleteUnnecessaryKeyEntries();
                                this.DetachRelationshipEntry();
                                return;
                            }
                            this._cache.ChangeState(this, EntityState.Added, EntityState.Detached);
                        }
                        return;

                    case EntityState.Modified:
                        if (doFixup)
                        {
                            this.FixupRelationships();
                        }
                        if (this.State != EntityState.Deleted)
                        {
                            this._cache.ChangeState(this, EntityState.Modified, EntityState.Deleted);
                            this.State = EntityState.Deleted;
                        }
                        return;

                    default:
                        return;
                }
                if (this.State != EntityState.Deleted)
                {
                    this._cache.ChangeState(this, EntityState.Unchanged, EntityState.Deleted);
                    this.State = EntityState.Deleted;
                }
            }
        }

        internal void DeleteRelationshipsThatReferenceKeys(RelationshipSet relationshipSet, RelationshipEndMember endMember)
        {
            if (this.State != EntityState.Detached)
            {
                RelationshipEntry[] entryArray = this._cache.CopyOfRelationshipsByKey(this.EntityKey);
                for (int i = 0; i < entryArray.Length; i++)
                {
                    ObjectStateEntry relationshipEntry = entryArray[i];
                    if ((relationshipEntry.State != EntityState.Deleted) && ((relationshipSet == null) || (relationshipSet == relationshipEntry.EntitySet)))
                    {
                        ObjectStateEntry otherEndOfRelationship = this.GetOtherEndOfRelationship(relationshipEntry);
                        if ((endMember == null) || (endMember == otherEndOfRelationship.GetAssociationEndMember(relationshipEntry)))
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                System.Data.EntityKey currentRelationValue = relationshipEntry.GetCurrentRelationValue(j, false) as System.Data.EntityKey;
                                if ((currentRelationValue != null) && this._cache.GetObjectStateEntry(currentRelationValue).IsKeyEntry)
                                {
                                    relationshipEntry.Delete(false);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DeleteUnnecessaryKeyEntries()
        {
            for (int i = 0; i < 2; i++)
            {
                System.Data.EntityKey currentRelationValue = this.GetCurrentRelationValue(i, false) as System.Data.EntityKey;
                ObjectStateEntry objectStateEntry = this._cache.GetObjectStateEntry(currentRelationValue);
                if (!objectStateEntry.IsKeyEntry)
                {
                    continue;
                }
                bool flag = false;
                using (EntityEntry.RelationshipEndEnumerator enumerator = this._cache.FindRelationshipsByKey(currentRelationValue).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current != this)
                        {
                            flag = true;
                            goto Label_006D;
                        }
                    }
                }
            Label_006D:
                if (!flag)
                {
                    this._cache.DeleteKeyEntry(objectStateEntry);
                    return;
                }
            }
        }

        internal void Detach()
        {
            this.ValidateState();
            bool flag = false;
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(this._entity);
            if (relationshipManager != null)
            {
                flag = (this.State != EntityState.Added) && this.IsOneEndOfSomeRelationship();
                relationshipManager.DetachEntityFromRelationships(this.State);
                this.DetachRelationshipsEntries(relationshipManager);
                if (flag)
                {
                    this.DegradeEntry();
                }
            }
            if (!flag)
            {
                System.Data.EntityKey entityKey = this._entityKey;
                EntityState state = this.State;
                object entity = this._entity;
                this._cache.ChangeState(this, this.State, EntityState.Detached);
                if (state != EntityState.Added)
                {
                    EntityUtil.SetKeyOntoEntity(entity, entityKey);
                }
            }
        }

        internal void DetachObjectStateManagerFromEntity()
        {
            if (!this.IsRelationship && !this.IsKeyEntry)
            {
                EntityUtil.SetChangeTrackerOntoEntity(this._entity, null);
                RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(this._entity);
                if (relationshipManager != null)
                {
                    relationshipManager.DetachContext();
                }
                EntityUtil.SetKeyOntoEntity(this._entity, null);
            }
        }

        internal void DetachRelationshipEntry()
        {
            if (this._cache != null)
            {
                this._cache.ChangeState(this, this.State, EntityState.Detached);
            }
        }

        private void DetachRelationshipsEntries(RelationshipManager relationshipManager)
        {
            RelationshipEntry[] entryArray = this._cache.CopyOfRelationshipsByKey(this.EntityKey);
            for (int i = 0; i < entryArray.Length; i++)
            {
                ObjectStateEntry entry = entryArray[i];
                System.Data.EntityKey otherEntityKey = entry.Wrapper.GetOtherEntityKey(this.EntityKey);
                if (this._cache.GetObjectStateEntry(otherEntityKey).IsKeyEntry)
                {
                    if (entry.State != EntityState.Deleted)
                    {
                        AssociationEndMember associationEndMember = entry.Wrapper.GetAssociationEndMember(otherEntityKey);
                        EntityReference relatedEnd = (EntityReference) relationshipManager.GetRelatedEnd(associationEndMember.DeclaringType.FullName, associationEndMember.Name);
                        relatedEnd.DetachedEntityKey = otherEntityKey;
                    }
                    entry.DeleteUnnecessaryKeyEntries();
                    entry.DetachRelationshipEntry();
                }
            }
        }

        private void EntityValueChanged(string entityMemberName, object complexObject, string complexObjectMemberName)
        {
            try
            {
                string str;
                StateManagerTypeMetadata metadata;
                object obj2;
                int ordinal = this.GetAndValidateChangeMemberInfo(entityMemberName, complexObject, complexObjectMemberName, out metadata, out str, out obj2);
                if (ordinal != -2)
                {
                    if (((obj2 != this._cache.ChangingObject) || (str != this._cache.ChangingMember)) || (entityMemberName != this._cache.ChangingEntityMember))
                    {
                        throw EntityUtil.EntityValueChangedWithoutEntityValueChanging();
                    }
                    if (this.State != this._cache.ChangingState)
                    {
                        throw EntityUtil.ChangedInDifferentStateFromChanging(this.State, this._cache.ChangingState);
                    }
                    if (this._cache.SaveOriginalValues)
                    {
                        StateManagerMemberMetadata memberMetadata = metadata.Member(ordinal);
                        object changingOldValue = this._cache.ChangingOldValue;
                        if (memberMetadata.IsComplex && (changingOldValue != null))
                        {
                            object newComplexObject = memberMetadata.GetValue(obj2);
                            this.ExpandComplexTypeAndAddValues(memberMetadata, changingOldValue, newComplexObject, false);
                        }
                        else
                        {
                            this.AddOriginalValue(memberMetadata, obj2, changingOldValue);
                        }
                    }
                    EntityState oldState = this.State;
                    if (this.State != EntityState.Added)
                    {
                        this.State = EntityState.Modified;
                    }
                    if (this.State == EntityState.Modified)
                    {
                        this.SetModifiedProperty(entityMemberName);
                    }
                    if (oldState != this._state)
                    {
                        this._cache.ChangeState(this, oldState, this._state);
                    }
                }
            }
            finally
            {
                this.SetCachedChangingValues(null, null, null, EntityState.Detached, null);
            }
        }

        private void EntityValueChanging(string entityMemberName, object complexObject, string complexObjectMemberName)
        {
            string str;
            StateManagerTypeMetadata metadata;
            object obj2;
            int ordinal = this.GetAndValidateChangeMemberInfo(entityMemberName, complexObject, complexObjectMemberName, out metadata, out str, out obj2);
            if (ordinal != -2)
            {
                StateManagerMemberMetadata metadata2 = metadata.Member(ordinal);
                object oldValue = null;
                if ((this._state == EntityState.Unchanged) || ((this._state == EntityState.Modified) && !this.FindOriginalValue(metadata2, obj2)))
                {
                    oldValue = metadata2.GetValue(obj2);
                    this._cache.SaveOriginalValues = true;
                }
                else
                {
                    this._cache.SaveOriginalValues = false;
                }
                this.SetCachedChangingValues(entityMemberName, obj2, str, this.State, oldValue);
            }
        }

        private void ExpandComplexTypeAndAddValues(StateManagerMemberMetadata memberMetadata, object oldComplexObject, object newComplexObject, bool useOldComplexObject)
        {
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(memberMetadata.CdmMetadata.TypeUsage.EdmType);
            for (int i = 0; i < orAddStateManagerTypeMetadata.FieldCount; i++)
            {
                object obj2;
                StateManagerMemberMetadata metadata = orAddStateManagerTypeMetadata.Member(i);
                if (metadata.IsComplex)
                {
                    object obj3 = null;
                    if (oldComplexObject != null)
                    {
                        obj3 = metadata.GetValue(oldComplexObject);
                        if ((obj3 == null) && this.FindOriginalValue(metadata, oldComplexObject, out obj2))
                        {
                            this._originalValues.Remove((StateManagerValue) obj2);
                        }
                    }
                    this.ExpandComplexTypeAndAddValues(metadata, obj3, metadata.GetValue(newComplexObject), useOldComplexObject);
                }
                else
                {
                    object originalValue = null;
                    object userObject = newComplexObject;
                    if (useOldComplexObject)
                    {
                        originalValue = metadata.GetValue(newComplexObject);
                        userObject = oldComplexObject;
                    }
                    else if (oldComplexObject != null)
                    {
                        originalValue = metadata.GetValue(oldComplexObject);
                        if (this.FindOriginalValue(metadata, oldComplexObject, out obj2))
                        {
                            StateManagerValue item = (StateManagerValue) obj2;
                            this._originalValues.Remove(item);
                            originalValue = item.originalValue;
                        }
                    }
                    else
                    {
                        originalValue = metadata.GetValue(newComplexObject);
                    }
                    this.AddOriginalValue(metadata, userObject, originalValue);
                }
            }
        }

        private bool FindOriginalValue(StateManagerMemberMetadata metadata, object instance)
        {
            object obj2;
            return this.FindOriginalValue(metadata, instance, out obj2);
        }

        private bool FindOriginalValue(StateManagerMemberMetadata metadata, object instance, out object value)
        {
            bool flag = false;
            object obj2 = null;
            if (this._originalValues != null)
            {
                foreach (StateManagerValue value2 in this._originalValues)
                {
                    if ((value2.userObject == instance) && (value2.memberMetadata == metadata))
                    {
                        flag = true;
                        obj2 = value2;
                        break;
                    }
                }
            }
            value = obj2;
            return flag;
        }

        private void FixupRelationships()
        {
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(this._entity);
            if (relationshipManager != null)
            {
                relationshipManager.RemoveEntityFromRelationships();
                this.DeleteRelationshipsThatReferenceKeys(null, null);
            }
        }

        private int GetAndValidateChangeMemberInfo(string entityMemberName, object complexObject, string complexObjectMemberName, out StateManagerTypeMetadata typeMetadata, out string changingMemberName, out object changingObject)
        {
            StateManagerTypeMetadata orAddStateManagerTypeMetadata;
            string str;
            object entity;
            typeMetadata = null;
            changingMemberName = null;
            changingObject = null;
            EntityUtil.CheckArgumentNull<string>(entityMemberName, "entityMemberName");
            this.ValidateState();
            int ordinalforOLayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(entityMemberName);
            if (ordinalforOLayerMemberName == -1)
            {
                if (entityMemberName != StructuralObject.EntityKeyPropertyName)
                {
                    throw EntityUtil.ChangeOnUnmappedProperty(entityMemberName);
                }
                if (!this._cache.InRelationshipFixup)
                {
                    throw EntityUtil.CantSetEntityKey();
                }
                this.SetCachedChangingValues(null, null, null, this.State, null);
                return -2;
            }
            if (complexObject != null)
            {
                if (!this._cacheTypeMetadata.Member(ordinalforOLayerMemberName).IsComplex)
                {
                    throw EntityUtil.ComplexChangeRequestedOnScalarProperty(entityMemberName);
                }
                orAddStateManagerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(complexObject.GetType(), (System.Data.Metadata.Edm.EntitySet) this.EntitySet);
                ordinalforOLayerMemberName = orAddStateManagerTypeMetadata.GetOrdinalforOLayerMemberName(complexObjectMemberName);
                if (ordinalforOLayerMemberName == -1)
                {
                    throw EntityUtil.ChangeOnUnmappedComplexProperty(complexObjectMemberName);
                }
                str = complexObjectMemberName;
                entity = complexObject;
            }
            else
            {
                orAddStateManagerTypeMetadata = this._cacheTypeMetadata;
                str = entityMemberName;
                entity = this.Entity;
            }
            this.VerifyEntityValueIsEditable(orAddStateManagerTypeMetadata, ordinalforOLayerMemberName, str);
            typeMetadata = orAddStateManagerTypeMetadata;
            changingMemberName = str;
            changingObject = entity;
            return ordinalforOLayerMemberName;
        }

        internal AssociationEndMember GetAssociationEndMember(ObjectStateEntry relationshipEntry)
        {
            this.ValidateState();
            return relationshipEntry.Wrapper.GetAssociationEndMember(this.EntityKey);
        }

        internal string GetCLayerName(int ordinal, StateManagerTypeMetadata metadata)
        {
            if (!this.IsRelationship)
            {
                return metadata.CLayerMemberName(ordinal);
            }
            ValidateRelationshipRange(ordinal);
            return this._wrapper.AssociationEndMembers[ordinal].Name;
        }

        internal object GetCurrentEntityValue(string memberName)
        {
            int ordinalforOLayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(memberName);
            return this.GetCurrentEntityValue(this._cacheTypeMetadata, ordinalforOLayerMemberName, this._entity, ObjectStateValueRecord.CurrentUpdatable);
        }

        internal object GetCurrentEntityValue(StateManagerTypeMetadata metadata, int ordinal, object userObject, ObjectStateValueRecord updatableRecord)
        {
            this.ValidateState();
            object obj2 = null;
            StateManagerMemberMetadata metadata2 = metadata.Member(ordinal);
            if (!metadata.IsMemberPartofShadowState(ordinal))
            {
                obj2 = metadata2.GetValue(userObject);
                if (metadata2.IsComplex && (obj2 != null))
                {
                    switch (updatableRecord)
                    {
                        case ObjectStateValueRecord.OriginalReadonly:
                            obj2 = new ObjectStateEntryDbDataRecord(this, this._cache.GetOrAddStateManagerTypeMetadata(metadata2.CdmMetadata.TypeUsage.EdmType), obj2);
                            break;

                        case ObjectStateValueRecord.CurrentUpdatable:
                            obj2 = new ObjectStateEntryDbUpdatableDataRecord(this, this._cache.GetOrAddStateManagerTypeMetadata(metadata2.CdmMetadata.TypeUsage.EdmType), obj2);
                            break;

                        case ObjectStateValueRecord.OriginalUpdatable:
                            obj2 = new ObjectStateEntryOriginalDbUpdatableDataRecord(this, this._cache.GetOrAddStateManagerTypeMetadata(metadata2.CdmMetadata.TypeUsage.EdmType), obj2);
                            break;
                    }
                }
            }
            return (obj2 ?? DBNull.Value);
        }

        internal object GetCurrentRelationValue(int ordinal) => 
            this.GetCurrentRelationValue(ordinal, true);

        private object GetCurrentRelationValue(int ordinal, bool throwException)
        {
            ValidateRelationshipRange(ordinal);
            this.ValidateState();
            if ((this.State == EntityState.Deleted) && throwException)
            {
                throw EntityUtil.CurrentValuesDoesNotExist();
            }
            return this._wrapper.GetEntityKey(ordinal);
        }

        internal DataRecordInfo GetDataRecordInfo(StateManagerTypeMetadata metadata, object userObject)
        {
            if (this.IsRelationship)
            {
                return new DataRecordInfo(TypeUsage.Create(((RelationshipSet) this.EntitySet).ElementType));
            }
            if (Helper.IsEntityType(metadata.CdmMetadata.EdmType) && (this._entityKey != null))
            {
                return new EntityRecordInfo(metadata.DataRecordInfo, this._entityKey, (System.Data.Metadata.Edm.EntitySet) this.EntitySet);
            }
            return metadata.DataRecordInfo;
        }

        internal int GetFieldCount(StateManagerTypeMetadata metadata)
        {
            if (!this.IsRelationship)
            {
                return metadata.FieldCount;
            }
            return this._wrapper.AssociationEndMembers.Count;
        }

        internal Type GetFieldType(int ordinal, StateManagerTypeMetadata metadata)
        {
            if (!this.IsRelationship)
            {
                return metadata.GetFieldType(ordinal);
            }
            return typeof(System.Data.EntityKey);
        }

        public IEnumerable<string> GetModifiedProperties()
        {
            this.ValidateState();
            if ((EntityState.Modified == this._state) && (this._modifiedFields != null))
            {
                for (int i = 0; i < this._modifiedFields.Count; i++)
                {
                    if (this._modifiedFields[i])
                    {
                        yield return (this.IsRelationship ? this.GetCLayerName(i, null) : this.GetCLayerName(i, this._cacheTypeMetadata));
                    }
                }
            }
        }

        internal int GetOrdinalforCLayerName(string name, StateManagerTypeMetadata metadata)
        {
            AssociationEndMember member;
            if (!this.IsRelationship)
            {
                return metadata.GetOrdinalforCLayerMemberName(name);
            }
            ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = this._wrapper.AssociationEndMembers;
            if (associationEndMembers.TryGetValue(name, false, out member))
            {
                return associationEndMembers.IndexOf(member);
            }
            return -1;
        }

        internal object GetOriginalEntityValue(StateManagerTypeMetadata metadata, int ordinal, object userObject, ObjectStateValueRecord updatableRecord)
        {
            object obj2;
            this.ValidateState();
            StateManagerMemberMetadata metadata2 = metadata.Member(ordinal);
            if (!this.FindOriginalValue(metadata2, userObject, out obj2))
            {
                return this.GetCurrentEntityValue(metadata, ordinal, userObject, updatableRecord);
            }
            return (((StateManagerValue) obj2).originalValue ?? DBNull.Value);
        }

        internal object GetOriginalRelationValue(int ordinal) => 
            this.GetCurrentRelationValue(ordinal, false);

        internal ObjectStateEntry GetOtherEndOfRelationship(ObjectStateEntry relationshipEntry) => 
            this._cache.GetObjectStateEntry(relationshipEntry.Wrapper.GetOtherEntityKey(this.EntityKey));

        internal void GetOtherKeyProperties(Dictionary<string, KeyValuePair<object, IntBox>> properties)
        {
            EntityType edmType = this._cacheTypeMetadata.DataRecordInfo.RecordType.EdmType as EntityType;
            foreach (EdmMember member in edmType.KeyMembers)
            {
                if (!properties.ContainsKey(member.Name))
                {
                    properties[member.Name] = new KeyValuePair<object, IntBox>(this.GetCurrentEntityValue(member.Name), new IntBox(1));
                }
            }
        }

        private bool IsOneEndOfSomeRelationship()
        {
            foreach (ObjectStateEntry entry in this._cache.FindRelationshipsByKey(this.EntityKey))
            {
                switch (this.GetAssociationEndMember(entry).RelationshipMultiplicity)
                {
                    case RelationshipMultiplicity.One:
                    case RelationshipMultiplicity.ZeroOrOne:
                        return true;
                }
            }
            return false;
        }

        internal bool IsSameAssociationSetAndRole(AssociationSet associationSet, System.Data.EntityKey entityKey, AssociationEndMember associationMember) => 
            (object.ReferenceEquals(this._entitySet, associationSet) && (this._wrapper.GetAssociationEndMember(entityKey).Name == associationMember.Name));

        internal void PromoteKeyEntry(object entity, IExtendedDataRecord shadowValues, StateManagerTypeMetadata typeMetadata)
        {
            this._entity = entity;
            this._cacheTypeMetadata = typeMetadata;
        }

        internal void Reset()
        {
            this.DetachObjectStateManagerFromEntity();
            this._cache = null;
            this._entity = null;
            this._entityKey = null;
            this._entitySet = null;
            this._modifiedFields = null;
            this._originalValues = null;
            this._wrapper = null;
            this.State = EntityState.Detached;
        }

        private void RetrieveAndCheckReferentialConstraintValues()
        {
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(this._entity);
            if (relationshipManager != null)
            {
                List<string> list;
                bool flag;
                relationshipManager.FindNamesOfReferentialConstraintProperties(out list, out flag);
                if (list != null)
                {
                    Dictionary<string, KeyValuePair<object, IntBox>> dictionary;
                    HashSet<object> visited = new HashSet<object>();
                    relationshipManager.RetrieveReferentialConstraintProperties(out dictionary, false, visited);
                    foreach (KeyValuePair<string, KeyValuePair<object, IntBox>> pair in dictionary)
                    {
                        this.SetCurrentEntityValue(pair.Key, pair.Value.Key);
                    }
                }
                if (flag)
                {
                    this.CheckReferentialConstraintPropertiesInDependents();
                }
            }
        }

        internal void RetrieveReferentialConstraintPropertiesFromKeyEntries(Dictionary<string, KeyValuePair<object, IntBox>> properties)
        {
            foreach (ObjectStateEntry entry in this._cache.FindRelationshipsByKey(this.EntityKey))
            {
                ObjectStateEntry otherEndOfRelationship = this.GetOtherEndOfRelationship(entry);
                if (otherEndOfRelationship.IsKeyEntry)
                {
                    AssociationSet entitySet = (AssociationSet) entry.EntitySet;
                    foreach (ReferentialConstraint constraint in entitySet.ElementType.ReferentialConstraints)
                    {
                        string name = this.GetAssociationEndMember(entry).Name;
                        if (constraint.ToRole.Name == name)
                        {
                            foreach (EntityKeyMember member in otherEndOfRelationship.EntityKey.EntityKeyValues)
                            {
                                for (int i = 0; i < constraint.FromProperties.Count; i++)
                                {
                                    if (constraint.FromProperties[i].Name == member.Key)
                                    {
                                        AddOrIncreaseCounter(properties, constraint.ToProperties[i].Name, member.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void RevertDelete()
        {
            if (!this.IsRelationship)
            {
                this.State = (this._modifiedFields == null) ? EntityState.Unchanged : EntityState.Modified;
            }
            else
            {
                this.State = EntityState.Unchanged;
            }
            this._cache.ChangeState(this, EntityState.Deleted, this.State);
        }

        private void SetCachedChangingValues(string entityMemberName, object changingObject, string changingMember, EntityState changingState, object oldValue)
        {
            this._cache.ChangingEntityMember = entityMemberName;
            this._cache.ChangingObject = changingObject;
            this._cache.ChangingMember = changingMember;
            this._cache.ChangingState = changingState;
            this._cache.ChangingOldValue = oldValue;
            if (changingState == EntityState.Detached)
            {
                this._cache.SaveOriginalValues = false;
            }
        }

        internal void SetCurrentEntityValue(string memberName, object newValue)
        {
            int ordinalforOLayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(memberName);
            this.SetCurrentEntityValue(this._cacheTypeMetadata, ordinalforOLayerMemberName, this._entity, newValue);
        }

        internal void SetCurrentEntityValue(StateManagerTypeMetadata metadata, int ordinal, object userObject, object newValue)
        {
            this.ValidateState();
            StateManagerMemberMetadata metadata2 = metadata.Member(ordinal);
            if (metadata2.IsComplex)
            {
                if (newValue == null)
                {
                    throw EntityUtil.NullableComplexTypesNotSupported(metadata2.CLayerName);
                }
                IExtendedDataRecord record = newValue as IExtendedDataRecord;
                if (record == null)
                {
                    throw EntityUtil.InvalidComplexDataRecordObject(metadata2.CLayerName);
                }
                newValue = this._cache.ComplexTypeMaterializer.CreateComplex(record, record.DataRecordInfo, null);
            }
            metadata2.SetValue(userObject, newValue);
        }

        public void SetModified()
        {
            this.ValidateState();
            if (this.IsRelationship)
            {
                throw EntityUtil.CantModifyRelationState();
            }
            if (this.IsKeyEntry)
            {
                throw EntityUtil.CannotModifyKeyEntryState();
            }
            if (EntityState.Unchanged == this.State)
            {
                this.State = EntityState.Modified;
                this._cache.ChangeState(this, EntityState.Unchanged, this.State);
            }
            else if (EntityState.Modified != this.State)
            {
                throw EntityUtil.SetModifiedStates();
            }
        }

        internal void SetModifiedAll()
        {
            this.ValidateState();
            if (this.IsRelationship)
            {
                throw EntityUtil.CantModifyRelationState();
            }
            if (this._modifiedFields == null)
            {
                this._modifiedFields = new BitArray(this.GetFieldCount(this._cacheTypeMetadata));
            }
            this._modifiedFields.SetAll(true);
        }

        public void SetModifiedProperty(string propertyName)
        {
            this.ValidateState();
            if (this.IsRelationship)
            {
                throw EntityUtil.CantModifyRelationState();
            }
            if (this.IsKeyEntry)
            {
                throw EntityUtil.CannotModifyKeyEntryState();
            }
            if (EntityState.Unchanged == this.State)
            {
                this.State = EntityState.Modified;
                this._cache.ChangeState(this, EntityState.Unchanged, this.State);
            }
            if (EntityState.Modified != this.State)
            {
                throw EntityUtil.SetModifiedStates();
            }
            if (this._modifiedFields == null)
            {
                this._modifiedFields = new BitArray(this.GetFieldCount(this._cacheTypeMetadata));
            }
            EntityUtil.CheckArgumentNull<string>(propertyName, "propertyName");
            int ordinalforOLayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(propertyName);
            if (ordinalforOLayerMemberName == -1)
            {
                throw EntityUtil.InvalidModifiedPropertyName(propertyName);
            }
            this._modifiedFields.Set(ordinalforOLayerMemberName, true);
        }

        internal void SetOriginalEntityValue(StateManagerTypeMetadata metadata, int ordinal, object userObject, object newValue)
        {
            object obj2;
            this.ValidateState();
            if (this.State == EntityState.Added)
            {
                throw EntityUtil.OriginalValuesDoesNotExist();
            }
            EntityState state = this.State;
            StateManagerMemberMetadata metadata2 = metadata.Member(ordinal);
            if (this.FindOriginalValue(metadata2, userObject, out obj2))
            {
                this._originalValues.Remove((StateManagerValue) obj2);
            }
            if (metadata2.IsComplex)
            {
                object oldComplexObject = metadata2.GetValue(userObject);
                if (oldComplexObject == null)
                {
                    throw EntityUtil.NullableComplexTypesNotSupported(metadata2.CLayerName);
                }
                IExtendedDataRecord record = newValue as IExtendedDataRecord;
                if (record != null)
                {
                    newValue = this._cache.ComplexTypeMaterializer.CreateComplex(record, record.DataRecordInfo, null);
                }
                this.ExpandComplexTypeAndAddValues(metadata2, oldComplexObject, newValue, true);
            }
            else
            {
                this.AddOriginalValue(metadata2, userObject, newValue);
            }
            if (state == EntityState.Unchanged)
            {
                this.State = EntityState.Modified;
            }
        }

        void IEntityChangeTracker.EntityComplexMemberChanged(string entityMemberName, object complexObject, string complexObjectMemberName)
        {
            EntityUtil.CheckArgumentNull<string>(complexObjectMemberName, "complexObjectMemberName");
            EntityUtil.CheckArgumentNull<object>(complexObject, "complexObject");
            this.EntityValueChanged(entityMemberName, complexObject, complexObjectMemberName);
        }

        void IEntityChangeTracker.EntityComplexMemberChanging(string entityMemberName, object complexObject, string complexObjectMemberName)
        {
            EntityUtil.CheckArgumentNull<string>(complexObjectMemberName, "complexObjectMemberName");
            EntityUtil.CheckArgumentNull<object>(complexObject, "complexObject");
            this.EntityValueChanging(entityMemberName, complexObject, complexObjectMemberName);
        }

        void IEntityChangeTracker.EntityMemberChanged(string entityMemberName)
        {
            this.EntityValueChanged(entityMemberName, null, null);
        }

        void IEntityChangeTracker.EntityMemberChanging(string entityMemberName)
        {
            this.EntityValueChanging(entityMemberName, null, null);
        }

        private static void ValidateRelationshipRange(int ordinal)
        {
            if (1 < ordinal)
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
        }

        private void ValidateState()
        {
            if (this.State == EntityState.Detached)
            {
                throw EntityUtil.ObjectStateEntryinInvalidState();
            }
        }

        internal void VerifyEntityValueIsEditable(StateManagerTypeMetadata typeMetadata, int ordinal, string memberName)
        {
            if (this._state == EntityState.Deleted)
            {
                throw EntityUtil.CantModifyDetachedDeletedEntries();
            }
            if (typeMetadata.Member(ordinal).IsPartOfKey && (this.State != EntityState.Added))
            {
                throw EntityUtil.CannotModifyKeyProperty(memberName);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public CurrentValueRecord CurrentValues
        {
            get
            {
                this.ValidateState();
                if (this._state == EntityState.Deleted)
                {
                    throw EntityUtil.CurrentValuesDoesNotExist();
                }
                if (this.IsRelationship)
                {
                    return new ObjectStateEntryDbUpdatableDataRecord(this);
                }
                if (this.IsKeyEntry)
                {
                    throw EntityUtil.CannotAccessKeyEntryValues();
                }
                return new ObjectStateEntryDbUpdatableDataRecord(this, this._cacheTypeMetadata, this._entity);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CurrentValueRecord EditableOriginalValues =>
            new ObjectStateEntryOriginalDbUpdatableDataRecord(this, this._cacheTypeMetadata, this._entity);

        public object Entity
        {
            get
            {
                this.ValidateState();
                return this._entity;
            }
        }

        public System.Data.EntityKey EntityKey
        {
            get
            {
                this.ValidateState();
                return this._entityKey;
            }
            internal set
            {
                this._entityKey = value;
            }
        }

        public EntitySetBase EntitySet
        {
            get
            {
                this.ValidateState();
                return this._entitySet;
            }
        }

        internal bool IsKeyEntry =>
            ((this._entity == null) && (null != this._entityKey));

        public bool IsRelationship
        {
            get
            {
                if (this._wrapper == null)
                {
                    this.ValidateState();
                    return false;
                }
                return true;
            }
        }

        public System.Data.Objects.ObjectStateManager ObjectStateManager
        {
            get
            {
                this.ValidateState();
                return this._cache;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DbDataRecord OriginalValues
        {
            get
            {
                this.ValidateState();
                if (this._state == EntityState.Added)
                {
                    throw EntityUtil.OriginalValuesDoesNotExist();
                }
                if (this.IsRelationship)
                {
                    return new ObjectStateEntryDbDataRecord(this);
                }
                if (this.IsKeyEntry)
                {
                    throw EntityUtil.CannotAccessKeyEntryValues();
                }
                return new ObjectStateEntryDbDataRecord(this, this._cacheTypeMetadata, this._entity);
            }
        }

        public EntityState State
        {
            get => 
                this._state;
            internal set
            {
                this._state = value;
            }
        }

        bool IEntityStateEntry.IsKeyEntry =>
            this.IsKeyEntry;

        IEntityStateManager IEntityStateEntry.StateManager =>
            this.ObjectStateManager;

        EntityState IEntityChangeTracker.EntityState =>
            this.State;

        internal RelationshipWrapper Wrapper
        {
            get => 
                this._wrapper;
            set
            {
                this._wrapper = value;
            }
        }

    }
}

