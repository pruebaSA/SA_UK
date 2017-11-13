namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class TypeConstant : CellConstant
    {
        private EdmType m_cdmType;

        internal TypeConstant(EdmType type)
        {
            this.m_cdmType = type;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias)
        {
            List<string> list = new List<string>();
            EntitySet scopeOfRelationEnd = outputMember.GetScopeOfRelationEnd();
            EntityType type = null;
            if (scopeOfRelationEnd != null)
            {
                EntityType elementType = scopeOfRelationEnd.ElementType;
                type = (EntityType) ((RefType) outputMember.EdmType).ElementType;
                builder.Append("CreateRef(");
                CqlWriter.AppendEscapedQualifiedName(builder, scopeOfRelationEnd.EntityContainer.Name, scopeOfRelationEnd.Name);
                builder.Append(", row(");
                foreach (EdmMember member in elementType.KeyMembers)
                {
                    MemberPath path = new MemberPath(outputMember, member);
                    string qualifiedName = CqlWriter.GetQualifiedName(blockAlias, path.CqlFieldAlias);
                    list.Add(qualifiedName);
                }
            }
            else
            {
                StructuralType cdmType = (StructuralType) this.m_cdmType;
                foreach (EdmMember member2 in Helper.GetAllStructuralMembers(cdmType))
                {
                    MemberPath path2 = new MemberPath(outputMember, member2);
                    string item = CqlWriter.GetQualifiedName(blockAlias, path2.CqlFieldAlias);
                    list.Add(item);
                }
                CqlWriter.AppendEscapedTypeName(builder, this.m_cdmType);
                builder.Append('(');
            }
            StringUtil.ToSeparatedString(builder, list, ", ", null);
            builder.Append(')');
            if (scopeOfRelationEnd != null)
            {
                builder.Append(",");
                CqlWriter.AppendEscapedTypeName(builder, type);
                builder.Append(')');
            }
            return builder;
        }

        protected override int GetHash() => 
            this.m_cdmType?.GetHashCode();

        private void InternalToString(StringBuilder builder, bool isInvariant)
        {
            if (this.m_cdmType == null)
            {
                builder.Append(isInvariant ? "NULL" : Strings.ViewGen_Null);
            }
            else
            {
                builder.Append(this.m_cdmType.Name);
            }
        }

        protected override bool IsEqualTo(CellConstant right)
        {
            TypeConstant constant = right as TypeConstant;
            if (constant == null)
            {
                return false;
            }
            return (this.m_cdmType == constant.m_cdmType);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.InternalToString(builder, true);
        }

        internal override string ToUserString()
        {
            StringBuilder builder = new StringBuilder();
            this.InternalToString(builder, false);
            return builder.ToString();
        }

        internal EdmType CdmType =>
            this.m_cdmType;
    }
}

