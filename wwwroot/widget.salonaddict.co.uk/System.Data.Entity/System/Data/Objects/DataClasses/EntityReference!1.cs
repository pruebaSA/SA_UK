namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable, DataContract]
    public sealed class EntityReference<TEntity> : EntityReference where TEntity: class, IEntityWithRelationships
    {
        private TEntity _cachedValue;

        public EntityReference()
        {
        }

        internal EntityReference(IEntityWithRelationships owner, RelationshipNavigation navigation, IRelationshipFixer relationshipFixer) : base(owner, navigation, relationshipFixer)
        {
            this._cachedValue = default(TEntity);
        }

        internal override void AddEntityToLocallyCachedCollection(IEntityWithRelationships entity, bool applyConstraints)
        {
            if (applyConstraints && (this._cachedValue != null))
            {
                throw EntityUtil.CannotAddMoreThanOneEntityToEntityReference();
            }
            this.ClearCollectionOrRef(null, null, false);
            this._cachedValue = (TEntity) entity;
        }

        public void Attach(TEntity entity)
        {
            base.CheckOwnerNull();
            ((IRelatedEnd) this).Attach(entity);
        }

        internal override void ClearCollectionOrRef(IEntityWithRelationships entity, RelationshipNavigation navigation, bool doCascadeDelete)
        {
            if (this._cachedValue != null)
            {
                if ((entity == this._cachedValue) && navigation.Equals(base.RelationshipNavigation))
                {
                    base.Remove(this._cachedValue, false, false, false, false);
                }
                else
                {
                    base.Remove(this._cachedValue, true, doCascadeDelete, false, true);
                }
            }
            else if (((base.Owner != null) && (base.Owner.RelationshipManager.Context != null)) && !base.UsingNoTracking)
            {
                base.Owner.RelationshipManager.Context.ObjectStateManager.GetObjectStateEntry(base.Owner).DeleteRelationshipsThatReferenceKeys(base.RelationshipSet, base.ToEndMember);
            }
            if (base.Owner != null)
            {
                base.DetachedEntityKey = null;
            }
        }

        internal override bool ContainsEntity(IEntityWithRelationships entity) => 
            ((this._cachedValue != null) && (this._cachedValue == (entity as TEntity)));

        public ObjectQuery<TEntity> CreateSourceQuery()
        {
            base.CheckOwnerNull();
            return base.CreateSourceQuery<TEntity>(base.DefaultMergeOption);
        }

        internal override IEnumerable CreateSourceQueryInternal() => 
            this.CreateSourceQuery();

        internal override void DisconnectedAdd(IEntityWithRelationships entity)
        {
            base.CheckOwnerNull();
        }

        internal override bool DisconnectedRemove(IEntityWithRelationships entity)
        {
            base.CheckOwnerNull();
            return false;
        }

        internal override void Exclude(HashSet<EntityReference> promotedEntityKeyRefs)
        {
            if (this._cachedValue != null)
            {
                if (promotedEntityKeyRefs.Contains(this))
                {
                    ObjectStateEntry entry = base.FindRelationshipEntryInObjectStateManager(this._cachedValue);
                    base.Remove(this._cachedValue, true, false, false, false);
                    if (entry.State != EntityState.Detached)
                    {
                        entry.AcceptChanges();
                    }
                    promotedEntityKeyRefs.Remove(this);
                }
                else
                {
                    base.ExcludeEntity<TEntity>(this._cachedValue, promotedEntityKeyRefs);
                }
            }
            else if (base.DetachedEntityKey != null)
            {
                this.ExcludeEntityKey();
            }
        }

        private void ExcludeEntityKey()
        {
            EntityKey key = ObjectContext.FindEntityKey(base.Owner, base.ObjectContext);
            ObjectStateEntry entry = base.ObjectContext.ObjectStateManager.FindRelationship(base.RelationshipSet, new KeyValuePair<string, EntityKey>(base.RelationshipNavigation.From, key), new KeyValuePair<string, EntityKey>(base.RelationshipNavigation.To, base.DetachedEntityKey));
            if (entry != null)
            {
                entry.Delete(false);
                if (entry.State != EntityState.Detached)
                {
                    entry.AcceptChanges();
                }
            }
        }

        internal override IEnumerator GetInternalEnumerator()
        {
            if (this.Value == null)
            {
                yield break;
            }
            yield return this.Value;
        }

        internal override void Include(bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            if (this._cachedValue != null)
            {
                base.IncludeEntity<TEntity>(this._cachedValue, addRelationshipAsUnchanged, doAttach, promotedEntityKeyRefs);
            }
            else if (base.DetachedEntityKey != null)
            {
                this.IncludeEntityKey(doAttach, promotedEntityKeyRefs);
            }
        }

        private void IncludeEntityKey(bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            ObjectStateManager objectStateManager = base.ObjectContext.ObjectStateManager;
            bool flag = false;
            bool flag2 = false;
            ObjectStateEntry entry = objectStateManager.FindObjectStateEntry(base.DetachedEntityKey);
            if (entry == null)
            {
                flag2 = true;
                flag = true;
            }
            else if (entry.IsKeyEntry)
            {
                if (base.FromEndProperty.RelationshipMultiplicity != RelationshipMultiplicity.Many)
                {
                    foreach (ObjectStateEntry entry2 in base.ObjectContext.ObjectStateManager.FindRelationshipsByKey(base.DetachedEntityKey))
                    {
                        if (entry2.IsSameAssociationSetAndRole((AssociationSet) base.RelationshipSet, base.DetachedEntityKey, (AssociationEndMember) base.ToEndMember) && (entry2.State != EntityState.Deleted))
                        {
                            throw EntityUtil.EntityConflictsWithKeyEntry();
                        }
                    }
                }
                flag = true;
            }
            else
            {
                IEntityWithRelationships entity = (IEntityWithRelationships) entry.Entity;
                if (entry.State == EntityState.Deleted)
                {
                    throw EntityUtil.ObjectStateManagerDoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(EntityState.Deleted);
                }
                RelatedEnd relatedEnd = (RelatedEnd) entity.RelationshipManager.GetRelatedEnd(base.RelationshipName, base.RelationshipNavigation.From);
                if ((base.FromEndProperty.RelationshipMultiplicity != RelationshipMultiplicity.Many) && !relatedEnd.IsEmpty())
                {
                    throw EntityUtil.EntityConflictsWithKeyEntry();
                }
                base.Add(entity, true, doAttach, false);
                promotedEntityKeyRefs.Add(this);
            }
            if (flag)
            {
                if (flag2)
                {
                    EntitySet entitySet = base.DetachedEntityKey.GetEntitySet(base.ObjectContext.MetadataWorkspace);
                    objectStateManager.AddKeyEntry(base.DetachedEntityKey, entitySet);
                }
                EntityKey key = ObjectContext.FindEntityKey(base.Owner, base.ObjectContext);
                RelationshipWrapper wrapper = new RelationshipWrapper((AssociationSet) base.RelationshipSet, base.RelationshipNavigation.From, key, base.RelationshipNavigation.To, base.DetachedEntityKey);
                objectStateManager.AddNewRelation(wrapper, doAttach ? EntityState.Unchanged : EntityState.Added);
            }
        }

        internal void InitializeWithValue(IRelatedEnd relatedEnd)
        {
            EntityReference<TEntity> reference = relatedEnd as EntityReference<TEntity>;
            if ((reference != null) && (reference._cachedValue != null))
            {
                this._cachedValue = reference._cachedValue;
            }
        }

        internal override bool IsEmpty() => 
            (this._cachedValue == null);

        public override void Load(MergeOption mergeOption)
        {
            base.CheckOwnerNull();
            ObjectQuery<TEntity> query = base.ValidateLoad<TEntity>(mergeOption, "EntityReference");
            base._suppressEvents = true;
            try
            {
                List<TEntity> collection = new List<TEntity>(RelatedEnd.GetResults<TEntity>(query));
                if (collection.Count > 1)
                {
                    throw EntityUtil.MoreThanExpectedRelatedEntitiesFound();
                }
                if (collection.Count == 0)
                {
                    if (base.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                    {
                        throw EntityUtil.LessThanExpectedRelatedEntitiesFound();
                    }
                    if ((mergeOption == MergeOption.OverwriteChanges) || (mergeOption == MergeOption.PreserveChanges))
                    {
                        EntityKey entityKey = ObjectStateManager.FindKeyOnEntityWithRelationships(base.Owner);
                        EntityUtil.CheckEntityKeyNull(entityKey);
                        ObjectStateManager.RemoveRelationships(base.ObjectContext, mergeOption, (AssociationSet) base.RelationshipSet, entityKey, (AssociationEndMember) base.FromEndProperty);
                    }
                    base._isLoaded = true;
                }
                else
                {
                    base.Merge<TEntity>(collection, mergeOption, true);
                }
            }
            finally
            {
                base._suppressEvents = false;
            }
            this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
        }

        internal override void OnRelatedEndClear()
        {
            base._isLoaded = false;
        }

        internal override bool RemoveEntityFromLocallyCachedCollection(IEntityWithRelationships entity, bool resetIsLoaded)
        {
            if ((this._cachedValue != null) && (entity != this._cachedValue))
            {
                throw EntityUtil.EntityIsNotPartOfRelationship();
            }
            this._cachedValue = default(TEntity);
            if (resetIsLoaded)
            {
                base._isLoaded = false;
            }
            return true;
        }

        internal override void RetrieveReferentialConstraintProperties(Dictionary<string, KeyValuePair<object, IntBox>> properties, HashSet<object> visited)
        {
            if (this._cachedValue != null)
            {
                foreach (ReferentialConstraint constraint in ((AssociationType) base.RelationMetadata).ReferentialConstraints)
                {
                    if (constraint.ToRole == base.FromEndProperty)
                    {
                        Dictionary<string, KeyValuePair<object, IntBox>> dictionary;
                        this._cachedValue.RelationshipManager.RetrieveReferentialConstraintProperties(out dictionary, true, visited);
                        for (int i = 0; i < constraint.FromProperties.Count; i++)
                        {
                            KeyValuePair<object, IntBox> pair = dictionary[constraint.FromProperties[i].Name];
                            ObjectStateEntry.AddOrIncreaseCounter(properties, constraint.ToProperties[i].Name, pair.Key);
                        }
                    }
                }
            }
        }

        internal override bool VerifyEntityForAdd(IEntityWithRelationships entity, bool relationshipAlreadyExists)
        {
            if (!relationshipAlreadyExists && this.ContainsEntity(entity))
            {
                return false;
            }
            if (!(entity is TEntity))
            {
                throw EntityUtil.InvalidContainedTypeReference(entity.GetType().FullName, typeof(TEntity).FullName);
            }
            return true;
        }

        internal override object CachedValue =>
            this._cachedValue;

        internal override object ReferenceValue
        {
            get => 
                this.Value;
            set
            {
                this.Value = (TEntity) value;
            }
        }

        [SoapIgnore, XmlIgnore]
        public TEntity Value
        {
            get
            {
                base.CheckOwnerNull();
                return this._cachedValue;
            }
            set
            {
                base.CheckOwnerNull();
                if ((value == null) || (value != this._cachedValue))
                {
                    base.ValidateOwnerWithRIConstraints();
                    if (value != null)
                    {
                        base.Add(value, false);
                    }
                    else
                    {
                        if (base.UsingNoTracking)
                        {
                            if (this._cachedValue != null)
                            {
                                base.GetOtherEndOfRelationship(this._cachedValue).OnRelatedEndClear();
                            }
                            base._isLoaded = false;
                        }
                        this.ClearCollectionOrRef(null, null, false);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetInternalEnumerator>d__0 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public EntityReference<TEntity> <>4__this;

            [DebuggerHidden]
            public <GetInternalEnumerator>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.<>4__this.Value == null)
                        {
                            break;
                        }
                        this.<>2__current = this.<>4__this.Value;
                        this.<>1__state = 1;
                        return true;

                    case 1:
                        this.<>1__state = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

