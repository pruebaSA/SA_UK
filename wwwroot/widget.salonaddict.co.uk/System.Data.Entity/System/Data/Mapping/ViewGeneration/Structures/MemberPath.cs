namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class MemberPath : InternalBase, IEquatable<MemberPath>
    {
        internal static readonly IEqualityComparer<MemberPath> EqualityComparer = new MemberPathComparer();
        private EntitySetBase m_extent;
        private List<EdmMember> m_path;
        private System.Data.Metadata.Edm.MetadataWorkspace m_workspace;

        internal MemberPath(MemberPath prefix, EdmMember last)
        {
            this.m_extent = prefix.m_extent;
            this.m_path = new List<EdmMember>(prefix.m_path);
            this.m_path.Add(last);
            this.m_workspace = prefix.MetadataWorkspace;
        }

        internal MemberPath(EntitySetBase extent, System.Data.Metadata.Edm.MetadataWorkspace workspace) : this(extent, new EdmMember[0], workspace)
        {
        }

        internal MemberPath(EntitySetBase extent, IEnumerable<EdmMember> path, System.Data.Metadata.Edm.MetadataWorkspace workspace)
        {
            this.m_extent = extent;
            this.m_path = new List<EdmMember>(path);
            this.m_workspace = workspace;
        }

        internal MemberPath(EntitySetBase extent, EdmMember member, System.Data.Metadata.Edm.MetadataWorkspace workspace) : this(extent, new EdmMember[] { member }, workspace)
        {
        }

        internal static bool AreAllMembersNullable(IEnumerable<MemberPath> members)
        {
            foreach (MemberPath path in members)
            {
                if (path.m_path.Count == 0)
                {
                    return false;
                }
                if (!path.IsNullable)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AreAssocationEndPathsEquivalentViaRefConstraint(MemberPath assocPath0, MemberPath assocPath1, AssociationSet assocSet)
        {
            AssociationEndMember firstMember = assocPath0.FirstMember as AssociationEndMember;
            AssociationEndMember member2 = assocPath1.FirstMember as AssociationEndMember;
            EdmProperty lastMember = assocPath0.LastMember as EdmProperty;
            EdmProperty property2 = assocPath1.LastMember as EdmProperty;
            if (((firstMember == null) || (member2 == null)) || ((lastMember == null) || (property2 == null)))
            {
                return false;
            }
            AssociationType elementType = assocSet.ElementType;
            foreach (ReferentialConstraint constraint in elementType.ReferentialConstraints)
            {
                bool flag2 = (firstMember.Name == constraint.FromRole.Name) && (member2.Name == constraint.ToRole.Name);
                bool flag3 = (member2.Name == constraint.FromRole.Name) && (firstMember.Name == constraint.ToRole.Name);
                if (flag2 || flag3)
                {
                    ReadOnlyMetadataCollection<EdmProperty> metadatas = flag2 ? constraint.FromProperties : constraint.ToProperties;
                    ReadOnlyMetadataCollection<EdmProperty> metadatas2 = flag2 ? constraint.ToProperties : constraint.FromProperties;
                    int index = metadatas.IndexOf(lastMember);
                    int num2 = metadatas2.IndexOf(property2);
                    if ((index == num2) && (index != -1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal StringBuilder AsCql(StringBuilder inputBuilder, string blockAlias)
        {
            StringBuilder builder = new StringBuilder();
            CqlWriter.AppendEscapedName(builder, blockAlias);
            System.Data.Metadata.Edm.EdmType elementType = this.m_extent.ElementType;
            foreach (EdmMember member in this.m_path)
            {
                StructuralType type2;
                RefType type3 = null;
                if (Helper.IsRefType(elementType))
                {
                    type3 = (RefType) elementType;
                    type2 = type3.ElementType;
                }
                else
                {
                    type2 = (StructuralType) elementType;
                }
                bool flag = MetadataHelper.DoesMemberExist(type2, member);
                if (type3 != null)
                {
                    builder.Insert(0, "Key(");
                    builder.Append(")");
                }
                else if (!flag)
                {
                    builder.Insert(0, "TREAT(");
                    builder.Append(" AS ");
                    CqlWriter.AppendEscapedTypeName(builder, member.DeclaringType);
                    builder.Append(')');
                }
                builder.Append('.');
                CqlWriter.AppendEscapedName(builder, member.Name);
                elementType = member.TypeUsage.EdmType;
            }
            inputBuilder.Append(builder.ToString());
            return inputBuilder;
        }

        public bool Equals(MemberPath right) => 
            EqualityComparer.Equals(this, right);

        public override bool Equals(object obj)
        {
            MemberPath right = obj as MemberPath;
            if (obj == null)
            {
                return false;
            }
            return this.Equals(right);
        }

        private MemberPath GetCorrespondingAssociationPath(AssociationSet assocSet)
        {
            AssociationEndMember someEndForEntitySet = MetadataHelper.GetSomeEndForEntitySet(assocSet, (System.Data.Metadata.Edm.EntitySet) this.m_extent);
            if (someEndForEntitySet == null)
            {
                return null;
            }
            List<EdmMember> path = new List<EdmMember> {
                someEndForEntitySet
            };
            path.AddRange(this.m_path);
            return new MemberPath(assocSet, path, this.m_workspace);
        }

        public override int GetHashCode() => 
            EqualityComparer.GetHashCode(this);

        internal void GetIdentifiers(CqlIdentifiers identifiers)
        {
            identifiers.AddIdentifier(this.m_extent.Name);
            identifiers.AddIdentifier(this.m_extent.ElementType.Name);
            foreach (EdmMember member in this.m_path)
            {
                identifiers.AddIdentifier(member.Name);
            }
        }

        internal static IEnumerable<MemberPath> GetKeyMembers(EntitySetBase extent, MemberDomainMap domainMap, System.Data.Metadata.Edm.MetadataWorkspace workspace)
        {
            MemberPath path = new MemberPath(extent, workspace);
            return new List<MemberPath>(path.GetMembers(path.Extent.ElementType, null, null, true, domainMap));
        }

        internal IEnumerable<MemberPath> GetMembers(System.Data.Metadata.Edm.EdmType edmType, bool? isScalar, bool? isConditional, bool? isPartOfKey, MemberDomainMap domainMap)
        {
            MemberPath prefix = this;
            StructuralType iteratorVariable1 = (StructuralType) edmType;
            foreach (EdmMember iteratorVariable2 in iteratorVariable1.Members)
            {
                if (iteratorVariable2 is AssociationEndMember)
                {
                    foreach (MemberPath iteratorVariable3 in new MemberPath(prefix, iteratorVariable2).GetMembers(((RefType) iteratorVariable2.TypeUsage.EdmType).ElementType, isScalar, isConditional, true, domainMap))
                    {
                        yield return iteratorVariable3;
                    }
                }
                bool iteratorVariable4 = MetadataHelper.IsNonRefSimpleMember(iteratorVariable2);
                if (isScalar.HasValue)
                {
                    bool? isScalar = isScalar;
                    bool flag2 = iteratorVariable4;
                    if (!((isScalar.GetValueOrDefault() == flag2) && isScalar.HasValue))
                    {
                        continue;
                    }
                }
                EdmProperty member = iteratorVariable2 as EdmProperty;
                if (member != null)
                {
                    bool iteratorVariable6 = MetadataHelper.IsPartOfEntityTypeKey(member);
                    if (isPartOfKey.HasValue)
                    {
                        bool? isPartOfKey = isPartOfKey;
                        bool flag3 = iteratorVariable6;
                        if (!((isPartOfKey.GetValueOrDefault() == flag3) && isPartOfKey.HasValue))
                        {
                            continue;
                        }
                    }
                    MemberPath path = new MemberPath(prefix, member);
                    bool iteratorVariable8 = domainMap.IsConditionMember(path);
                    if (isConditional.HasValue)
                    {
                        bool? isConditional = isConditional;
                        bool flag4 = iteratorVariable8;
                        if (!((isConditional.GetValueOrDefault() == flag4) && isConditional.HasValue))
                        {
                            continue;
                        }
                    }
                    yield return path;
                }
            }
        }

        internal System.Data.Metadata.Edm.EntitySet GetScopeOfRelationEnd()
        {
            if (this.m_path.Count == 0)
            {
                return null;
            }
            AssociationEndMember lastMember = this.LastMember as AssociationEndMember;
            if (lastMember == null)
            {
                return null;
            }
            AssociationSet extent = (AssociationSet) this.m_extent;
            return MetadataHelper.GetEntitySetAtEnd(extent, lastMember);
        }

        internal bool IsAlwaysDefined(Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> inheritanceGraph)
        {
            if (this.m_path.Count == 0)
            {
                return true;
            }
            EdmMember member = this.m_path.Last<EdmMember>();
            for (int i = 0; i < (this.m_path.Count - 1); i++)
            {
                EdmMember member2 = this.m_path[i];
                if (MetadataHelper.IsMemberNullable(member2))
                {
                    return false;
                }
            }
            if (this.m_path[0].DeclaringType is AssociationType)
            {
                return true;
            }
            EntityType elementType = this.m_extent.ElementType as EntityType;
            if (elementType == null)
            {
                return true;
            }
            EntityType declaringType = this.m_path[0].DeclaringType as EntityType;
            EntityType baseType = declaringType.BaseType as EntityType;
            if ((elementType.EdmEquals(declaringType) || MetadataHelper.IsParentOf(declaringType, elementType)) || (baseType == null))
            {
                return true;
            }
            if (!baseType.Abstract && !MetadataHelper.DoesMemberExist(baseType, member))
            {
                return false;
            }
            return !RecurseToFindMemberAbsentInConcreteType(baseType, declaringType, member, elementType, inheritanceGraph);
        }

        internal bool IsEquivalentViaRefConstraint(MemberPath path1)
        {
            MemberPath path = this;
            if (((path.EdmType is EntityTypeBase) || (path1.EdmType is EntityTypeBase)) || (!MetadataHelper.IsNonRefSimpleMember(path.LastMember) || !MetadataHelper.IsNonRefSimpleMember(path1.LastMember)))
            {
                return false;
            }
            AssociationSet extent = path.Extent as AssociationSet;
            AssociationSet set2 = path1.Extent as AssociationSet;
            System.Data.Metadata.Edm.EntitySet set3 = path.Extent as System.Data.Metadata.Edm.EntitySet;
            System.Data.Metadata.Edm.EntitySet set4 = path1.Extent as System.Data.Metadata.Edm.EntitySet;
            bool flag = false;
            if ((extent != null) && (set2 != null))
            {
                if (!extent.Equals(set2))
                {
                    return false;
                }
                return AreAssocationEndPathsEquivalentViaRefConstraint(path, path1, extent);
            }
            if ((set3 != null) && (set4 != null))
            {
                foreach (AssociationSet set5 in MetadataHelper.GetAssociationsForEntitySets(set3, set4))
                {
                    MemberPath path2 = path.GetCorrespondingAssociationPath(set5);
                    MemberPath path3 = path1.GetCorrespondingAssociationPath(set5);
                    if (AreAssocationEndPathsEquivalentViaRefConstraint(path2, path3, set5))
                    {
                        return true;
                    }
                }
                return flag;
            }
            AssociationSet assocSet = (extent != null) ? extent : set2;
            MemberPath path4 = (path.Extent is AssociationSet) ? path : path1;
            MemberPath correspondingAssociationPath = ((path.Extent is System.Data.Metadata.Edm.EntitySet) ? path : path1).GetCorrespondingAssociationPath(assocSet);
            if (correspondingAssociationPath == null)
            {
                return false;
            }
            return AreAssocationEndPathsEquivalentViaRefConstraint(path4, correspondingAssociationPath, assocSet);
        }

        internal bool IsScalarType()
        {
            if (this.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
            {
                return (this.EdmType.BuiltInTypeKind == BuiltInTypeKind.EnumType);
            }
            return true;
        }

        internal string PathToString(bool? forAlias)
        {
            StringBuilder builder = new StringBuilder();
            if (forAlias.HasValue)
            {
                if (forAlias == true)
                {
                    if (this.m_path.Count == 0)
                    {
                        return this.m_extent.ElementType.Name;
                    }
                    builder.Append(this.m_path[0].DeclaringType.Name);
                }
                else
                {
                    builder.Append(this.m_extent.Name);
                }
            }
            for (int i = 0; i < this.m_path.Count; i++)
            {
                builder.Append('.');
                builder.Append(this.m_path[i].Name);
            }
            return builder.ToString();
        }

        internal static string PropertiesToUserString(IEnumerable<MemberPath> members, bool fullPath)
        {
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (MemberPath path in members)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                flag = false;
                if (fullPath)
                {
                    builder.Append(path.PathToString(false));
                }
                else
                {
                    builder.Append(path.LastComponentName);
                }
            }
            return builder.ToString();
        }

        private static bool RecurseToFindMemberAbsentInConcreteType(EntityType current, EntityType avoidEdge, EdmMember member, EntityType entitySetType, Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> inheritanceGraph)
        {
            System.Data.Common.Utils.Set<EntityType> set = inheritanceGraph[current];
            foreach (EntityType type in from type in set
                where !type.EdmEquals(avoidEdge)
                select type)
            {
                if ((entitySetType.BaseType == null) || !entitySetType.BaseType.EdmEquals(type))
                {
                    if (!type.Abstract && !MetadataHelper.DoesMemberExist(type, member))
                    {
                        return true;
                    }
                    if (RecurseToFindMemberAbsentInConcreteType(type, current, member, entitySetType, inheritanceGraph))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.PathToString(false));
        }

        internal void ToCompactString(StringBuilder builder, string instanceToken)
        {
            builder.Append(instanceToken + this.PathToString(null));
        }

        internal string CqlFieldAlias
        {
            get
            {
                string name = this.PathToString(true);
                if (!name.Contains("_"))
                {
                    name = name.Replace('.', '_');
                }
                StringBuilder builder = new StringBuilder();
                CqlWriter.AppendEscapedName(builder, name);
                return builder.ToString();
            }
        }

        internal object DefaultValue
        {
            get
            {
                Facet facet;
                if ((this.m_path.Count != 0) && this.LastMember.TypeUsage.Facets.TryGetValue("DefaultValue", false, out facet))
                {
                    return facet.Value;
                }
                return null;
            }
        }

        internal System.Data.Metadata.Edm.EdmType EdmType
        {
            get
            {
                if (this.m_path.Count > 0)
                {
                    return this.LastMember.TypeUsage.EdmType;
                }
                return this.m_extent.ElementType;
            }
        }

        internal System.Data.Metadata.Edm.EntitySet EntitySet
        {
            get
            {
                if (this.m_path.Count == 0)
                {
                    return (System.Data.Metadata.Edm.EntitySet) this.m_extent;
                }
                AssociationEndMember firstMember = (AssociationEndMember) this.FirstMember;
                return MetadataHelper.GetEntitySetAtEnd((AssociationSet) this.m_extent, firstMember);
            }
        }

        internal EntitySetBase Extent =>
            this.m_extent;

        internal EdmMember FirstMember =>
            this.m_path[0];

        internal bool IsComputed
        {
            get
            {
                if (this.m_path.Count == 0)
                {
                    return false;
                }
                return this.FirstMember.IsStoreGeneratedComputed;
            }
        }

        internal bool IsNullable
        {
            get
            {
                if (this.m_path.Count == 0)
                {
                    return false;
                }
                return MetadataHelper.IsMemberNullable(this.LastMember);
            }
        }

        internal bool IsPartOfKey
        {
            get
            {
                if (this.m_path.Count == 0)
                {
                    return false;
                }
                EdmProperty lastMember = this.LastMember as EdmProperty;
                return MetadataHelper.IsPartOfEntityTypeKey(lastMember);
            }
        }

        internal string LastComponentName
        {
            get
            {
                if (this.m_path.Count == 0)
                {
                    return this.m_extent.Name;
                }
                return this.LastMember.Name;
            }
        }

        internal EdmMember LastMember =>
            this.m_path[this.m_path.Count - 1];

        internal IList<EdmMember> Members =>
            this.m_path;

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_workspace;


        private class MemberPathComparer : IEqualityComparer<MemberPath>
        {
            public bool Equals(MemberPath left, MemberPath right)
            {
                if (!object.ReferenceEquals(left, right))
                {
                    if ((left == null) || (right == null))
                    {
                        return false;
                    }
                    if (!left.m_extent.Equals(right.m_extent) || (left.m_path.Count != right.m_path.Count))
                    {
                        return false;
                    }
                    for (int i = 0; i < left.m_path.Count; i++)
                    {
                        if (!left.m_path[i].Equals(right.m_path[i]))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public int GetHashCode(MemberPath key)
            {
                int hashCode = key.m_extent.GetHashCode();
                foreach (EdmMember member in key.m_path)
                {
                    hashCode ^= member.GetHashCode();
                }
                return hashCode;
            }
        }
    }
}

