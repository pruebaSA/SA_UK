namespace System.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public class DataRecordInfo
    {
        private readonly System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Common.FieldMetadata> _fieldMetadata;
        private readonly TypeUsage _metadata;

        internal DataRecordInfo(DataRecordInfo recordInfo)
        {
            this._fieldMetadata = recordInfo._fieldMetadata;
            this._metadata = recordInfo._metadata;
        }

        internal DataRecordInfo(TypeUsage metadata)
        {
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(metadata);
            System.Data.Common.FieldMetadata[] list = new System.Data.Common.FieldMetadata[allStructuralMembers.Count];
            for (int i = 0; i < list.Length; i++)
            {
                EdmMember fieldType = allStructuralMembers[i];
                list[i] = new System.Data.Common.FieldMetadata(i, fieldType);
            }
            this._fieldMetadata = new System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Common.FieldMetadata>(list);
            this._metadata = metadata;
        }

        public DataRecordInfo(TypeUsage metadata, IEnumerable<EdmMember> memberInfo)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(metadata, "metadata");
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(metadata.EdmType);
            List<System.Data.Common.FieldMetadata> list = new List<System.Data.Common.FieldMetadata>(allStructuralMembers.Count);
            if (memberInfo != null)
            {
                foreach (EdmMember member in memberInfo)
                {
                    if (((member == null) || (0 > allStructuralMembers.IndexOf(member))) || ((BuiltInTypeKind.EdmProperty != member.BuiltInTypeKind) && (member.BuiltInTypeKind != BuiltInTypeKind.AssociationEndMember)))
                    {
                        throw EntityUtil.Argument("memberInfo");
                    }
                    if ((member.DeclaringType != metadata.EdmType) && !member.DeclaringType.IsBaseTypeOf(metadata.EdmType))
                    {
                        throw EntityUtil.Argument(Strings.EdmMembersDefiningTypeDoNotAgreeWithMetadataType);
                    }
                    list.Add(new System.Data.Common.FieldMetadata(list.Count, member));
                }
            }
            if (Helper.IsStructuralType(metadata.EdmType) != (0 < list.Count))
            {
                throw EntityUtil.Argument("memberInfo");
            }
            this._fieldMetadata = new System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Common.FieldMetadata>(list);
            this._metadata = metadata;
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Common.FieldMetadata> FieldMetadata =>
            this._fieldMetadata;

        public TypeUsage RecordType =>
            this._metadata;
    }
}

