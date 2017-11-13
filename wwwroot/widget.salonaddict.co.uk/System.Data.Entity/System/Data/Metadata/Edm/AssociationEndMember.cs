namespace System.Data.Metadata.Edm
{
    using System;
    using System.Threading;

    public sealed class AssociationEndMember : RelationshipEndMember
    {
        private object _getRelatedEndMethod;

        internal AssociationEndMember(string name, RefType endRefType, RelationshipMultiplicity multiplicity) : base(name, endRefType, multiplicity)
        {
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.AssociationEndMember;

        internal object GetRelatedEnd
        {
            get => 
                this._getRelatedEndMethod;
            set
            {
                Interlocked.CompareExchange(ref this._getRelatedEndMethod, value, null);
            }
        }
    }
}

