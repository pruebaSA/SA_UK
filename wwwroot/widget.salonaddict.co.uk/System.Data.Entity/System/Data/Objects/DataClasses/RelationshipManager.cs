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
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable]
    public class RelationshipManager
    {
        [NonSerialized]
        private ObjectContext _context;
        [NonSerialized]
        private System.Data.Objects.MergeOption _mergeOption;
        [NonSerialized]
        private bool _nodeVisited;
        private readonly IEntityWithRelationships _owner;
        private List<RelatedEnd> _relationships;

        private RelationshipManager(IEntityWithRelationships owner)
        {
            EntityUtil.CheckArgumentNull<IEntityWithRelationships>(owner, "owner");
            this._owner = owner;
        }

        internal void AddRelatedEntitiesToObjectStateManager(bool doAttach)
        {
            if (this._relationships != null)
            {
                HashSet<EntityReference> promotedEntityKeyRefs = new HashSet<EntityReference>();
                bool flag = true;
                try
                {
                    List<RelatedEnd> list = new List<RelatedEnd>(this._relationships);
                    foreach (RelatedEnd end in list)
                    {
                        end.Include(false, doAttach, promotedEntityKeyRefs);
                    }
                    flag = false;
                }
                finally
                {
                    if (flag)
                    {
                        ObjectStateEntry entry;
                        if (this.Context.ObjectStateManager.IsAttachTracking)
                        {
                            this.Context.ObjectStateManager.DegradePromotedRelationships();
                        }
                        this.NodeVisited = true;
                        RemoveRelatedEntitiesFromObjectStateManager(this._owner, promotedEntityKeyRefs);
                        if (this.Context.ObjectStateManager.IsAttachTracking && this.Context.ObjectStateManager.PromotedKeyEntries.TryGetValue(this._owner, out entry))
                        {
                            entry.DegradeEntry();
                        }
                        else
                        {
                            RelatedEnd.RemoveEntityFromObjectStateManager(this._owner);
                        }
                    }
                }
            }
        }

        internal void AttachContext(ObjectContext context, EntitySet entitySet, System.Data.Objects.MergeOption mergeOption)
        {
            this._context = context;
            this._mergeOption = mergeOption;
            if (this._relationships != null)
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    end.AttachContext(context, entitySet, mergeOption);
                }
            }
        }

        private static bool CheckIfAllPropertiesWereRetrieved(Dictionary<string, KeyValuePair<object, IntBox>> properties, List<string> propertiesToRetrieve)
        {
            bool flag = true;
            List<int> list = new List<int>();
            ICollection<KeyValuePair<object, IntBox>> values = properties.Values;
            foreach (KeyValuePair<object, IntBox> pair in values)
            {
                list.Add(pair.Value.Value);
            }
            foreach (string str in propertiesToRetrieve)
            {
                if (!properties.ContainsKey(str))
                {
                    flag = false;
                    break;
                }
                KeyValuePair<object, IntBox> pair2 = properties[str];
                pair2.Value.Value--;
                if (pair2.Value.Value < 0)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                foreach (KeyValuePair<object, IntBox> pair3 in values)
                {
                    if (pair3.Value.Value != 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
            {
                IEnumerator<int> enumerator = list.GetEnumerator();
                foreach (KeyValuePair<object, IntBox> pair4 in values)
                {
                    enumerator.MoveNext();
                    pair4.Value.Value = enumerator.Current;
                }
            }
            return flag;
        }

        internal void CheckReferentialConstraintProperties(EntityKey ownerKey)
        {
            List<string> list;
            bool flag;
            this.FindNamesOfReferentialConstraintProperties(out list, out flag);
            if (((list != null) || flag) && (this._relationships != null))
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    if (!end.CheckReferentialConstraintProperties(ownerKey))
                    {
                        throw EntityUtil.InconsistentReferentialConstraintProperties();
                    }
                }
            }
        }

        public static RelationshipManager Create(IEntityWithRelationships owner) => 
            new RelationshipManager(owner);

        internal RelatedEnd CreateRelatedEnd<TSourceEntity, TTargetEntity>(RelationshipNavigation navigation, RelationshipMultiplicity sourceRoleMultiplicity, RelationshipMultiplicity targetRoleMultiplicity, RelatedEnd existingRelatedEnd) where TSourceEntity: class, IEntityWithRelationships where TTargetEntity: class, IEntityWithRelationships
        {
            IRelationshipFixer relationshipFixer = new RelationshipFixer<TSourceEntity, TTargetEntity>(sourceRoleMultiplicity, targetRoleMultiplicity);
            RelatedEnd item = null;
            switch (targetRoleMultiplicity)
            {
                case RelationshipMultiplicity.ZeroOrOne:
                case RelationshipMultiplicity.One:
                    if (existingRelatedEnd == null)
                    {
                        item = new EntityReference<TTargetEntity>(this._owner, navigation, relationshipFixer);
                        break;
                    }
                    existingRelatedEnd.InitializeRelatedEnd(this._owner, navigation, relationshipFixer);
                    item = existingRelatedEnd;
                    break;

                case RelationshipMultiplicity.Many:
                    if (existingRelatedEnd == null)
                    {
                        item = new EntityCollection<TTargetEntity>(this._owner, navigation, relationshipFixer);
                        break;
                    }
                    existingRelatedEnd.InitializeRelatedEnd(this._owner, navigation, relationshipFixer);
                    item = existingRelatedEnd;
                    break;

                default:
                    throw EntityUtil.InvalidEnumerationValue(typeof(RelationshipMultiplicity), (int) targetRoleMultiplicity);
            }
            if (this._context != null)
            {
                item.AttachContext(this._context, this._mergeOption);
            }
            this.Relationships.Add(item);
            return item;
        }

        internal void DetachContext()
        {
            this._context = null;
            if (this._relationships != null)
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    end.DetachContext();
                }
            }
        }

        internal void DetachEntityFromRelationships(EntityState ownerEntityState)
        {
            if (this._relationships != null)
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    end.DetachAll(ownerEntityState);
                }
            }
        }

        internal void FindNamesOfReferentialConstraintProperties(out List<string> propertiesToRetrieve, out bool propertiesToPropagateExist)
        {
            EntityKey entityKey = ObjectStateManager.FindKeyOnEntityWithRelationships(this._owner);
            EntityUtil.CheckEntityKeyNull(entityKey);
            propertiesToRetrieve = null;
            propertiesToPropagateExist = false;
            ObjectContext context = this.Context;
            EntityUtil.CheckContextNull(context);
            EntitySet entitySet = entityKey.GetEntitySet(context.MetadataWorkspace);
            foreach (AssociationSet set2 in MetadataHelper.GetAssociationsForEntitySet(entitySet))
            {
                foreach (ReferentialConstraint constraint in set2.ElementType.ReferentialConstraints)
                {
                    if (constraint.ToRole.TypeUsage.EdmType == entitySet.ElementType.GetReferenceType())
                    {
                        propertiesToRetrieve = propertiesToRetrieve ?? new List<string>();
                        foreach (EdmProperty property in constraint.ToProperties)
                        {
                            propertiesToRetrieve.Add(property.Name);
                        }
                    }
                    if (constraint.FromRole.TypeUsage.EdmType == entitySet.ElementType.GetReferenceType())
                    {
                        propertiesToPropagateExist = true;
                    }
                }
            }
        }

        public IEnumerable<IRelatedEnd> GetAllRelatedEnds()
        {
            if (this._owner != null)
            {
                foreach (AssociationEndMember iteratorVariable0 in this.GetAllTargetEnds(this._owner.GetType()))
                {
                    yield return this.GetRelatedEnd(iteratorVariable0.DeclaringType.FullName, iteratorVariable0.Name);
                }
            }
        }

        private IEnumerable<AssociationEndMember> GetAllTargetEnds(Type entityClrType)
        {
            ObjectItemCollection itemCollection = null;
            if ((this._context != null) && (this._context.MetadataWorkspace != null))
            {
                itemCollection = (ObjectItemCollection) this._context.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
            }
            IEnumerable<AssociationType> items = null;
            if (itemCollection != null)
            {
                items = itemCollection.GetItems<AssociationType>();
            }
            else
            {
                items = ObjectItemCollection.GetAllRelationshipTypesExpensiveWay(entityClrType.Assembly);
            }
            foreach (AssociationType iteratorVariable2 in items)
            {
                RefType edmType = iteratorVariable2.AssociationEndMembers[0].TypeUsage.EdmType as RefType;
                if ((edmType != null) && edmType.ElementType.ClrType.IsAssignableFrom(entityClrType))
                {
                    yield return iteratorVariable2.AssociationEndMembers[1];
                }
                edmType = iteratorVariable2.AssociationEndMembers[1].TypeUsage.EdmType as RefType;
                if ((edmType != null) && edmType.ElementType.ClrType.IsAssignableFrom(entityClrType))
                {
                    yield return iteratorVariable2.AssociationEndMembers[0];
                }
            }
        }

        public EntityCollection<TTargetEntity> GetRelatedCollection<TTargetEntity>(string relationshipName, string targetRoleName) where TTargetEntity: class, IEntityWithRelationships
        {
            EntityCollection<TTargetEntity> relatedEnd = this.GetRelatedEnd(relationshipName, targetRoleName) as EntityCollection<TTargetEntity>;
            if (relatedEnd == null)
            {
                throw EntityUtil.ExpectedCollectionGotReference(typeof(TTargetEntity).Name, targetRoleName, relationshipName);
            }
            return relatedEnd;
        }

        private EntityCollection<TTargetEntity> GetRelatedCollection<TSourceEntity, TTargetEntity>(string relationshipName, string sourceRoleName, string targetRoleName, RelationshipMultiplicity sourceRoleMultiplicity, RelatedEnd existingRelatedEnd) where TSourceEntity: class, IEntityWithRelationships where TTargetEntity: class, IEntityWithRelationships
        {
            RelatedEnd end;
            this.TryGetCachedRelatedEnd(relationshipName, targetRoleName, out end);
            if (existingRelatedEnd == null)
            {
                if (end != null)
                {
                    return (end as EntityCollection<TTargetEntity>);
                }
                RelationshipNavigation navigation = new RelationshipNavigation(relationshipName, sourceRoleName, targetRoleName);
                return (this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(navigation, sourceRoleMultiplicity, RelationshipMultiplicity.Many, existingRelatedEnd) as EntityCollection<TTargetEntity>);
            }
            if (end != null)
            {
                this._relationships.Remove(end);
            }
            RelationshipNavigation navigation2 = new RelationshipNavigation(relationshipName, sourceRoleName, targetRoleName);
            EntityCollection<TTargetEntity> collection = this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(navigation2, sourceRoleMultiplicity, RelationshipMultiplicity.Many, existingRelatedEnd) as EntityCollection<TTargetEntity>;
            if (collection != null)
            {
                bool flag = true;
                try
                {
                    this.RemergeCollections<TTargetEntity>(end as EntityCollection<TTargetEntity>, collection);
                    flag = false;
                }
                finally
                {
                    if (flag && (end != null))
                    {
                        this._relationships.Remove(collection);
                        this._relationships.Add(end);
                    }
                }
            }
            return collection;
        }

        internal IRelatedEnd GetRelatedEnd(RelationshipNavigation navigation, IRelationshipFixer relationshipFixer)
        {
            RelatedEnd end;
            if (this.TryGetCachedRelatedEnd(navigation.RelationshipName, navigation.To, out end))
            {
                return end;
            }
            return relationshipFixer.CreateSourceEnd(navigation, this);
        }

        public IRelatedEnd GetRelatedEnd(string relationshipName, string targetRoleName)
        {
            string str;
            EntityUtil.CheckArgumentNull<string>(relationshipName, "relationshipName");
            EntityUtil.CheckArgumentNull<string>(targetRoleName, "targetRoleName");
            AssociationType relationship = this.GetRelationshipType(this._owner.GetType(), relationshipName, out str);
            return this.GetRelatedEndInternal(relationshipName, targetRoleName, null, relationship);
        }

        private IRelatedEnd GetRelatedEndInternal(string relationshipName, string targetRoleName, RelatedEnd existingRelatedEnd, AssociationType relationship) => 
            this.GetRelatedEndInternal(relationshipName, targetRoleName, existingRelatedEnd, relationship, true);

        private IRelatedEnd GetRelatedEndInternal(string relationshipName, string targetRoleName, RelatedEnd existingRelatedEnd, AssociationType relationship, bool throwOnError)
        {
            AssociationEndMember otherAssociationEnd;
            AssociationEndMember member2;
            IRelatedEnd end = null;
            if (relationship.AssociationEndMembers.TryGetValue(targetRoleName, false, out member2))
            {
                otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(member2);
            }
            else
            {
                if (throwOnError)
                {
                    throw EntityUtil.InvalidTargetRole(relationshipName, targetRoleName, "targetRoleName");
                }
                return end;
            }
            Type clrType = MetadataHelper.GetEntityTypeForEnd(otherAssociationEnd).ClrType;
            if (!clrType.IsAssignableFrom(this._owner.GetType()))
            {
                if (throwOnError)
                {
                    throw EntityUtil.OwnerIsNotSourceType(this._owner.GetType().FullName, clrType.FullName, otherAssociationEnd.Name, relationshipName);
                }
                return end;
            }
            if (this.VerifyRelationship(relationship, otherAssociationEnd.Name, throwOnError))
            {
                end = LightweightCodeGenerator.GetRelatedEnd(this, otherAssociationEnd, member2, existingRelatedEnd);
            }
            return end;
        }

        public EntityReference<TTargetEntity> GetRelatedReference<TTargetEntity>(string relationshipName, string targetRoleName) where TTargetEntity: class, IEntityWithRelationships
        {
            EntityReference<TTargetEntity> relatedEnd = this.GetRelatedEnd(relationshipName, targetRoleName) as EntityReference<TTargetEntity>;
            if (relatedEnd == null)
            {
                throw EntityUtil.ExpectedReferenceGotCollection(typeof(TTargetEntity).Name, targetRoleName, relationshipName);
            }
            return relatedEnd;
        }

        private EntityReference<TTargetEntity> GetRelatedReference<TSourceEntity, TTargetEntity>(string relationshipName, string sourceRoleName, string targetRoleName, RelationshipMultiplicity sourceRoleMultiplicity, RelatedEnd existingRelatedEnd) where TSourceEntity: class, IEntityWithRelationships where TTargetEntity: class, IEntityWithRelationships
        {
            RelatedEnd end;
            if (this.TryGetCachedRelatedEnd(relationshipName, targetRoleName, out end))
            {
                return (end as EntityReference<TTargetEntity>);
            }
            RelationshipNavigation navigation = new RelationshipNavigation(relationshipName, sourceRoleName, targetRoleName);
            return (this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(navigation, sourceRoleMultiplicity, RelationshipMultiplicity.One, existingRelatedEnd) as EntityReference<TTargetEntity>);
        }

        private AssociationType GetRelationshipType(Type entityClrType, string relationshipName, out string fullSearchName)
        {
            AssociationType associationType = null;
            if (!this.TryGetRelationshipType(entityClrType, relationshipName, out fullSearchName, out associationType))
            {
                throw EntityUtil.UnableToFindRelationshipTypeInMetadata(fullSearchName, "relationshipName");
            }
            return associationType;
        }

        [Browsable(false)]
        public void InitializeRelatedCollection<TTargetEntity>(string relationshipName, string targetRoleName, EntityCollection<TTargetEntity> entityCollection) where TTargetEntity: class, IEntityWithRelationships
        {
            string str;
            EntityUtil.CheckArgumentNull<string>(relationshipName, "relationshipName");
            EntityUtil.CheckArgumentNull<string>(targetRoleName, "targetRoleName");
            EntityUtil.CheckArgumentNull<EntityCollection<TTargetEntity>>(entityCollection, "entityCollection");
            if (entityCollection.Owner != null)
            {
                throw EntityUtil.CollectionAlreadyInitialized();
            }
            if ((this.Context != null) && (this._mergeOption != System.Data.Objects.MergeOption.NoTracking))
            {
                throw EntityUtil.CollectionRelationshipManagerAttached();
            }
            AssociationType relationship = this.GetRelationshipType(this._owner.GetType(), relationshipName, out str);
            if (!(this.GetRelatedEndInternal(relationshipName, targetRoleName, entityCollection, relationship) is EntityCollection<TTargetEntity>))
            {
                throw EntityUtil.ExpectedCollectionGotReference(typeof(TTargetEntity).Name, targetRoleName, relationshipName);
            }
        }

        [Browsable(false)]
        public void InitializeRelatedReference<TTargetEntity>(string relationshipName, string targetRoleName, EntityReference<TTargetEntity> entityReference) where TTargetEntity: class, IEntityWithRelationships
        {
            string str;
            RelatedEnd end;
            EntityUtil.CheckArgumentNull<string>(relationshipName, "relationshipName");
            EntityUtil.CheckArgumentNull<string>(targetRoleName, "targetRoleName");
            EntityUtil.CheckArgumentNull<EntityReference<TTargetEntity>>(entityReference, "entityReference");
            if (entityReference.Owner != null)
            {
                throw EntityUtil.ReferenceAlreadyInitialized();
            }
            if ((this.Context != null) && (this._mergeOption != System.Data.Objects.MergeOption.NoTracking))
            {
                throw EntityUtil.RelationshipManagerAttached();
            }
            AssociationType relationship = this.GetRelationshipType(this._owner.GetType(), relationshipName, out str);
            if (this.TryGetCachedRelatedEnd(str, targetRoleName, out end))
            {
                if (!end.IsEmpty())
                {
                    entityReference.InitializeWithValue(end);
                }
                this._relationships.Remove(end);
            }
            if (!(this.GetRelatedEndInternal(relationshipName, targetRoleName, entityReference, relationship) is EntityReference<TTargetEntity>))
            {
                throw EntityUtil.ExpectedReferenceGotCollection(typeof(TTargetEntity).Name, targetRoleName, relationshipName);
            }
        }

        internal bool IsOwner(IEntityWithRelationships entity) => 
            object.ReferenceEquals(entity, this._owner);

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            if ((this._context != null) && (this._mergeOption != System.Data.Objects.MergeOption.NoTracking))
            {
                EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(this._owner);
                foreach (ObjectStateEntry entry in this._context.ObjectStateManager.FindRelationshipsByKey(entityKey))
                {
                    if (entry.State != EntityState.Deleted)
                    {
                        EntityKey otherEntityKey = entry.Wrapper.GetOtherEntityKey(entityKey);
                        EntityKey key3 = otherEntityKey;
                        if (!RelatedEnd.IsValidEntityKeyType(otherEntityKey))
                        {
                            key3 = null;
                        }
                        AssociationEndMember associationEndMember = entry.Wrapper.GetAssociationEndMember(otherEntityKey);
                        if ((associationEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One) || (associationEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne))
                        {
                            EntityReference relatedEnd = this.GetRelatedEnd(associationEndMember.DeclaringType.FullName, associationEndMember.Name) as EntityReference;
                            relatedEnd.DetachedEntityKey = key3;
                        }
                    }
                }
            }
        }

        private void RemergeCollections<TTargetEntity>(EntityCollection<TTargetEntity> previousCollection, EntityCollection<TTargetEntity> collection) where TTargetEntity: class, IEntityWithRelationships
        {
            int num = 0;
            List<IEntityWithRelationships> list = new List<IEntityWithRelationships>(collection.Count);
            foreach (object obj2 in collection)
            {
                list.Add((IEntityWithRelationships) obj2);
            }
            foreach (IEntityWithRelationships relationships in list)
            {
                bool flag = true;
                if ((previousCollection != null) && previousCollection.ContainsEntity(relationships))
                {
                    num++;
                    flag = false;
                }
                if (flag)
                {
                    ((IRelatedEnd) collection).Remove(relationships);
                    ((IRelatedEnd) collection).Add(relationships);
                }
            }
            if ((previousCollection != null) && (num != previousCollection.Count))
            {
                throw EntityUtil.CannotRemergeCollections();
            }
        }

        internal void RemoveEntity(RelationshipNavigation navigation, IEntityWithRelationships entity)
        {
            RelatedEnd end;
            if (this.TryGetCachedRelatedEnd(navigation.RelationshipName, navigation.To, out end))
            {
                ((IRelatedEnd) end).Remove(entity);
            }
        }

        internal void RemoveEntityFromRelationships()
        {
            if (this._relationships != null)
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    end.RemoveAll();
                }
            }
        }

        internal static void RemoveRelatedEntitiesFromObjectStateManager(IEntityWithRelationships entity, HashSet<EntityReference> promotedEntityKeyRefs)
        {
            foreach (RelatedEnd end in entity.RelationshipManager.Relationships)
            {
                if (end.ObjectContext != null)
                {
                    end.Exclude(promotedEntityKeyRefs);
                    end.DetachContext();
                }
            }
        }

        internal void ResetContext(ObjectContext context, EntitySet entitySet, System.Data.Objects.MergeOption mergeOption)
        {
            if (!object.ReferenceEquals(this._context, context))
            {
                this._context = context;
                this._mergeOption = mergeOption;
                if (this._relationships != null)
                {
                    foreach (RelatedEnd end in this._relationships)
                    {
                        end.AttachContext(context, entitySet, mergeOption);
                        foreach (object obj2 in end)
                        {
                            IEntityWithRelationships relationships = obj2 as IEntityWithRelationships;
                            if (relationships != null)
                            {
                                relationships.RelationshipManager.ResetContext(context, end.GetTargetEntitySetFromRelationshipSet(), mergeOption);
                            }
                        }
                    }
                }
            }
        }

        internal void RetrieveReferentialConstraintProperties(out Dictionary<string, KeyValuePair<object, IntBox>> properties, bool includeOwnValues, HashSet<object> visited)
        {
            properties = new Dictionary<string, KeyValuePair<object, IntBox>>();
            EntityKey entityKey = this._context.ObjectStateManager.GetEntityKey(this._owner);
            if (visited.Contains(this._owner))
            {
                throw EntityUtil.CircularRelationshipsWithReferentialConstraints();
            }
            visited.Add(this._owner);
            if (entityKey.IsTemporary)
            {
                List<string> list;
                bool flag;
                this.FindNamesOfReferentialConstraintProperties(out list, out flag);
                if (list != null)
                {
                    if (this._relationships != null)
                    {
                        foreach (RelatedEnd end in this._relationships)
                        {
                            end.RetrieveReferentialConstraintProperties(properties, visited);
                        }
                    }
                    if (!CheckIfAllPropertiesWereRetrieved(properties, list))
                    {
                        this._context.ObjectStateManager.FindObjectStateEntry(entityKey).RetrieveReferentialConstraintPropertiesFromKeyEntries(properties);
                        if (!CheckIfAllPropertiesWereRetrieved(properties, list))
                        {
                            throw EntityUtil.UnableToRetrieveReferentialConstraintProperties();
                        }
                    }
                }
            }
            if (!entityKey.IsTemporary || includeOwnValues)
            {
                this._context.ObjectStateManager.FindObjectStateEntry(entityKey).GetOtherKeyProperties(properties);
            }
        }

        private bool TryGetCachedRelatedEnd(string relationshipName, string targetRoleName, out RelatedEnd relatedEnd)
        {
            relatedEnd = null;
            if (this._relationships != null)
            {
                foreach (RelatedEnd end in this._relationships)
                {
                    RelationshipNavigation relationshipNavigation = end.RelationshipNavigation;
                    if ((relationshipNavigation.RelationshipName == relationshipName) && (relationshipNavigation.To == targetRoleName))
                    {
                        relatedEnd = end;
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool TryGetRelatedEnd(string relationshipName, string targetRoleName, out IRelatedEnd relatedEnd)
        {
            string str;
            AssociationType type;
            EntityUtil.CheckArgumentNull<string>(relationshipName, "relationshipName");
            EntityUtil.CheckArgumentNull<string>(targetRoleName, "targetRoleName");
            relatedEnd = null;
            if (this.TryGetRelationshipType(this._owner.GetType(), relationshipName, out str, out type))
            {
                relatedEnd = this.GetRelatedEndInternal(relationshipName, targetRoleName, null, type, false);
                return (relatedEnd != null);
            }
            return false;
        }

        private bool TryGetRelationshipType(Type entityClrType, string relationshipName, out string fullSearchName, out AssociationType associationType)
        {
            ObjectItemCollection itemCollection = null;
            if ((this._context != null) && (this._context.MetadataWorkspace != null))
            {
                itemCollection = (ObjectItemCollection) this._context.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
            }
            if (itemCollection != null)
            {
                associationType = itemCollection.GetRelationshipType(entityClrType, relationshipName, out fullSearchName);
            }
            else
            {
                associationType = ObjectItemCollection.GetRelationshipTypeExpensiveWay(entityClrType, relationshipName, out fullSearchName);
            }
            return (associationType != null);
        }

        private bool VerifyRelationship(AssociationType relationship, string sourceEndName, bool throwOnError)
        {
            TypeUsage usage;
            EntitySet set2;
            if (this._context == null)
            {
                return true;
            }
            EntityKey key = null;
            key = ObjectContext.FindEntityKey(this._owner, this._context);
            if (key == null)
            {
                return true;
            }
            bool flag = true;
            if (!this._context.Perspective.TryGetTypeByName(relationship.FullName, false, out usage) || (MetadataHelper.GetAssociationsForEntitySetAndAssociationType(this.Context.MetadataWorkspace.GetEntityContainer(key.EntityContainerName, DataSpace.CSpace), key.EntitySetName, (AssociationType) usage.EdmType, sourceEndName, out set2) != null))
            {
                return flag;
            }
            if (throwOnError)
            {
                throw EntityUtil.NoRelationshipSetMatched(relationship.FullName);
            }
            return false;
        }

        internal ObjectContext Context =>
            this._context;

        internal System.Data.Objects.MergeOption MergeOption =>
            this._mergeOption;

        internal bool NodeVisited
        {
            get => 
                this._nodeVisited;
            set
            {
                this._nodeVisited = value;
            }
        }

        internal List<RelatedEnd> Relationships
        {
            get
            {
                if (this._relationships == null)
                {
                    this._relationships = new List<RelatedEnd>();
                }
                return this._relationships;
            }
        }



        internal delegate IRelatedEnd GetRelatedEndMethod(RelationshipManager relationshipManager, RelatedEnd existingRelatedEnd);
    }
}

