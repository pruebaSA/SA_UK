namespace System.Data.Metadata.Edm
{
    using System;

    public class EntitySet : EntitySetBase
    {
        internal EntitySet(string name, string schema, string table, string definingQuery, EntityType entityType) : base(name, schema, table, definingQuery, entityType)
        {
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet;

        public EntityType ElementType =>
            ((EntityType) base.ElementType);
    }
}

