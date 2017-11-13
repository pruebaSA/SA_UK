namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    [Serializable]
    public sealed class EntityCollection<TEntity> : RelatedEnd, ICollection<TEntity>, IEnumerable<TEntity>, IEnumerable, IListSource where TEntity: class, IEntityWithRelationships
    {
        private HashSet<TEntity> _relatedEntities;

        [field: NonSerialized]
        internal event CollectionChangeEventHandler AssociationChangedForObjectView;

        public EntityCollection()
        {
        }

        internal EntityCollection(IEntityWithRelationships owner, RelationshipNavigation navigation, IRelationshipFixer relationshipFixer) : base(owner, navigation, relationshipFixer)
        {
        }

        public void Add(TEntity entity)
        {
            EntityUtil.CheckArgumentNull<TEntity>(entity, "entity");
            if (base.Owner != null)
            {
                ((IRelatedEnd) this).Add(entity);
            }
            else
            {
                this.DisconnectedAdd(entity);
            }
        }

        internal override void AddEntityToLocallyCachedCollection(IEntityWithRelationships entity, bool applyConstraints)
        {
            this.RelatedEntities.Add((TEntity) entity);
        }

        public void Attach(IEnumerable<TEntity> entities)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<TEntity>>(entities, "entities");
            base.CheckOwnerNull();
            base.Attach<TEntity>(entities, true);
        }

        public void Attach(TEntity entity)
        {
            ((IRelatedEnd) this).Attach(entity);
        }

        internal override void BulkDeleteAll(List<IEntityWithRelationships> list)
        {
            if (list.Count > 0)
            {
                base._suppressEvents = true;
                try
                {
                    foreach (IEntityWithRelationships relationships in list)
                    {
                        this.Remove(relationships as TEntity);
                    }
                }
                finally
                {
                    base._suppressEvents = false;
                }
                this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
            }
        }

        public void Clear()
        {
            if (base.Owner != null)
            {
                bool flag = this.Count > 0;
                if (this._relatedEntities != null)
                {
                    List<TEntity> list = new List<TEntity>(this._relatedEntities);
                    try
                    {
                        base._suppressEvents = true;
                        foreach (TEntity local in list)
                        {
                            this.Remove(local);
                            if (base.UsingNoTracking)
                            {
                                base.GetOtherEndOfRelationship(local).OnRelatedEndClear();
                            }
                        }
                    }
                    finally
                    {
                        base._suppressEvents = false;
                    }
                    if (base.UsingNoTracking)
                    {
                        base._isLoaded = false;
                    }
                }
                if (flag)
                {
                    this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
                }
            }
            else if (this._relatedEntities != null)
            {
                this._relatedEntities.Clear();
            }
        }

        internal override void ClearCollectionOrRef(IEntityWithRelationships entity, RelationshipNavigation navigation, bool doCascadeDelete)
        {
            if (this._relatedEntities != null)
            {
                List<TEntity> list = new List<TEntity>(this._relatedEntities);
                foreach (TEntity local in list)
                {
                    if ((entity == local) && navigation.Equals(base.RelationshipNavigation))
                    {
                        base.Remove(local, false, false, false, false);
                    }
                    else
                    {
                        base.Remove(local, true, doCascadeDelete, false, false);
                    }
                }
            }
        }

        public bool Contains(TEntity entity) => 
            ((this._relatedEntities != null) && this._relatedEntities.Contains(entity));

        internal override bool ContainsEntity(IEntityWithRelationships entity) => 
            this.Contains(entity as TEntity);

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            this.RelatedEntities.CopyTo(array, arrayIndex);
        }

        public ObjectQuery<TEntity> CreateSourceQuery()
        {
            base.CheckOwnerNull();
            return base.CreateSourceQuery<TEntity>(base.DefaultMergeOption);
        }

        internal override IEnumerable CreateSourceQueryInternal() => 
            this.CreateSourceQuery();

        internal override void DisconnectedAdd(IEntityWithRelationships entity)
        {
            if ((entity.RelationshipManager.Context != null) && (entity.RelationshipManager.MergeOption != MergeOption.NoTracking))
            {
                throw EntityUtil.UnableToAddToDisconnectedRelatedEnd();
            }
            this.AddEntityToLocallyCachedCollection(entity, false);
        }

        internal override bool DisconnectedRemove(IEntityWithRelationships entity)
        {
            if ((entity.RelationshipManager.Context != null) && (entity.RelationshipManager.MergeOption != MergeOption.NoTracking))
            {
                throw EntityUtil.UnableToRemoveFromDisconnectedRelatedEnd();
            }
            return this.RemoveEntityFromLocallyCachedCollection(entity, false);
        }

        internal override void Exclude(HashSet<EntityReference> promotedEntityKeyRefs)
        {
            if ((this._relatedEntities != null) && (base.ObjectContext != null))
            {
                foreach (TEntity local in this._relatedEntities)
                {
                    base.ExcludeEntity<TEntity>(local, promotedEntityKeyRefs);
                }
            }
        }

        public IEnumerator<TEntity> GetEnumerator() => 
            this.RelatedEntities.GetEnumerator();

        internal override IEnumerator GetInternalEnumerator() => 
            this.RelatedEntities.GetEnumerator();

        internal override void Include(bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            if ((this._relatedEntities != null) && (base.ObjectContext != null))
            {
                foreach (TEntity local in this._relatedEntities)
                {
                    base.IncludeEntity<TEntity>(local, addRelationshipAsUnchanged, doAttach, promotedEntityKeyRefs);
                }
            }
        }

        internal override bool IsEmpty()
        {
            if (this._relatedEntities != null)
            {
                return (this._relatedEntities.Count == 0);
            }
            return true;
        }

        public override void Load(MergeOption mergeOption)
        {
            base.CheckOwnerNull();
            this.Load(null, mergeOption);
        }

        internal void Load(IEnumerable<TEntity> collection, MergeOption mergeOption)
        {
            ObjectQuery<TEntity> query = base.ValidateLoad<TEntity>(mergeOption, "EntityCollection");
            base._suppressEvents = true;
            try
            {
                IEnumerable<TEntity> enumerable = collection ?? RelatedEnd.GetResults<TEntity>(query);
                base.Merge<TEntity>(enumerable, mergeOption, true);
            }
            finally
            {
                base._suppressEvents = false;
            }
            this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
        }

        internal override void OnAssociationChanged(CollectionChangeAction collectionChangeAction, object entity)
        {
            if (!base._suppressEvents)
            {
                if (this._onAssociationChangedforObjectView != null)
                {
                    this._onAssociationChangedforObjectView(this, new CollectionChangeEventArgs(collectionChangeAction, entity));
                }
                if (base._onAssociationChanged != null)
                {
                    base._onAssociationChanged(this, new CollectionChangeEventArgs(collectionChangeAction, entity));
                }
            }
        }

        internal override void OnRelatedEndClear()
        {
            base._isLoaded = false;
        }

        public bool Remove(TEntity entity)
        {
            EntityUtil.CheckArgumentNull<TEntity>(entity, "entity");
            return ((IRelatedEnd) this).Remove(entity);
        }

        internal override bool RemoveEntityFromLocallyCachedCollection(IEntityWithRelationships entity, bool resetIsLoaded)
        {
            if ((this._relatedEntities == null) || !this._relatedEntities.Remove((TEntity) entity))
            {
                return false;
            }
            if (resetIsLoaded)
            {
                base._isLoaded = false;
            }
            return true;
        }

        internal override void RetrieveReferentialConstraintProperties(Dictionary<string, KeyValuePair<object, IntBox>> properties, HashSet<object> visited)
        {
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.RelatedEntities.GetEnumerator();

        IList IListSource.GetList()
        {
            base.CheckOwnerNull();
            EntitySet entitySet = null;
            EntityType entityType = null;
            if (base.RelationshipSet != null)
            {
                entitySet = ((AssociationSet) base.RelationshipSet).AssociationSetEnds[base.ToEndMember.Name].EntitySet;
                EntityType elementType = (EntityType) ((RefType) ((AssociationEndMember) base.ToEndMember).TypeUsage.EdmType).ElementType;
                EntityType otherType = entitySet.ElementType;
                if (elementType.IsAssignableFrom(otherType))
                {
                    entityType = otherType;
                }
                else
                {
                    entityType = elementType;
                }
            }
            return ObjectViewFactory.CreateViewForEntityCollection<TEntity>(entityType, (EntityCollection<TEntity>) this);
        }

        internal override bool VerifyEntityForAdd(IEntityWithRelationships entity, bool relationshipAlreadyExists)
        {
            if (!relationshipAlreadyExists && this.ContainsEntity(entity))
            {
                return false;
            }
            if (!(entity is TEntity))
            {
                throw EntityUtil.InvalidContainedTypeCollection(entity.GetType().FullName, typeof(TEntity).FullName);
            }
            return true;
        }

        public int Count =>
            this._relatedEntities?.Count;

        public bool IsReadOnly =>
            false;

        private HashSet<TEntity> RelatedEntities
        {
            get
            {
                if (this._relatedEntities == null)
                {
                    this._relatedEntities = new HashSet<TEntity>();
                }
                return this._relatedEntities;
            }
        }

        bool IListSource.ContainsListCollection =>
            false;
    }
}

