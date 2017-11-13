namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class SignatureGenerator : InternalBase
    {
        private MemberPathMap m_projectedSlotMap;
        private MetadataWorkspace m_workspace;

        internal SignatureGenerator(EntitySetBase extent, MetadataWorkspace workspace)
        {
            this.m_workspace = workspace;
            this.m_projectedSlotMap = new MemberPathMap();
            MemberPath member = new MemberPath(extent, workspace);
            this.GatherPartialSignature(member, false, false);
        }

        private void GatherPartialSignature(MemberPath member, bool isNullable, bool needKeysOnly)
        {
            EdmType edmType = member.EdmType;
            if (!(edmType is ComplexType) || !needKeysOnly)
            {
                this.m_projectedSlotMap.CreateIndex(member);
                List<EdmType> list = new List<EdmType>();
                list.AddRange(MetadataHelper.GetTypeAndSubtypesOf(edmType, this.m_workspace, false));
                foreach (EdmType type2 in list)
                {
                    this.GatherSignatureFromScalars(member, type2, needKeysOnly);
                    this.GatherSignaturesFromNonScalars(member, type2, needKeysOnly);
                }
            }
        }

        private void GatherSignatureFromScalars(MemberPath member, EdmType possibleType, bool needKeysOnly)
        {
            StructuralType type = (StructuralType) possibleType;
            foreach (EdmMember member2 in type.Members)
            {
                if (Helper.IsEdmProperty(member2))
                {
                    EdmProperty property = (EdmProperty) member2;
                    if (MetadataHelper.IsNonRefSimpleMember(property) && (!needKeysOnly || MetadataHelper.IsPartOfEntityTypeKey(property)))
                    {
                        MemberPath path = new MemberPath(member, member2);
                        this.m_projectedSlotMap.CreateIndex(path);
                    }
                }
            }
        }

        private void GatherSignaturesFromNonScalars(MemberPath member, EdmType possibleType, bool needKeysOnly)
        {
            int num = 0;
            foreach (EdmMember member2 in Helper.GetAllStructuralMembers(possibleType))
            {
                num++;
                if (!MetadataHelper.IsNonRefSimpleMember(member2))
                {
                    bool isNullable = MetadataHelper.IsMemberNullable(member2);
                    needKeysOnly = needKeysOnly || (member2 is AssociationEndMember);
                    MemberPath path = new MemberPath(member, member2);
                    this.GatherPartialSignature(path, isNullable, needKeysOnly);
                }
            }
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("Projected Slot Map: ");
            this.m_projectedSlotMap.ToCompactString(builder);
        }

        internal MemberPathMapBase ProjectedSlotMap =>
            this.m_projectedSlotMap;
    }
}

