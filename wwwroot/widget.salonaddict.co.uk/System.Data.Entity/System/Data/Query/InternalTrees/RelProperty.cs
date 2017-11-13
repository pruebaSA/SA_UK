namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class RelProperty
    {
        private readonly RelationshipEndMember m_fromEnd;
        private readonly RelationshipType m_relationshipType;
        private readonly RelationshipEndMember m_toEnd;

        internal RelProperty(RelationshipType relationshipType, RelationshipEndMember fromEnd, RelationshipEndMember toEnd)
        {
            this.m_relationshipType = relationshipType;
            this.m_fromEnd = fromEnd;
            this.m_toEnd = toEnd;
        }

        public override bool Equals(object obj)
        {
            RelProperty property = obj as RelProperty;
            return ((((property != null) && this.Relationship.EdmEquals(property.Relationship)) && this.FromEnd.EdmEquals(property.FromEnd)) && this.ToEnd.EdmEquals(property.ToEnd));
        }

        public override int GetHashCode() => 
            this.ToEnd.Identity.GetHashCode();

        [DebuggerNonUserCode]
        public override string ToString() => 
            (this.m_relationshipType.ToString() + ":" + this.m_fromEnd.ToString() + ":" + this.m_toEnd.ToString());

        public RelationshipEndMember FromEnd =>
            this.m_fromEnd;

        public RelationshipType Relationship =>
            this.m_relationshipType;

        public RelationshipEndMember ToEnd =>
            this.m_toEnd;
    }
}

