namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class ScalarConstant : CellConstant
    {
        private object m_scalar;

        internal ScalarConstant(object value)
        {
            this.m_scalar = value;
        }

        private StringBuilder AppendEscapedScalar(StringBuilder builder)
        {
            string str = StringUtil.FormatInvariant("{0}", new object[] { this.m_scalar });
            if (str.Contains("'"))
            {
                str = str.Replace("'", "''");
            }
            StringUtil.FormatStringBuilder(builder, "'{0}'", new object[] { str });
            return builder;
        }

        internal override StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias)
        {
            EdmType edmType = Helper.GetModelTypeUsage(outputMember.LastMember).EdmType;
            if (BuiltInTypeKind.PrimitiveType == edmType.BuiltInTypeKind)
            {
                switch (((PrimitiveType) edmType).PrimitiveTypeKind)
                {
                    case PrimitiveTypeKind.Boolean:
                    {
                        bool scalar = (bool) this.m_scalar;
                        string str = StringUtil.FormatInvariant("{0}", new object[] { scalar });
                        builder.Append(str);
                        return builder;
                    }
                    case PrimitiveTypeKind.String:
                        this.AppendEscapedScalar(builder);
                        return builder;
                }
            }
            else if (BuiltInTypeKind.EnumType == edmType.BuiltInTypeKind)
            {
                EnumMember member = (EnumMember) this.m_scalar;
                builder.Append(member.Name);
                return builder;
            }
            builder.Append("CAST(");
            this.AppendEscapedScalar(builder);
            builder.Append(" AS ");
            CqlWriter.AppendEscapedTypeName(builder, edmType);
            builder.Append(')');
            return builder;
        }

        protected override int GetHash() => 
            this.m_scalar?.GetHashCode();

        private void InternalToString(StringBuilder builder, bool isInvariant)
        {
            EnumMember scalar = this.m_scalar as EnumMember;
            if (scalar != null)
            {
                builder.Append(scalar.Name);
            }
            else if (this.m_scalar == null)
            {
                builder.Append(isInvariant ? "NULL" : Strings.ViewGen_Null);
            }
            else
            {
                builder.Append(StringUtil.FormatInvariant("'{0}'", new object[] { this.m_scalar }));
            }
        }

        protected override bool IsEqualTo(CellConstant right)
        {
            ScalarConstant constant = right as ScalarConstant;
            if (constant == null)
            {
                return false;
            }
            return CdpEqualityComparer.DefaultEqualityComparer.Equals(this.m_scalar, constant.m_scalar);
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

        internal object Value =>
            this.m_scalar;
    }
}

