namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, DataContract]
    public abstract class RelatedEnd : IRelatedEnd
    {
        [NonSerialized]
        private System.Data.Objects.ObjectContext _context;
        private const string _entityKeyParamName = "EntityKeyValue";
        [NonSerialized]
        private RelationshipEndMember _fromEndProperty;
        internal bool _isLoaded;
        private System.Data.Objects.DataClasses.RelationshipNavigation _navigation;
        [NonSerialized]
        internal CollectionChangeEventHandler _onAssociationChanged;
        private IEntityWithRelationships _owner;
        [NonSerialized]
        private RelationshipType _relationMetadata;
        private IRelationshipFixer _relationshipFixer;
        [NonSerialized]
        private System.Data.Metadata.Edm.RelationshipSet _relationshipSet;
        [NonSerialized]
        private string _sourceQuery;
        [NonSerialized]
        internal bool _suppressEvents;
        [NonSerialized]
        private RelationshipEndMember _toEndProperty;
        [NonSerialized]
        private bool _usingNoTracking;

        public event CollectionChangeEventHandler AssociationChanged
        {
            add
            {
                this.CheckOwnerNull();
                this._onAssociationChanged = (CollectionChangeEventHandler) Delegate.Combine(this._onAssociationChanged, value);
            }
            remove
            {
                this.CheckOwnerNull();
                this._onAssociationChanged = (CollectionChangeEventHandler) Delegate.Remove(this._onAssociationChanged, value);
            }
        }

        internal event CollectionChangeEventHandler AssociationChangedForObjectView
        {
            add
            {
            }
            remove
            {
            }
        }

        internal RelatedEnd()
        {
        }

        internal RelatedEnd(IEntityWithRelationships owner, System.Data.Objects.DataClasses.RelationshipNavigation navigation, IRelationshipFixer relationshipFixer)
        {
            EntityUtil.CheckArgumentNull<IEntityWithRelationships>(owner, "owner");
            EntityUtil.CheckArgumentNull<System.Data.Objects.DataClasses.RelationshipNavigation>(navigation, "navigation");
            EntityUtil.CheckArgumentNull<IRelationshipFixer>(relationshipFixer, "fixer");
            this.InitializeRelatedEnd(owner, navigation, relationshipFixer);
        }

        internal void Add(IEntityWithRelationships entity, bool applyConstraints)
        {
            EntityUtil.ValidateRelationshipManager(entity);
            EntityUtil.ValidateRelationshipManager(this._owner);
            if ((this._context != null) && !this.UsingNoTracking)
            {
                this.ValidateStateForAdd(this._owner);
                this.ValidateStateForAdd(entity);
            }
            this.Add(entity, applyConstraints, false, false);
        }

        internal void Add(IEntityWithRelationships targetEntity, bool applyConstraints, bool addRelationshipAsUnchanged, bool relationshipAlreadyExists)
        {
            if (this.VerifyEntityForAdd(targetEntity, relationshipAlreadyExists))
            {
                EntityKey key = ObjectStateManager.FindKeyOnEntityWithRelationships(targetEntity);
                if ((key != null) && (this.ObjectContext != null))
                {
                    this.CheckRelationEntitySet(key.GetEntitySet(this.ObjectContext.MetadataWorkspace));
                }
                RelatedEnd otherEndOfRelationship = this.GetOtherEndOfRelationship(targetEntity);
                if (object.ReferenceEquals(this.ObjectContext, otherEndOfRelationship.ObjectContext) && (this.ObjectContext != null))
                {
                    if (this.UsingNoTracking != otherEndOfRelationship.UsingNoTracking)
                    {
                        throw EntityUtil.CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(this.UsingNoTracking ? this._navigation.From : this._navigation.To);
                    }
                }
                else if ((this.ObjectContext != null) && (otherEndOfRelationship.ObjectContext != null))
                {
                    if (!this.UsingNoTracking || !otherEndOfRelationship.UsingNoTracking)
                    {
                        throw EntityUtil.CannotCreateRelationshipEntitiesInDifferentContexts();
                    }
                    targetEntity.RelationshipManager.ResetContext(this.ObjectContext, this.GetTargetEntitySetFromRelationshipSet(), MergeOption.NoTracking);
                }
                otherEndOfRelationship.VerifyEntityForAdd(this.Owner, relationshipAlreadyExists);
                this.AddEntityToLocallyCachedCollection(targetEntity, applyConstraints);
                otherEndOfRelationship.AddEntityToLocallyCachedCollection(this.Owner, false);
                RelatedEnd end2 = null;
                IEntityWithRelationships entity = null;
                HashSet<EntityReference> promotedEntityKeyRefs = new HashSet<EntityReference>();
                if (object.ReferenceEquals(this.ObjectContext, otherEndOfRelationship.ObjectContext) && (this.ObjectContext != null))
                {
                    if (!relationshipAlreadyExists && !this.UsingNoTracking)
                    {
                        this.AddRelationshipToObjectStateManager(targetEntity, addRelationshipAsUnchanged, false);
                    }
                }
                else if ((this.ObjectContext != null) || (otherEndOfRelationship.ObjectContext != null))
                {
                    if (this.ObjectContext == null)
                    {
                        end2 = otherEndOfRelationship;
                        entity = this.Owner;
                    }
                    else
                    {
                        end2 = this;
                        entity = targetEntity;
                    }
                    try
                    {
                        if (!end2.UsingNoTracking)
                        {
                            end2.AddGraphToObjectStateManager(entity, relationshipAlreadyExists, addRelationshipAsUnchanged, false, promotedEntityKeyRefs);
                        }
                        end2 = null;
                        entity = null;
                    }
                    finally
                    {
                        if (end2 != null)
                        {
                            end2.FixupOtherEndOfRelationshipForRemove(entity);
                            end2.RemoveEntityFromLocallyCachedCollection(entity, false);
                            RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(entity, promotedEntityKeyRefs);
                            RemoveEntityFromObjectStateManager(entity);
                        }
                    }
                }
                otherEndOfRelationship.OnAssociationChanged(CollectionChangeAction.Add, this.Owner);
                this.OnAssociationChanged(CollectionChangeAction.Add, targetEntity);
            }
        }

        internal abstract void AddEntityToLocallyCachedCollection(IEntityWithRelationships entity, bool applyConstraints);
        private void AddEntityToObjectStateManager(IEntityWithRelationships entity, bool doAttach)
        {
            EntitySet targetEntitySetFromRelationshipSet = this.GetTargetEntitySetFromRelationshipSet();
            if (!doAttach)
            {
                this._context.AddSingleObject(targetEntitySetFromRelationshipSet, entity, "entity");
            }
            else
            {
                this._context.AttachSingleObject(entity, targetEntitySetFromRelationshipSet, "entity");
            }
            EntityReference reference = this as EntityReference;
            if ((reference != null) && (reference.DetachedEntityKey != null))
            {
                EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(entity);
                if (reference.DetachedEntityKey != entityKey)
                {
                    throw EntityUtil.EntityKeyValueMismatch();
                }
            }
        }

        private void AddGraphToObjectStateManager(IEntityWithRelationships entity, bool relationshipAlreadyExists, bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            this.AddEntityToObjectStateManager(entity, doAttach);
            if (!relationshipAlreadyExists)
            {
                this.AddRelationshipToObjectStateManager(entity, addRelationshipAsUnchanged, doAttach);
            }
            WalkObjectGraphToIncludeAllRelatedEntities(entity, addRelationshipAsUnchanged, doAttach, promotedEntityKeyRefs);
        }

        private void AddRelationshipToObjectStateManager(IEntityWithRelationships entity, bool addRelationshipAsUnchanged, bool doAttach)
        {
            if ((this._context != null) && (entity.RelationshipManager.Context != null))
            {
                EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(this._owner);
                EntityKey key2 = this._context.ObjectStateManager.GetEntityKey(entity);
                this._context.ObjectStateManager.AddRelation(new RelationshipWrapper((AssociationSet) this._relationshipSet, new KeyValuePair<string, EntityKey>(this._navigation.From, entityKey), new KeyValuePair<string, EntityKey>(this._navigation.To, key2)), (addRelationshipAsUnchanged || doAttach) ? EntityState.Unchanged : EntityState.Added);
            }
        }

        protected internal void Attach<TEntity>(IEnumerable<TEntity> entities, bool allowCollection)
        {
            this.ValidateOwnerForAttach();
            int num = 0;
            List<TEntity> collection = new List<TEntity>();
            foreach (TEntity local in entities)
            {
                this.ValidateEntityForAttach<TEntity>(local, num++, allowCollection);
                collection.Add(local);
            }
            this._suppressEvents = true;
            try
            {
                this.Merge<TEntity>(collection, MergeOption.OverwriteChanges, false);
            }
            finally
            {
                this._suppressEvents = false;
            }
            this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
        }

        internal void AttachContext(System.Data.Objects.ObjectContext context, MergeOption mergeOption)
        {
            EntityUtil.CheckKeyForRelationship(this._owner, mergeOption);
            EntityKey entityKey = System.Data.Objects.ObjectContext.FindEntityKey(this._owner, context);
            EntityUtil.CheckEntityKeyNull(entityKey);
            EntitySet entitySet = entityKey.GetEntitySet(context.MetadataWorkspace);
            this.AttachContext(context, entitySet, mergeOption);
        }

        internal void AttachContext(System.Data.Objects.ObjectContext context, EntitySet entitySet, MergeOption mergeOption)
        {
            EntityUtil.CheckArgumentNull<System.Data.Objects.ObjectContext>(context, "context");
            EntityUtil.CheckArgumentMergeOption(mergeOption);
            EntityUtil.CheckArgumentNull<EntitySet>(entitySet, "entitySet");
            this._owner.RelationshipManager.NodeVisited = false;
            if ((this._context != context) || (this._usingNoTracking != (mergeOption == MergeOption.NoTracking)))
            {
                bool flag = true;
                try
                {
                    this._sourceQuery = null;
                    this._context = context;
                    this._usingNoTracking = mergeOption == MergeOption.NoTracking;
                    EdmType item = this._context.MetadataWorkspace.GetItem<EdmType>(this._navigation.RelationshipName, DataSpace.CSpace);
                    if (item == null)
                    {
                        throw EntityUtil.NoRelationshipSetMatched(this._navigation.RelationshipName);
                    }
                    System.Data.Metadata.Edm.RelationshipSet set = null;
                    foreach (EntitySetBase base2 in entitySet.EntityContainer.BaseEntitySets)
                    {
                        if ((base2.ElementType == item) && (((AssociationSet) base2).AssociationSetEnds[this._navigation.From].EntitySet == entitySet))
                        {
                            set = (System.Data.Metadata.Edm.RelationshipSet) base2;
                            break;
                        }
                    }
                    if (set != null)
                    {
                        this._relationshipSet = set;
                        this._relationMetadata = (RelationshipType) item;
                    }
                    else
                    {
                        foreach (EntitySetBase base3 in entitySet.EntityContainer.BaseEntitySets)
                        {
                            AssociationSet set2 = base3 as AssociationSet;
                            if (((set2 != null) && (set2.ElementType == item)) && ((set2.AssociationSetEnds[this._navigation.From].EntitySet != entitySet) && (set2.AssociationSetEnds[this._navigation.From].EntitySet.ElementType == entitySet.ElementType)))
                            {
                                throw EntityUtil.EntitySetIsNotValidForRelationship(entitySet.EntityContainer.Name, entitySet.Name, this._navigation.From, ((AssociationSet) base3).EntityContainer.Name, ((AssociationSet) base3).Name);
                            }
                        }
                        throw EntityUtil.NoRelationshipSetMatched(this._navigation.RelationshipName);
                    }
                    bool flag2 = false;
                    bool flag3 = false;
                    foreach (AssociationEndMember member in ((AssociationType) this._relationMetadata).AssociationEndMembers)
                    {
                        if (member.Name == this._navigation.From)
                        {
                            if (flag2)
                            {
                                throw EntityUtil.FoundMoreThanOneRelatedEnd();
                            }
                            flag2 = true;
                            this._fromEndProperty = member;
                        }
                        if (member.Name == this._navigation.To)
                        {
                            if (flag3)
                            {
                                throw EntityUtil.FoundMoreThanOneRelatedEnd();
                            }
                            flag3 = true;
                            this._toEndProperty = member;
                        }
                    }
                    if (!flag2 || !flag3)
                    {
                        throw EntityUtil.RelatedEndNotFound();
                    }
                    if (this.IsEmpty())
                    {
                        EntityReference reference = this as EntityReference;
                        if ((reference != null) && (reference.DetachedEntityKey != null))
                        {
                            EntityKey detachedEntityKey = reference.DetachedEntityKey;
                            if (!IsValidEntityKeyType(detachedEntityKey))
                            {
                                throw EntityUtil.CannotSetSpecialKeys();
                            }
                            EntitySet set3 = detachedEntityKey.GetEntitySet(context.MetadataWorkspace);
                            this.CheckRelationEntitySet(set3);
                            detachedEntityKey.ValidateEntityKey(set3);
                        }
                    }
                    flag = false;
                }
                finally
                {
                    if (flag)
                    {
                        this.DetachContext();
                    }
                }
            }
        }

        internal virtual void BulkDeleteAll(List<IEntityWithRelationships> list)
        {
            throw EntityUtil.NotSupported();
        }

        private static bool CheckCascadeDeleteFlag(RelationshipEndMember relationEndProperty) => 
            ((relationEndProperty != null) && (relationEndProperty.DeleteBehavior == OperationAction.Cascade));

        internal void CheckOwnerNull()
        {
            if (this._owner == null)
            {
                throw EntityUtil.OwnerIsNull();
            }
        }

        internal bool CheckReferentialConstraintProperties(EntityKey ownerKey)
        {
            if (!this.IsEmpty() || (((this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne) || (this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)) && (((EntityReference) this).DetachedEntityKey != null)))
            {
                foreach (ReferentialConstraint constraint in ((AssociationType) this.RelationMetadata).ReferentialConstraints)
                {
                    if (constraint.ToRole == this.FromEndProperty)
                    {
                        if (this.IsEmpty())
                        {
                            EntityKey detachedEntityKey = ((EntityReference) this).DetachedEntityKey;
                            if (!VerifyRIConstraintsWithRelatedEntry(constraint, ownerKey, detachedEntityKey))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            foreach (IEntityWithRelationships relationships in this)
                            {
                                EntitySet targetEntitySetFromRelationshipSet = this.GetTargetEntitySetFromRelationshipSet();
                                EntityKey key = System.Data.Objects.ObjectContext.FindEntityKey(relationships, this.ObjectContext);
                                if (key != null)
                                {
                                    EntityUtil.ValidateEntitySetInKey(key, targetEntitySetFromRelationshipSet);
                                    key.ValidateEntityKey(targetEntitySetFromRelationshipSet);
                                }
                                else
                                {
                                    key = this._context.CreateEntityKey(targetEntitySetFromRelationshipSet, relationships);
                                }
                                if (!VerifyRIConstraintsWithRelatedEntry(constraint, ownerKey, key))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else if ((constraint.FromRole == this.FromEndProperty) && this.IsEmpty())
                    {
                        EntityKey dependentKey = ((EntityReference) this).DetachedEntityKey;
                        if (!VerifyRIConstraintsWithRelatedEntry(constraint, dependentKey, ownerKey))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal void CheckRelationEntitySet(EntitySet set)
        {
            if ((((AssociationSet) this._relationshipSet).AssociationSetEnds[this._navigation.To] != null) && (((AssociationSet) this._relationshipSet).AssociationSetEnds[this._navigation.To].EntitySet != set))
            {
                throw EntityUtil.EntitySetIsNotValidForRelationship(set.EntityContainer.Name, set.Name, this._navigation.To, this._relationshipSet.EntityContainer.Name, this._relationshipSet.Name);
            }
        }

        internal void Clear(IEntityWithRelationships entity, System.Data.Objects.DataClasses.RelationshipNavigation navigation, bool doCascadeDelete)
        {
            this.ClearCollectionOrRef(entity, navigation, doCascadeDelete);
        }

        internal abstract void ClearCollectionOrRef(IEntityWithRelationships entity, System.Data.Objects.DataClasses.RelationshipNavigation navigation, bool doCascadeDelete);
        internal abstract bool ContainsEntity(IEntityWithRelationships entity);
        internal ObjectQuery<TEntity> CreateSourceQuery<TEntity>(MergeOption mergeOption)
        {
            EntityState detached;
            if (this._context == null)
            {
                return null;
            }
            ObjectStateEntry entry = this._context.ObjectStateManager.FindObjectStateEntry(this._owner);
            if (entry == null)
            {
                if (!this.UsingNoTracking)
                {
                    throw EntityUtil.InvalidEntityStateSource();
                }
                detached = EntityState.Detached;
            }
            else
            {
                detached = entry.State;
            }
            if (detached == EntityState.Added)
            {
                throw EntityUtil.InvalidEntityStateSource();
            }
            if (((detached != EntityState.Detached) || !this.UsingNoTracking) && (((detached != EntityState.Modified) && (detached != EntityState.Unchanged)) && (detached != EntityState.Deleted)))
            {
                return null;
            }
            EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(this._owner);
            if (this._sourceQuery == null)
            {
                EntitySet entitySet = ((AssociationSet) this._relationshipSet).AssociationSetEnds[this._toEndProperty.Name].EntitySet;
                StringBuilder builder = new StringBuilder("SELECT VALUE [TargetEntity] FROM (SELECT VALUE x FROM ");
                builder.Append("[");
                builder.Append(this._relationshipSet.EntityContainer.Name);
                builder.Append("].[");
                builder.Append(this._relationshipSet.Name);
                builder.Append("] AS x WHERE Key(x.[");
                builder.Append(this._fromEndProperty.Name);
                builder.Append("]) = ROW(");
                AliasGenerator generator = new AliasGenerator("EntityKeyValue");
                int count = entityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace).ElementType.KeyMembers.Count;
                for (int i = 0; i < count; i++)
                {
                    string str = generator.Next();
                    builder.Append("@");
                    builder.Append(str);
                    builder.Append(" AS ");
                    builder.Append(str);
                    if (i < (count - 1))
                    {
                        builder.Append(",");
                    }
                }
                builder.Append(")) AS [AssociationEntry] INNER JOIN ");
                EntityType entityTypeForEnd = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) this._toEndProperty);
                bool flag = false;
                if (!entitySet.ElementType.EdmEquals(entityTypeForEnd) && !TypeSemantics.IsSubTypeOf(entitySet.ElementType, entityTypeForEnd))
                {
                    flag = true;
                    entityTypeForEnd = (EntityType) this.ObjectContext.MetadataWorkspace.GetOSpaceTypeUsage(TypeUsage.Create(entityTypeForEnd)).EdmType;
                }
                if (flag)
                {
                    builder.Append("OfType(");
                }
                builder.Append("[");
                builder.Append(entitySet.EntityContainer.Name);
                builder.Append("].[");
                builder.Append(entitySet.Name);
                builder.Append("]");
                if (flag)
                {
                    builder.Append(", [");
                    if (entityTypeForEnd.NamespaceName != string.Empty)
                    {
                        builder.Append(entityTypeForEnd.NamespaceName);
                        builder.Append("].[");
                    }
                    builder.Append(entityTypeForEnd.Name);
                    builder.Append("])");
                }
                builder.Append(" AS [TargetEntity] ON Key([AssociationEntry].[");
                builder.Append(this._toEndProperty.Name);
                builder.Append("]) = Key(Ref([TargetEntity]))");
                this._sourceQuery = builder.ToString();
            }
            ObjectQuery<TEntity> query = new ObjectQuery<TEntity>(this._sourceQuery, this._context, mergeOption);
            AliasGenerator generator2 = new AliasGenerator("EntityKeyValue");
            ReadOnlyMetadataCollection<EdmMember>.Enumerator enumerator = entityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace).ElementType.KeyMembers.GetEnumerator();
            foreach (EntityKeyMember member in entityKey.EntityKeyValues)
            {
                enumerator.MoveNext();
                ObjectParameter parameter = new ObjectParameter(generator2.Next(), member.Value) {
                    TypeUsage = Helper.GetModelTypeUsage(enumerator.Current)
                };
                query.Parameters.Add(parameter);
            }
            query.Parameters.SetReadOnly(true);
            return query;
        }

        internal abstract IEnumerable CreateSourceQueryInternal();
        internal void DetachAll(EntityState ownerEntityState)
        {
            List<IEntityWithRelationships> list = new List<IEntityWithRelationships>();
            foreach (IEntityWithRelationships relationships in this)
            {
                list.Add(relationships);
            }
            bool flag = (ownerEntityState == EntityState.Added) || (this._fromEndProperty.RelationshipMultiplicity == RelationshipMultiplicity.Many);
            foreach (IEntityWithRelationships relationships2 in list)
            {
                if (!this.ContainsEntity(relationships2))
                {
                    return;
                }
                EntityReference reference = this as EntityReference;
                if (reference != null)
                {
                    reference.DetachedEntityKey = reference.AttachedEntityKey;
                }
                if (flag)
                {
                    DetachRelationshipFromObjectStateManager(relationships2, this._owner, this._relationshipSet, this._navigation);
                }
                RelatedEnd otherEndOfRelationship = this.GetOtherEndOfRelationship(relationships2);
                otherEndOfRelationship.RemoveEntityFromLocallyCachedCollection(this._owner, true);
                otherEndOfRelationship.OnAssociationChanged(CollectionChangeAction.Remove, this._owner);
            }
            foreach (IEntityWithRelationships relationships3 in list)
            {
                this.GetOtherEndOfRelationship(relationships3);
                this.RemoveEntityFromLocallyCachedCollection(relationships3, false);
            }
            this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
        }

        internal void DetachContext()
        {
            this._sourceQuery = null;
            this._context = null;
            this._relationshipSet = null;
            this._fromEndProperty = null;
            this._toEndProperty = null;
            this._isLoaded = false;
        }

        private static void DetachRelationshipFromObjectStateManager(IEntityWithRelationships entity, IEntityWithRelationships owner, System.Data.Metadata.Edm.RelationshipSet relationshipSet, System.Data.Objects.DataClasses.RelationshipNavigation navigation)
        {
            if (((owner.RelationshipManager.Context != null) && (entity.RelationshipManager.Context != null)) && (relationshipSet != null))
            {
                EntityKey entityKey = owner.RelationshipManager.Context.ObjectStateManager.GetEntityKey(owner);
                EntityKey key2 = entity.RelationshipManager.Context.ObjectStateManager.GetEntityKey(entity);
                ObjectStateEntry entry = entity.RelationshipManager.Context.ObjectStateManager.FindRelationship(relationshipSet, new KeyValuePair<string, EntityKey>(navigation.From, entityKey), new KeyValuePair<string, EntityKey>(navigation.To, key2));
                if (entry != null)
                {
                    entry.DetachRelationshipEntry();
                }
            }
        }

        internal abstract void DisconnectedAdd(IEntityWithRelationships entity);
        internal abstract bool DisconnectedRemove(IEntityWithRelationships entity);
        internal abstract void Exclude(HashSet<EntityReference> promotedEntityKeyRefs);
        internal void ExcludeEntity<U>(U entity, HashSet<EntityReference> promotedEntityKeyRefs) where U: class, IEntityWithRelationships
        {
            ObjectStateEntry entry = this._context.ObjectStateManager.FindObjectStateEntry(entity);
            if (((entry != null) && (entry.State != EntityState.Deleted)) && !entity.RelationshipManager.NodeVisited)
            {
                entity.RelationshipManager.NodeVisited = true;
                RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(entity, promotedEntityKeyRefs);
                RemoveRelationshipFromObjectStateManager(entity, this._owner, this._relationshipSet, this._navigation);
                RemoveEntityFromObjectStateManager(entity);
            }
            else if (this.FindRelationshipEntryInObjectStateManager(entity) != null)
            {
                RemoveRelationshipFromObjectStateManager(entity, this._owner, this._relationshipSet, this._navigation);
            }
        }

        internal ObjectStateEntry FindRelationshipEntryInObjectStateManager(IEntityWithRelationships entity)
        {
            EntityKey key = System.Data.Objects.ObjectContext.FindEntityKey(entity, this._context);
            EntityKey key2 = System.Data.Objects.ObjectContext.FindEntityKey(this._owner, this._context);
            return this._context.ObjectStateManager.FindRelationship(this._relationshipSet, new KeyValuePair<string, EntityKey>(this._navigation.From, key2), new KeyValuePair<string, EntityKey>(this._navigation.To, key));
        }

        private void FixupOtherEndOfRelationshipForRemove(IEntityWithRelationships entity)
        {
            this.GetOtherEndOfRelationship(entity).Remove(this._owner, false, false, false, false);
        }

        public IEnumerator GetEnumerator()
        {
            if (this is EntityReference)
            {
                this.CheckOwnerNull();
            }
            return this.GetInternalEnumerator();
        }

        internal abstract IEnumerator GetInternalEnumerator();
        internal RelatedEnd GetOtherEndOfRelationship(IEntityWithRelationships entity) => 
            ((RelatedEnd) entity.RelationshipManager.GetRelatedEnd(this._navigation.Reverse, this._relationshipFixer));

        internal static IEnumerable<U> GetResults<U>(ObjectQuery<U> query) => 
            query.Execute(query.MergeOption);

        internal EntitySet GetTargetEntitySetFromRelationshipSet()
        {
            AssociationSet set2 = (AssociationSet) this._relationshipSet;
            AssociationEndMember toEndMember = (AssociationEndMember) this.ToEndMember;
            return set2.AssociationSetEnds[toEndMember.Name].EntitySet;
        }

        internal abstract void Include(bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs);
        internal void IncludeEntity<U>(U entity, bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs) where U: class, IEntityWithRelationships
        {
            ObjectStateEntry entry = this._context.ObjectStateManager.FindObjectStateEntry(entity);
            if ((entry == null) || (entry.State == EntityState.Deleted))
            {
                this.AddGraphToObjectStateManager(entity, false, addRelationshipAsUnchanged, doAttach, promotedEntityKeyRefs);
            }
            else if (this.FindRelationshipEntryInObjectStateManager(entity) == null)
            {
                EntityReference reference = this as EntityReference;
                if ((reference != null) && (reference.DetachedEntityKey != null))
                {
                    EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(entity);
                    if (reference.DetachedEntityKey != entityKey)
                    {
                        throw EntityUtil.EntityKeyValueMismatch();
                    }
                }
                this.AddRelationshipToObjectStateManager(entity, addRelationshipAsUnchanged, doAttach);
            }
        }

        internal void InitializeRelatedEnd(IEntityWithRelationships owner, System.Data.Objects.DataClasses.RelationshipNavigation navigation, IRelationshipFixer relationshipFixer)
        {
            this._owner = owner;
            this._navigation = navigation;
            this._relationshipFixer = relationshipFixer;
        }

        internal bool IsDependentEndOfReferentialConstraint()
        {
            if (this._relationMetadata != null)
            {
                foreach (ReferentialConstraint constraint in ((AssociationType) this._relationMetadata).ReferentialConstraints)
                {
                    if (constraint.ToRole == this._fromEndProperty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal abstract bool IsEmpty();
        internal bool IsPrincipalEndOfReferentialConstraint()
        {
            if (this._relationMetadata != null)
            {
                foreach (ReferentialConstraint constraint in ((AssociationType) this._relationMetadata).ReferentialConstraints)
                {
                    if (constraint.FromRole == this._fromEndProperty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsValidEntityKeyType(EntityKey entityKey) => 
            ((!entityKey.IsTemporary && !object.ReferenceEquals(EntityKey.EntityNotValidKey, entityKey)) && !object.ReferenceEquals(EntityKey.NoEntitySetKey, entityKey));

        public void Load()
        {
            this.CheckOwnerNull();
            this.Load(this.DefaultMergeOption);
        }

        public abstract void Load(MergeOption mergeOption);
        private static ObjectStateEntry MarkEntityAsDeletedInObjectStateManager(IEntityWithRelationships entity)
        {
            ObjectStateEntry entry = null;
            if (entity.RelationshipManager.Context != null)
            {
                entry = entity.RelationshipManager.Context.ObjectStateManager.FindObjectStateEntry(entity);
                if (entry != null)
                {
                    entry.Delete(false);
                }
            }
            return entry;
        }

        private static ObjectStateEntry MarkRelationshipAsDeletedInObjectStateManager(IEntityWithRelationships entity, IEntityWithRelationships owner, System.Data.Metadata.Edm.RelationshipSet relationshipSet, System.Data.Objects.DataClasses.RelationshipNavigation navigation)
        {
            ObjectStateEntry entry = null;
            if (((owner.RelationshipManager.Context != null) && (entity.RelationshipManager.Context != null)) && (relationshipSet != null))
            {
                EntityKey entityKey = owner.RelationshipManager.Context.ObjectStateManager.GetEntityKey(owner);
                EntityKey key2 = entity.RelationshipManager.Context.ObjectStateManager.GetEntityKey(entity);
                entry = entity.RelationshipManager.Context.ObjectStateManager.DeleteRelationship(relationshipSet, new KeyValuePair<string, EntityKey>(navigation.From, entityKey), new KeyValuePair<string, EntityKey>(navigation.To, key2));
            }
            return entry;
        }

        protected void Merge<TEntity>(IEnumerable<TEntity> collection, MergeOption mergeOption, bool setIsLoaded)
        {
            List<TEntity> targetEntities = collection as List<TEntity>;
            if (targetEntities == null)
            {
                targetEntities = new List<TEntity>(collection);
            }
            EntityKey entityKey = ObjectStateManager.FindKeyOnEntityWithRelationships(this.Owner);
            EntityUtil.CheckEntityKeyNull(entityKey);
            ObjectStateManager.UpdateRelationships(this.ObjectContext, mergeOption, (AssociationSet) this.RelationshipSet, (AssociationEndMember) this.FromEndProperty, entityKey, this.Owner, (AssociationEndMember) this.ToEndMember, targetEntities, setIsLoaded);
            if (setIsLoaded)
            {
                this._isLoaded = true;
            }
        }

        internal virtual void OnAssociationChanged(CollectionChangeAction collectionChangeAction, object entity)
        {
            if (!this._suppressEvents && (this._onAssociationChanged != null))
            {
                this._onAssociationChanged(this, new CollectionChangeEventArgs(collectionChangeAction, entity));
            }
        }

        internal abstract void OnRelatedEndClear();
        internal void Remove(IEntityWithRelationships entity, bool doFixup, bool deleteEntity, bool deleteOwner, bool applyReferentialConstraints)
        {
            EntityUtil.ValidateRelationshipManager(entity);
            EntityUtil.ValidateRelationshipManager(this._owner);
            if (this.ContainsEntity(entity))
            {
                if (((this._context != null) && doFixup) && (applyReferentialConstraints && this.IsDependentEndOfReferentialConstraint()))
                {
                    this.GetOtherEndOfRelationship(entity).Remove(this._owner, doFixup, deleteEntity, deleteOwner, applyReferentialConstraints);
                }
                else
                {
                    bool flag = this.RemoveEntityFromLocallyCachedCollection(entity, false);
                    if (!this.UsingNoTracking)
                    {
                        MarkRelationshipAsDeletedInObjectStateManager(entity, this._owner, this._relationshipSet, this._navigation);
                    }
                    if (doFixup)
                    {
                        bool flag2 = false;
                        if ((this._context != null) && ((deleteEntity || (deleteOwner && CheckCascadeDeleteFlag(this._fromEndProperty))) || (applyReferentialConstraints && this.IsPrincipalEndOfReferentialConstraint())))
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            RemoveEntityFromRelatedEnds(entity, this._owner, this._navigation.Reverse);
                        }
                        this.FixupOtherEndOfRelationshipForRemove(entity);
                        if (flag2)
                        {
                            MarkEntityAsDeletedInObjectStateManager(entity);
                        }
                    }
                    if (flag)
                    {
                        this.OnAssociationChanged(CollectionChangeAction.Remove, entity);
                    }
                }
            }
        }

        internal void RemoveAll()
        {
            List<IEntityWithRelationships> list = null;
            bool flag = false;
            try
            {
                this._suppressEvents = true;
                foreach (IEntityWithRelationships relationships in this)
                {
                    if (list == null)
                    {
                        list = new List<IEntityWithRelationships>();
                    }
                    list.Add(relationships);
                }
                if (flag = (list != null) && (list.Count > 0))
                {
                    foreach (IEntityWithRelationships relationships2 in list)
                    {
                        this.Remove(relationships2, true, false, true, true);
                    }
                }
            }
            finally
            {
                this._suppressEvents = false;
            }
            if (flag)
            {
                this.OnAssociationChanged(CollectionChangeAction.Refresh, null);
            }
        }

        internal abstract bool RemoveEntityFromLocallyCachedCollection(IEntityWithRelationships entity, bool resetIsLoaded);
        internal static void RemoveEntityFromObjectStateManager(IEntityWithRelationships entity)
        {
            ObjectStateEntry entry;
            if (((entity.RelationshipManager.Context != null) && entity.RelationshipManager.Context.ObjectStateManager.IsAttachTracking) && entity.RelationshipManager.Context.ObjectStateManager.PromotedKeyEntries.TryGetValue(entity, out entry))
            {
                entry.DegradeEntry();
            }
            else
            {
                entry = MarkEntityAsDeletedInObjectStateManager(entity);
                if ((entry != null) && (entry.State != EntityState.Detached))
                {
                    entry.AcceptChanges();
                }
            }
        }

        private static void RemoveEntityFromRelatedEnds(IEntityWithRelationships entity1, IEntityWithRelationships entity2, System.Data.Objects.DataClasses.RelationshipNavigation navigation)
        {
            foreach (RelatedEnd end in entity1.RelationshipManager.Relationships)
            {
                bool doCascadeDelete = false;
                doCascadeDelete = CheckCascadeDeleteFlag(end.FromEndProperty) || end.IsPrincipalEndOfReferentialConstraint();
                end.Clear(entity2, navigation, doCascadeDelete);
            }
        }

        private static void RemoveRelationshipFromObjectStateManager(IEntityWithRelationships entity, IEntityWithRelationships owner, System.Data.Metadata.Edm.RelationshipSet relationshipSet, System.Data.Objects.DataClasses.RelationshipNavigation navigation)
        {
            ObjectStateEntry entry = MarkRelationshipAsDeletedInObjectStateManager(entity, owner, relationshipSet, navigation);
            if ((entry != null) && (entry.State != EntityState.Detached))
            {
                entry.AcceptChanges();
            }
        }

        internal abstract void RetrieveReferentialConstraintProperties(Dictionary<string, KeyValuePair<object, IntBox>> keyValues, HashSet<object> visited);
        internal void SetIsLoaded(bool value)
        {
            this._isLoaded = value;
        }

        void IRelatedEnd.Add(IEntityWithRelationships entity)
        {
            EntityUtil.CheckArgumentNull<IEntityWithRelationships>(entity, "entity");
            if (this._owner != null)
            {
                this.Add(entity, true);
            }
            else
            {
                this.DisconnectedAdd(entity);
            }
        }

        void IRelatedEnd.Attach(IEntityWithRelationships entity)
        {
            this.CheckOwnerNull();
            EntityUtil.CheckArgumentNull<IEntityWithRelationships>(entity, "entity");
            this.Attach<IEntityWithRelationships>(new IEntityWithRelationships[] { entity }, false);
        }

        IEnumerable IRelatedEnd.CreateSourceQuery()
        {
            this.CheckOwnerNull();
            return this.CreateSourceQueryInternal();
        }

        bool IRelatedEnd.Remove(IEntityWithRelationships entity)
        {
            EntityUtil.CheckArgumentNull<IEntityWithRelationships>(entity, "entity");
            if (this._owner == null)
            {
                return this.DisconnectedRemove(entity);
            }
            if (this.ContainsEntity(entity))
            {
                this.Remove(entity, true, false, false, true);
                return true;
            }
            return false;
        }

        protected internal void ValidateEntityForAttach<TEntity>(TEntity entity, int index, bool allowCollection)
        {
            if (entity == null)
            {
                if (allowCollection)
                {
                    throw EntityUtil.InvalidNthElementNullForAttach(index);
                }
                throw EntityUtil.ArgumentNull("entity");
            }
            ObjectStateEntry entry = this.ObjectContext.ObjectStateManager.FindObjectStateEntry(entity);
            if ((entry == null) || !object.ReferenceEquals(entry.Entity, entity))
            {
                if (allowCollection)
                {
                    throw EntityUtil.InvalidNthElementContextForAttach(index);
                }
                throw EntityUtil.InvalidEntityContextForAttach();
            }
            if ((entry.State != EntityState.Unchanged) && (entry.State != EntityState.Modified))
            {
                if (allowCollection)
                {
                    throw EntityUtil.InvalidNthElementStateForAttach(index);
                }
                throw EntityUtil.InvalidEntityStateForAttach();
            }
        }

        protected ObjectQuery<TEntity> ValidateLoad<TEntity>(MergeOption mergeOption, string relatedEndName)
        {
            ObjectQuery<TEntity> query = this.CreateSourceQuery<TEntity>(mergeOption);
            if (query == null)
            {
                throw EntityUtil.RelatedEndNotAttachedToContext(relatedEndName);
            }
            ObjectStateEntry entry = this.ObjectContext.ObjectStateManager.FindObjectStateEntry(this.Owner);
            if ((entry != null) && (entry.State == EntityState.Deleted))
            {
                throw EntityUtil.InvalidEntityStateLoad(relatedEndName);
            }
            if (this.UsingNoTracking != (mergeOption == MergeOption.NoTracking))
            {
                throw EntityUtil.MismatchedMergeOptionOnLoad(mergeOption);
            }
            if (this.UsingNoTracking)
            {
                if (this.IsLoaded)
                {
                    throw EntityUtil.LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd();
                }
                if (!this.IsEmpty())
                {
                    throw EntityUtil.LoadCalledOnNonEmptyNoTrackedRelatedEnd();
                }
            }
            return query;
        }

        protected internal void ValidateOwnerForAttach()
        {
            if ((this.ObjectContext == null) || this.UsingNoTracking)
            {
                throw EntityUtil.InvalidOwnerStateForAttach();
            }
            ObjectStateEntry objectStateEntry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(this.Owner);
            if ((objectStateEntry.State != EntityState.Modified) && (objectStateEntry.State != EntityState.Unchanged))
            {
                throw EntityUtil.InvalidOwnerStateForAttach();
            }
        }

        internal void ValidateStateForAdd(IEntityWithRelationships entity)
        {
            ObjectStateEntry entry = this.ObjectContext.ObjectStateManager.FindObjectStateEntry(entity);
            if ((entry != null) && (entry.State == EntityState.Deleted))
            {
                throw EntityUtil.ObjectStateManagerDoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(EntityState.Deleted);
            }
        }

        internal abstract bool VerifyEntityForAdd(IEntityWithRelationships entity, bool relationshipAlreadyExists);
        private static bool VerifyRIConstraintsWithRelatedEntry(ReferentialConstraint constraint, EntityKey dependentKey, EntityKey principalKey)
        {
            for (int i = 0; i < constraint.FromProperties.Count; i++)
            {
                string name = constraint.FromProperties[i].Name;
                string keyName = constraint.ToProperties[i].Name;
                object obj2 = principalKey.FindValueByName(name);
                object obj3 = dependentKey.FindValueByName(keyName);
                if (!obj2.Equals(obj3))
                {
                    return false;
                }
            }
            return true;
        }

        private static void WalkObjectGraphToIncludeAllRelatedEntities(IEntityWithRelationships entity, bool addRelationshipAsUnchanged, bool doAttach, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            foreach (RelatedEnd end in entity.RelationshipManager.Relationships)
            {
                end.Include(addRelationshipAsUnchanged, doAttach, promotedEntityKeyRefs);
            }
        }

        internal MergeOption DefaultMergeOption
        {
            get
            {
                if (!this.UsingNoTracking)
                {
                    return MergeOption.AppendOnly;
                }
                return MergeOption.NoTracking;
            }
        }

        internal RelationshipEndMember FromEndProperty =>
            this._fromEndProperty;

        [XmlIgnore, SoapIgnore]
        public bool IsLoaded
        {
            get
            {
                this.CheckOwnerNull();
                return this._isLoaded;
            }
        }

        internal System.Data.Objects.ObjectContext ObjectContext =>
            this._context;

        internal IEntityWithRelationships Owner =>
            this._owner;

        internal RelationshipType RelationMetadata =>
            this._relationMetadata;

        [SoapIgnore, XmlIgnore]
        public string RelationshipName
        {
            get
            {
                this.CheckOwnerNull();
                return this._navigation.RelationshipName;
            }
        }

        internal System.Data.Objects.DataClasses.RelationshipNavigation RelationshipNavigation =>
            this._navigation;

        [SoapIgnore, XmlIgnore]
        public System.Data.Metadata.Edm.RelationshipSet RelationshipSet
        {
            get
            {
                this.CheckOwnerNull();
                return this._relationshipSet;
            }
        }

        [XmlIgnore, SoapIgnore]
        public string SourceRoleName
        {
            get
            {
                this.CheckOwnerNull();
                return this._navigation.From;
            }
        }

        [SoapIgnore, XmlIgnore]
        public string TargetRoleName
        {
            get
            {
                this.CheckOwnerNull();
                return this._navigation.To;
            }
        }

        internal RelationshipEndMember ToEndMember =>
            this._toEndProperty;

        internal bool UsingNoTracking =>
            this._usingNoTracking;
    }
}

