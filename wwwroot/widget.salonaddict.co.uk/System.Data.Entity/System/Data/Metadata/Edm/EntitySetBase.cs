namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    public abstract class EntitySetBase : MetadataItem
    {
        private string _cachedProviderSql;
        private string _definingQuery;
        private EntityTypeBase _elementType;
        private System.Data.Metadata.Edm.EntityContainer _entityContainer;
        private string _name;
        private string _schema;
        private string _table;

        internal EntitySetBase(string name, string schema, string table, string definingQuery, EntityTypeBase entityType)
        {
            EntityUtil.GenericCheckArgumentNull<EntityTypeBase>(entityType, "entityType");
            EntityUtil.CheckStringArgument(name, "name");
            this._name = name;
            this._schema = schema;
            this._table = table;
            this._definingQuery = definingQuery;
            this.ElementType = entityType;
        }

        internal void ChangeEntityContainerWithoutCollectionFixup(System.Data.Metadata.Edm.EntityContainer newEntityContainer)
        {
            this._entityContainer = newEntityContainer;
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                EntityTypeBase elementType = this.ElementType;
                if (elementType != null)
                {
                    elementType.SetReadOnly();
                }
            }
        }

        public override string ToString() => 
            this.Name;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase;

        internal string CachedProviderSql
        {
            get => 
                this._cachedProviderSql;
            set
            {
                this._cachedProviderSql = value;
            }
        }

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string DefiningQuery
        {
            get => 
                this._definingQuery;
            set
            {
                this._definingQuery = value;
            }
        }

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase, false)]
        public EntityTypeBase ElementType
        {
            get => 
                this._elementType;
            internal set
            {
                EntityUtil.GenericCheckArgumentNull<EntityTypeBase>(value, "value");
                Util.ThrowIfReadOnly(this);
                this._elementType = value;
            }
        }

        public System.Data.Metadata.Edm.EntityContainer EntityContainer =>
            this._entityContainer;

        internal override string Identity =>
            this.Name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string Schema =>
            this._schema;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string Table =>
            this._table;
    }
}

