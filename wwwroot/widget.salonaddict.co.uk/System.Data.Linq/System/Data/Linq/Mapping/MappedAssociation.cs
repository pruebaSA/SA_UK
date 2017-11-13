namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.ObjectModel;
    using System.Data.Linq.SqlClient;

    internal class MappedAssociation : MetaAssociationImpl
    {
        private AssociationMapping assocMap;
        private bool isForeignKey;
        private bool isMany;
        private bool isNullable;
        private ReadOnlyCollection<MetaDataMember> otherKey;
        private bool otherKeyIsPrimaryKey;
        private MetaDataMember otherMember;
        private MetaType otherType;
        private ReadOnlyCollection<MetaDataMember> thisKey;
        private bool thisKeyIsPrimaryKey;
        private MappedDataMember thisMember;

        internal MappedAssociation(MappedDataMember mm, AssociationMapping assocMap)
        {
            this.thisMember = mm;
            this.assocMap = assocMap;
            this.Init();
            this.InitOther();
            if (((this.thisKey.Count != this.otherKey.Count) && (this.thisKey.Count > 0)) && (this.otherKey.Count > 0))
            {
                throw System.Data.Linq.Mapping.Error.MismatchedThisKeyOtherKey(this.thisMember.Name, this.thisMember.DeclaringType.Name);
            }
        }

        private void Init()
        {
            this.isMany = TypeSystem.IsSequenceType(this.thisMember.Type);
            this.thisKey = (this.assocMap.ThisKey != null) ? MetaAssociationImpl.MakeKeys(this.thisMember.DeclaringType, this.assocMap.ThisKey) : this.thisMember.DeclaringType.IdentityMembers;
            this.thisKeyIsPrimaryKey = MetaAssociationImpl.AreEqual(this.thisKey, this.thisMember.DeclaringType.IdentityMembers);
            this.isForeignKey = this.assocMap.IsForeignKey;
            this.isNullable = true;
            foreach (MetaDataMember member in this.thisKey)
            {
                if (member == null)
                {
                    throw System.Data.Linq.Mapping.Error.UnexpectedNull("MetaDataMember");
                }
                if (!member.CanBeNull)
                {
                    this.isNullable = false;
                    break;
                }
            }
            if (this.assocMap.DeleteOnNull && ((!this.isForeignKey || this.isMany) || this.isNullable))
            {
                throw System.Data.Linq.Mapping.Error.InvalidDeleteOnNullSpecification(this.thisMember);
            }
        }

        private void InitOther()
        {
            if (this.otherType == null)
            {
                Type type = this.isMany ? TypeSystem.GetElementType(this.thisMember.Type) : this.thisMember.Type;
                this.otherType = this.thisMember.DeclaringType.Model.GetMetaType(type);
                this.otherKey = (this.assocMap.OtherKey != null) ? MetaAssociationImpl.MakeKeys(this.otherType, this.assocMap.OtherKey) : this.otherType.IdentityMembers;
                this.otherKeyIsPrimaryKey = MetaAssociationImpl.AreEqual(this.otherKey, this.otherType.IdentityMembers);
                foreach (MetaDataMember member in this.otherType.DataMembers)
                {
                    if ((member.IsAssociation && (member != this.thisMember)) && (member.MappedName == this.thisMember.MappedName))
                    {
                        this.otherMember = member;
                        break;
                    }
                }
            }
        }

        public override bool DeleteOnNull =>
            this.assocMap.DeleteOnNull;

        public override string DeleteRule =>
            this.assocMap.DeleteRule;

        public override bool IsForeignKey =>
            this.isForeignKey;

        public override bool IsMany =>
            this.isMany;

        public override bool IsNullable =>
            this.isNullable;

        public override bool IsUnique =>
            this.assocMap.IsUnique;

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

