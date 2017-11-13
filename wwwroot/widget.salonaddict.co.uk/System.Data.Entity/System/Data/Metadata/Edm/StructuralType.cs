namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    public abstract class StructuralType : EdmType
    {
        private readonly MemberCollection _members;
        private readonly ReadOnlyMetadataCollection<EdmMember> _readOnlyMembers;

        internal StructuralType()
        {
            this._members = new MemberCollection(this);
            this._readOnlyMembers = this._members.AsReadOnlyMetadataCollection();
        }

        internal StructuralType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
            this._members = new MemberCollection(this);
            this._readOnlyMembers = this._members.AsReadOnlyMetadataCollection();
        }

        internal void AddMember(EdmMember member)
        {
            EntityUtil.GenericCheckArgumentNull<EdmMember>(member, "member");
            Util.ThrowIfReadOnly(this);
            if (BuiltInTypeKind.RowType == this.BuiltInTypeKind)
            {
                if (this._members.Count == 0)
                {
                    base.DataSpace = member.TypeUsage.EdmType.DataSpace;
                }
                else if ((base.DataSpace != ~DataSpace.OSpace) && (member.TypeUsage.EdmType.DataSpace != base.DataSpace))
                {
                    base.DataSpace = ~DataSpace.OSpace;
                }
            }
            this._members.Add(member);
        }

        internal ReadOnlyMetadataCollection<T> GetDeclaredOnlyMembers<T>() where T: EdmMember => 
            this._members.GetDeclaredOnlyMembers<T>();

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.Members.Source.SetReadOnly();
            }
        }

        internal abstract void ValidateMemberForAdd(EdmMember member);

        [MetadataProperty(BuiltInTypeKind.EdmMember, true)]
        public ReadOnlyMetadataCollection<EdmMember> Members =>
            this._readOnlyMembers;
    }
}

