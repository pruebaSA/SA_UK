namespace System.Data.Metadata.Edm
{
    using System;
    using System.Threading;

    public abstract class RelationshipType : EntityTypeBase
    {
        private ReadOnlyMetadataCollection<RelationshipEndMember> _relationshipEndMembers;

        internal RelationshipType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
        }

        public ReadOnlyMetadataCollection<RelationshipEndMember> RelationshipEndMembers
        {
            get
            {
                if (this._relationshipEndMembers == null)
                {
                    FilteredReadOnlyMetadataCollection<RelationshipEndMember, EdmMember> metadatas = new FilteredReadOnlyMetadataCollection<RelationshipEndMember, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsRelationshipEndMember));
                    Interlocked.CompareExchange<ReadOnlyMetadataCollection<RelationshipEndMember>>(ref this._relationshipEndMembers, metadatas, null);
                }
                return this._relationshipEndMembers;
            }
        }
    }
}

