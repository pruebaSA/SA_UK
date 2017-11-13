namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Runtime.Serialization;

    [Serializable, DataContract]
    public abstract class EntityReference : RelatedEnd
    {
        private System.Data.EntityKey _detachedEntityKey;

        protected EntityReference()
        {
        }

        internal EntityReference(IEntityWithRelationships owner, RelationshipNavigation navigation, IRelationshipFixer relationshipFixer) : base(owner, navigation, relationshipFixer)
        {
        }

        internal System.Data.EntityKey ValidateOwnerWithRIConstraints()
        {
            System.Data.EntityKey key = ObjectStateManager.FindKeyOnEntityWithRelationships(base.Owner);
            if (((key != null) && !key.IsTemporary) && base.IsDependentEndOfReferentialConstraint())
            {
                throw EntityUtil.CannotChangeReferentialConstraintProperty();
            }
            return key;
        }

        internal System.Data.EntityKey AttachedEntityKey =>
            this.EntityKey;

        internal abstract object CachedValue { get; }

        internal System.Data.EntityKey DetachedEntityKey
        {
            get => 
                this._detachedEntityKey;
            set
            {
                this._detachedEntityKey = value;
            }
        }

        [DataMember]
        public System.Data.EntityKey EntityKey
        {
            get
            {
                if ((base.ObjectContext == null) || base.UsingNoTracking)
                {
                    return this._detachedEntityKey;
                }
                System.Data.EntityKey entityKey = null;
                if (this.CachedValue != null)
                {
                    entityKey = base.ObjectContext.ObjectStateManager.GetEntityKey(this.CachedValue);
                    if (!RelatedEnd.IsValidEntityKeyType(entityKey))
                    {
                        entityKey = null;
                    }
                    return entityKey;
                }
                System.Data.EntityKey key = ObjectContext.FindEntityKey(base.Owner, base.ObjectContext);
                foreach (ObjectStateEntry entry in base.ObjectContext.ObjectStateManager.FindRelationshipsByKey(key))
                {
                    if ((entry.State != EntityState.Deleted) && entry.IsSameAssociationSetAndRole((AssociationSet) base.RelationshipSet, key, (AssociationEndMember) base.FromEndProperty))
                    {
                        entityKey = entry.Wrapper.GetOtherEntityKey(key);
                    }
                }
                return entityKey;
            }
            set
            {
                if ((value == null) || (value != this.EntityKey))
                {
                    if ((base.ObjectContext != null) && !base.UsingNoTracking)
                    {
                        if ((value != null) && !RelatedEnd.IsValidEntityKeyType(value))
                        {
                            throw EntityUtil.CannotSetSpecialKeys();
                        }
                        if (value == null)
                        {
                            this.ReferenceValue = null;
                        }
                        else
                        {
                            EntitySet entitySet = value.GetEntitySet(base.ObjectContext.MetadataWorkspace);
                            base.CheckRelationEntitySet(entitySet);
                            value.ValidateEntityKey(entitySet, true, "value");
                            ObjectStateManager objectStateManager = base.ObjectContext.ObjectStateManager;
                            bool flag = false;
                            bool flag2 = false;
                            ObjectStateEntry entry = objectStateManager.FindObjectStateEntry(value);
                            if (entry != null)
                            {
                                if (!entry.IsKeyEntry)
                                {
                                    this.ReferenceValue = entry.Entity;
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            else
                            {
                                flag2 = true;
                                flag = true;
                            }
                            if (flag)
                            {
                                System.Data.EntityKey key = this.ValidateOwnerWithRIConstraints();
                                base.ValidateStateForAdd(base.Owner);
                                if (flag2)
                                {
                                    objectStateManager.AddKeyEntry(value, entitySet);
                                }
                                this.ClearCollectionOrRef(null, null, false);
                                RelationshipWrapper wrapper = new RelationshipWrapper((AssociationSet) base.RelationshipSet, base.RelationshipNavigation.From, key, base.RelationshipNavigation.To, value);
                                objectStateManager.AddNewRelation(wrapper, EntityState.Added);
                            }
                        }
                    }
                    else
                    {
                        this._detachedEntityKey = value;
                    }
                }
            }
        }

        internal abstract object ReferenceValue { get; set; }
    }
}

