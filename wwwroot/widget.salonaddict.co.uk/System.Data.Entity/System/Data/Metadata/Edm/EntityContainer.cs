namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices;

    public sealed class EntityContainer : GlobalItem
    {
        private readonly ReadOnlyMetadataCollection<EntitySetBase> _baseEntitySets;
        private readonly ReadOnlyMetadataCollection<EdmFunction> _functionImports;
        private readonly string _name;

        internal EntityContainer(string name, DataSpace dataSpace)
        {
            EntityUtil.CheckStringArgument(name, "name");
            this._name = name;
            base.DataSpace = dataSpace;
            this._baseEntitySets = new ReadOnlyMetadataCollection<EntitySetBase>(new EntitySetBaseCollection(this));
            this._functionImports = new ReadOnlyMetadataCollection<EdmFunction>(new MetadataCollection<EdmFunction>());
        }

        internal void AddEntitySetBase(EntitySetBase entitySetBase)
        {
            this._baseEntitySets.Source.Add(entitySetBase);
        }

        internal void AddFunctionImport(EdmFunction function)
        {
            this._functionImports.Source.Add(function);
        }

        public EntitySet GetEntitySetByName(string name, bool ignoreCase)
        {
            EntitySet set = this.BaseEntitySets.GetValue(name, ignoreCase) as EntitySet;
            if (set == null)
            {
                throw EntityUtil.InvalidEntitySetName(name);
            }
            return set;
        }

        public RelationshipSet GetRelationshipSetByName(string name, bool ignoreCase)
        {
            RelationshipSet set;
            if (!this.TryGetRelationshipSetByName(name, ignoreCase, out set))
            {
                throw EntityUtil.InvalidRelationshipSetName(name);
            }
            return set;
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.BaseEntitySets.Source.SetReadOnly();
                this.FunctionImports.Source.SetReadOnly();
            }
        }

        public override string ToString() => 
            this.Name;

        public bool TryGetEntitySetByName(string name, bool ignoreCase, out EntitySet entitySet)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            EntitySetBase item = null;
            entitySet = null;
            if (this.BaseEntitySets.TryGetValue(name, ignoreCase, out item) && Helper.IsEntitySet(item))
            {
                entitySet = (EntitySet) item;
                return true;
            }
            return false;
        }

        public bool TryGetRelationshipSetByName(string name, bool ignoreCase, out RelationshipSet relationshipSet)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            EntitySetBase item = null;
            relationshipSet = null;
            if (this.BaseEntitySets.TryGetValue(name, ignoreCase, out item) && Helper.IsRelationshipSet(item))
            {
                relationshipSet = (RelationshipSet) item;
                return true;
            }
            return false;
        }

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase, true)]
        public ReadOnlyMetadataCollection<EntitySetBase> BaseEntitySets =>
            this._baseEntitySets;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EntityContainer;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EdmFunction, true)]
        internal ReadOnlyMetadataCollection<EdmFunction> FunctionImports =>
            this._functionImports;

        internal override string Identity =>
            this.Name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;
    }
}

