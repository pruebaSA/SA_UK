namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data.Metadata.Edm;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class EdmRelationshipAttribute : Attribute
    {
        private string _relationshipName;
        private string _relationshipNamespaceName;
        private RelationshipMultiplicity _role1Multiplicity;
        private string _role1Name;
        private Type _role1Type;
        private RelationshipMultiplicity _role2Multiplicity;
        private string _role2Name;
        private Type _role2Type;

        public EdmRelationshipAttribute(string relationshipNamespaceName, string relationshipName, string role1Name, RelationshipMultiplicity role1Multiplicity, Type role1Type, string role2Name, RelationshipMultiplicity role2Multiplicity, Type role2Type)
        {
            this._relationshipNamespaceName = relationshipNamespaceName;
            this._relationshipName = relationshipName;
            this._role1Name = role1Name;
            this._role1Multiplicity = role1Multiplicity;
            this._role1Type = role1Type;
            this._role2Name = role2Name;
            this._role2Multiplicity = role2Multiplicity;
            this._role2Type = role2Type;
        }

        public string RelationshipName =>
            this._relationshipName;

        public string RelationshipNamespaceName =>
            this._relationshipNamespaceName;

        public RelationshipMultiplicity Role1Multiplicity =>
            this._role1Multiplicity;

        public string Role1Name =>
            this._role1Name;

        public Type Role1Type =>
            this._role1Type;

        public RelationshipMultiplicity Role2Multiplicity =>
            this._role2Multiplicity;

        public string Role2Name =>
            this._role2Name;

        public Type Role2Type =>
            this._role2Type;
    }
}

