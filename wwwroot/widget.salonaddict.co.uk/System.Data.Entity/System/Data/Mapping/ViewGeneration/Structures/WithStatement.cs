namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class WithStatement : InternalBase
    {
        private AssociationSet m_associationSet;
        private EntitySet m_entitySetForToEnd;
        private EntityType m_entityTypeForFromEnd;
        private EntityType m_entityTypeForToEnd;
        private string m_fromRoleName;
        private IEnumerable<MemberPath> m_memberPathsForEndMembers;
        private string m_toRoleName;

        internal WithStatement(EntitySet entitySetForToEnd, EntityType entityTypeForToEnd, EntityType entityTypeForFromEnd, AssociationSet associationSet, string fromRoleName, string toRoleName, IEnumerable<MemberPath> memberPathsForEndMembers)
        {
            this.m_entitySetForToEnd = entitySetForToEnd;
            this.m_entityTypeForToEnd = entityTypeForToEnd;
            this.m_entityTypeForFromEnd = entityTypeForFromEnd;
            this.m_memberPathsForEndMembers = memberPathsForEndMembers;
            this.m_associationSet = associationSet;
            this.m_fromRoleName = fromRoleName;
            this.m_toRoleName = toRoleName;
        }

        internal StringBuilder AsCql(StringBuilder builder, string blockAlias, int indentLevel)
        {
            StringUtil.IndentNewLine(builder, indentLevel + 1);
            builder.Append("RELATIONSHIP(");
            List<string> list = new List<string>();
            builder.Append("CREATEREF(");
            CqlWriter.AppendEscapedQualifiedName(builder, this.m_entitySetForToEnd.EntityContainer.Name, this.m_entitySetForToEnd.Name);
            builder.Append(", ROW(");
            foreach (MemberPath path in this.m_memberPathsForEndMembers)
            {
                string qualifiedName = CqlWriter.GetQualifiedName(blockAlias, path.CqlFieldAlias);
                list.Add(qualifiedName);
            }
            StringUtil.ToSeparatedString(builder, list, ", ", null);
            builder.Append(')');
            builder.Append(",");
            CqlWriter.AppendEscapedTypeName(builder, this.m_entityTypeForToEnd);
            builder.Append(')');
            builder.Append(',');
            CqlWriter.AppendEscapedTypeName(builder, this.m_associationSet.ElementType);
            builder.Append(',');
            CqlWriter.AppendEscapedName(builder, this.m_fromRoleName);
            builder.Append(',');
            CqlWriter.AppendEscapedName(builder, this.m_toRoleName);
            builder.Append(')');
            builder.Append(' ');
            return builder;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            throw new NotImplementedException();
        }

        internal EntityType EntityTypeForFromEnd =>
            this.m_entityTypeForFromEnd;
    }
}

