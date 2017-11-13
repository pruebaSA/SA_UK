namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class MemberCollection : MetadataCollection<EdmMember>
    {
        private StructuralType _declaringType;

        public MemberCollection(StructuralType declaringType) : this(declaringType, null)
        {
        }

        public MemberCollection(StructuralType declaringType, IEnumerable<EdmMember> items) : base(items)
        {
            this._declaringType = declaringType;
        }

        public override void Add(EdmMember member)
        {
            this.ValidateMemberForAdd(member, "member");
            base.Add(member);
            member.ChangeDeclaringTypeWithoutCollectionFixup(this._declaringType);
        }

        public override bool ContainsIdentity(string identity)
        {
            if (base.ContainsIdentity(identity))
            {
                return true;
            }
            EdmType baseType = this._declaringType.BaseType;
            return ((baseType != null) && ((StructuralType) baseType).Members.Contains(identity));
        }

        public override void CopyTo(EdmMember[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
            {
                throw EntityUtil.ArgumentOutOfRange("arrayIndex");
            }
            int baseTypeMemberCount = this.GetBaseTypeMemberCount();
            if ((base.Count + baseTypeMemberCount) > (array.Length - arrayIndex))
            {
                throw EntityUtil.Argument("arrayIndex");
            }
            if (baseTypeMemberCount > 0)
            {
                ((StructuralType) this._declaringType.BaseType).Members.CopyTo(array, arrayIndex);
            }
            base.CopyTo(array, arrayIndex + baseTypeMemberCount);
        }

        private int GetBaseTypeMemberCount()
        {
            StructuralType baseType = this._declaringType.BaseType as StructuralType;
            if (baseType != null)
            {
                return baseType.Members.Count;
            }
            return 0;
        }

        internal ReadOnlyMetadataCollection<T> GetDeclaredOnlyMembers<T>() where T: EdmMember
        {
            MetadataCollection<T> metadatas = new MetadataCollection<T>();
            for (int i = 0; i < base.Count; i++)
            {
                T item = base[i] as T;
                if (item != null)
                {
                    metadatas.Add(item);
                }
            }
            return metadatas.AsReadOnlyMetadataCollection();
        }

        private int GetRelativeIndex(int index)
        {
            int baseTypeMemberCount = this.GetBaseTypeMemberCount();
            int count = base.Count;
            if ((index < 0) || (index >= (baseTypeMemberCount + count)))
            {
                throw EntityUtil.ArgumentOutOfRange("index");
            }
            return (index - baseTypeMemberCount);
        }

        public override int IndexOf(EdmMember item)
        {
            int index = base.IndexOf(item);
            if (index != -1)
            {
                return (index + this.GetBaseTypeMemberCount());
            }
            StructuralType baseType = this._declaringType.BaseType as StructuralType;
            if (baseType != null)
            {
                return baseType.Members.IndexOf(item);
            }
            return -1;
        }

        private static void ThrowIfItHasDeclaringType(EdmMember member, string argumentName)
        {
            EntityUtil.GenericCheckArgumentNull<EdmMember>(member, argumentName);
            if (member.DeclaringType != null)
            {
                throw EntityUtil.MemberAlreadyBelongsToType(argumentName);
            }
        }

        public override bool TryGetValue(string identity, bool ignoreCase, out EdmMember item)
        {
            if (!base.TryGetValue(identity, ignoreCase, out item))
            {
                EdmType baseType = this._declaringType.BaseType;
                if (baseType != null)
                {
                    ((StructuralType) baseType).Members.TryGetValue(identity, ignoreCase, out item);
                }
            }
            return (item != null);
        }

        private void ValidateMemberForAdd(EdmMember member, string argumentName)
        {
            ThrowIfItHasDeclaringType(member, argumentName);
            this._declaringType.ValidateMemberForAdd(member);
        }

        public override ReadOnlyCollection<EdmMember> AsReadOnly =>
            new ReadOnlyCollection<EdmMember>(this);

        public override int Count =>
            (this.GetBaseTypeMemberCount() + base.Count);

        public override EdmMember this[int index]
        {
            get
            {
                int relativeIndex = this.GetRelativeIndex(index);
                if (relativeIndex < 0)
                {
                    return ((StructuralType) this._declaringType.BaseType).Members[index];
                }
                return base[relativeIndex];
            }
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }

        public override EdmMember this[string identity]
        {
            get
            {
                EdmMember item = null;
                if (!this.TryGetValue(identity, false, out item))
                {
                    throw EntityUtil.MemberInvalidIdentity(identity, "identity");
                }
                return item;
            }
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }
    }
}

