namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data.Linq.SqlClient;

    internal class AttributedMetaAssociation : MetaAssociationImpl
    {
        private bool deleteOnNull;
        private string deleteRule;
        private bool isForeignKey;
        private bool isMany;
        private bool isNullable = true;
        private bool isUnique;
        private ReadOnlyCollection<MetaDataMember> otherKey;
        private bool otherKeyIsPrimaryKey;
        private MetaDataMember otherMember;
        private MetaType otherType;
        private ReadOnlyCollection<MetaDataMember> thisKey;
        private bool thisKeyIsPrimaryKey;
        private AttributedMetaDataMember thisMember;

        internal AttributedMetaAssociation(AttributedMetaDataMember member, AssociationAttribute attr)
        {
            this.thisMember = member;
            this.isMany = TypeSystem.IsSequenceType(this.thisMember.Type);
            Type type = this.isMany ? TypeSystem.GetElementType(this.thisMember.Type) : this.thisMember.Type;
            this.otherType = this.thisMember.DeclaringType.Model.GetMetaType(type);
            this.thisKey = (attr.ThisKey != null) ? MetaAssociationImpl.MakeKeys(this.thisMember.DeclaringType, attr.ThisKey) : this.thisMember.DeclaringType.IdentityMembers;
            this.otherKey = (attr.OtherKey != null) ? MetaAssociationImpl.MakeKeys(this.otherType, attr.OtherKey) : this.otherType.IdentityMembers;
            this.thisKeyIsPrimaryKey = MetaAssociationImpl.AreEqual(this.thisKey, this.thisMember.DeclaringType.IdentityMembers);
            this.otherKeyIsPrimaryKey = MetaAssociationImpl.AreEqual(this.otherKey, this.otherType.IdentityMembers);
            this.isForeignKey = attr.IsForeignKey;
            this.isUnique = attr.IsUnique;
            this.deleteRule = attr.DeleteRule;
            this.deleteOnNull = attr.DeleteOnNull;
            foreach (MetaDataMember member2 in this.thisKey)
            {
                if (!member2.CanBeNull)
                {
                    this.isNullable = false;
                    break;
                }
            }
            if (this.deleteOnNull && ((!this.isForeignKey || this.isMany) || this.isNullable))
            {
                throw System.Data.Linq.Mapping.Error.InvalidDeleteOnNullSpecification(member);
            }
            if (((this.thisKey.Count != this.otherKey.Count) && (this.thisKey.Count > 0)) && (this.otherKey.Count > 0))
            {
                throw System.Data.Linq.Mapping.Error.MismatchedThisKeyOtherKey(member.Name, member.DeclaringType.Name);
            }
            foreach (MetaDataMember member3 in this.otherType.PersistentDataMembers)
            {
                AssociationAttribute customAttribute = (AssociationAttribute) Attribute.GetCustomAttribute(member3.Member, typeof(AssociationAttribute));
                if (((customAttribute != null) && (member3 != this.thisMember)) && (customAttribute.Name == attr.Name))
                {
                    this.otherMember = member3;
                    break;
                }
            }
        }

        public override bool DeleteOnNull =>
            this.deleteOnNull;

        public override string DeleteRule =>
            this.deleteRule;

        public override bool IsForeignKey =>
            this.isForeignKey;

        public override bool IsMany =>
            this.isMany;

        public override bool IsNullable =>
            this.isNullable;

        public override bool IsUnique =>
            this.isUnique;

        public override ReadOnlyCollection<MetaDataMember> OtherKey =>
            this.otherKey;

        public override bool OtherKeyIsPrimaryKey =>
            this.otherKeyIsPrimaryKey;

        public override MetaDataMember OtherMember =>
            this.otherMember;

        public override MetaType OtherType =>
            this.otherType;

        public override ReadOnlyCollection<MetaDataMember> ThisKey =>
            this.thisKey;

        public override bool ThisKeyIsPrimaryKey =>
            this.thisKeyIsPrimaryKey;

        public override MetaDataMember ThisMember =>
            this.thisMember;
    }
}

