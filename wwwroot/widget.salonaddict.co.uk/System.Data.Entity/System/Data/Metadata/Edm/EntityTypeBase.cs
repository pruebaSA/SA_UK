namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;

    public abstract class EntityTypeBase : StructuralType
    {
        private string[] _keyMemberNames;
        private readonly ReadOnlyMetadataCollection<EdmMember> _keyMembers;

        internal EntityTypeBase(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
            this._keyMembers = new ReadOnlyMetadataCollection<EdmMember>(new MetadataCollection<EdmMember>());
        }

        internal void AddKeyMember(EdmMember member)
        {
            EntityUtil.GenericCheckArgumentNull<EdmMember>(member, "member");
            Util.ThrowIfReadOnly(this);
            if (!base.Members.Contains(member))
            {
                base.AddMember(member);
            }
            this._keyMembers.Source.Add(member);
        }

        internal void CheckAndAddKeyMembers(IEnumerable<string> keyMembers)
        {
            foreach (string str in keyMembers)
            {
                EdmMember member;
                if (str == null)
                {
                    throw EntityUtil.CollectionParameterElementIsNull("keyMembers");
                }
                if (!base.Members.TryGetValue(str, false, out member))
                {
                    throw EntityUtil.Argument(Strings.InvalidKeyMember(str));
                }
                this.AddKeyMember(member);
            }
        }

        internal static void CheckAndAddMembers(IEnumerable<EdmMember> members, EntityType entityType)
        {
            foreach (EdmMember member in members)
            {
                if (member == null)
                {
                    throw EntityUtil.CollectionParameterElementIsNull("members");
                }
                entityType.AddMember(member);
            }
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                this._keyMembers.Source.SetReadOnly();
                base.SetReadOnly();
            }
        }

        internal string[] KeyMemberNames
        {
            get
            {
                if (this._keyMemberNames == null)
                {
                    string[] strArray = new string[this.KeyMembers.Count];
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        strArray[i] = this.KeyMembers[i].Name;
                    }
                    this._keyMemberNames = strArray;
                }
                return this._keyMemberNames;
            }
        }

        [MetadataProperty(BuiltInTypeKind.EdmMember, true)]
        public ReadOnlyMetadataCollection<EdmMember> KeyMembers
        {
            get
            {
                if ((base.BaseType != null) && (((EntityTypeBase) base.BaseType).KeyMembers.Count != 0))
                {
                    return ((EntityTypeBase) base.BaseType).KeyMembers;
                }
                return this._keyMembers;
            }
        }
    }
}

