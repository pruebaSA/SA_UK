namespace System.Data.Objects.DataClasses
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable, DataContract(IsReference=true)]
    public abstract class EntityObject : StructuralObject, IEntityWithKey, IEntityWithChangeTracker, IEntityWithRelationships
    {
        [NonSerialized]
        private IEntityChangeTracker _entityChangeTracker = s_detachedEntityChangeTracker;
        private System.Data.EntityKey _entityKey;
        private RelationshipManager _relationships;
        [NonSerialized]
        private static readonly DetachedEntityChangeTracker s_detachedEntityChangeTracker = new DetachedEntityChangeTracker();

        protected EntityObject()
        {
        }

        internal sealed override void ReportComplexPropertyChanged(string entityMemberName, ComplexObject complexObject, string complexMemberName)
        {
            this.EntityChangeTracker.EntityComplexMemberChanged(entityMemberName, complexObject, complexMemberName);
        }

        internal sealed override void ReportComplexPropertyChanging(string entityMemberName, ComplexObject complexObject, string complexMemberName)
        {
            this.EntityChangeTracker.EntityComplexMemberChanging(entityMemberName, complexObject, complexMemberName);
        }

        protected sealed override void ReportPropertyChanged(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            this.EntityChangeTracker.EntityMemberChanged(property);
            base.ReportPropertyChanged(property);
        }

        protected sealed override void ReportPropertyChanging(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            base.ReportPropertyChanging(property);
            this.EntityChangeTracker.EntityMemberChanging(property);
        }

        void IEntityWithChangeTracker.SetChangeTracker(IEntityChangeTracker changeTracker)
        {
            if (((changeTracker != null) && (this.EntityChangeTracker != s_detachedEntityChangeTracker)) && !object.ReferenceEquals(changeTracker, this.EntityChangeTracker))
            {
                throw EntityUtil.EntityCantHaveMultipleChangeTrackers();
            }
            this.EntityChangeTracker = changeTracker;
        }

        private IEntityChangeTracker EntityChangeTracker
        {
            get
            {
                if (this._entityChangeTracker == null)
                {
                    this._entityChangeTracker = s_detachedEntityChangeTracker;
                }
                return this._entityChangeTracker;
            }
            set
            {
                this._entityChangeTracker = value;
            }
        }

        [DataMember, Browsable(false)]
        public System.Data.EntityKey EntityKey
        {
            get => 
                this._entityKey;
            set
            {
                this.EntityChangeTracker.EntityMemberChanging(StructuralObject.EntityKeyPropertyName);
                this._entityKey = value;
                this.EntityChangeTracker.EntityMemberChanged(StructuralObject.EntityKeyPropertyName);
            }
        }

        [Browsable(false), XmlIgnore]
        public System.Data.EntityState EntityState =>
            this.EntityChangeTracker.EntityState;

        internal sealed override bool IsChangeTracked =>
            (this.EntityState != System.Data.EntityState.Detached);

        RelationshipManager IEntityWithRelationships.RelationshipManager
        {
            get
            {
                if (this._relationships == null)
                {
                    this._relationships = RelationshipManager.Create(this);
                }
                return this._relationships;
            }
        }

        private class DetachedEntityChangeTracker : IEntityChangeTracker
        {
            void IEntityChangeTracker.EntityComplexMemberChanged(string entityMemberName, object complexObject, string complexMemberName)
            {
            }

            void IEntityChangeTracker.EntityComplexMemberChanging(string entityMemberName, object complexObject, string complexMemberName)
            {
            }

            void IEntityChangeTracker.EntityMemberChanged(string entityMemberName)
            {
            }

            void IEntityChangeTracker.EntityMemberChanging(string entityMemberName)
            {
            }

            EntityState IEntityChangeTracker.EntityState =>
                EntityState.Detached;
        }
    }
}

