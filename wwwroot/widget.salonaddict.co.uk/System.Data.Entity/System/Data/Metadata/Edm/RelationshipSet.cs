namespace System.Data.Metadata.Edm
{
    using System;

    public abstract class RelationshipSet : EntitySetBase
    {
        internal RelationshipSet(string name, string schema, string table, string definingQuery, RelationshipType relationshipType) : base(name, schema, table, definingQuery, relationshipType)
        {
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipSet;

        public RelationshipType ElementType =>
            ((RelationshipType) base.ElementType);
    }
}

