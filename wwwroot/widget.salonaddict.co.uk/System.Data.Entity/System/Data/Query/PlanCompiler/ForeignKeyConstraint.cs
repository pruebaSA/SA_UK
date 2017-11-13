namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal class ForeignKeyConstraint
    {
        private List<string> m_childKeys;
        private ReferentialConstraint m_constraint;
        private ExtentPair m_extentPair;
        private Dictionary<string, string> m_keyMap;
        private List<string> m_parentKeys;

        internal ForeignKeyConstraint(RelationshipType relType, RelationshipSet relationshipSet, ReferentialConstraint constraint)
        {
            AssociationSet associationSet = relationshipSet as AssociationSet;
            AssociationEndMember fromRole = constraint.FromRole as AssociationEndMember;
            AssociationEndMember toRole = constraint.ToRole as AssociationEndMember;
            if (((associationSet == null) || (fromRole == null)) || (toRole == null))
            {
                throw EntityUtil.NotSupported();
            }
            this.m_constraint = constraint;
            EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(associationSet, fromRole);
            EntitySet right = MetadataHelper.GetEntitySetAtEnd(associationSet, toRole);
            this.m_extentPair = new ExtentPair(entitySetAtEnd, right);
            this.m_childKeys = new List<string>();
            foreach (EdmProperty property in constraint.ToProperties)
            {
                this.m_childKeys.Add(property.Name);
            }
            this.m_parentKeys = new List<string>();
            foreach (EdmProperty property2 in constraint.FromProperties)
            {
                this.m_parentKeys.Add(property2.Name);
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((fromRole.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne) || (RelationshipMultiplicity.One == fromRole.RelationshipMultiplicity), "from-end of relationship constraint cannot have multiplicity greater than 1");
        }

        private void BuildKeyMap()
        {
            if (this.m_keyMap != null)
            {
                return;
            }
            this.m_keyMap = new Dictionary<string, string>();
            IEnumerator<EdmProperty> enumerator = this.m_constraint.FromProperties.GetEnumerator();
            IEnumerator<EdmProperty> enumerator2 = this.m_constraint.ToProperties.GetEnumerator();
            while (true)
            {
                bool flag = !enumerator.MoveNext();
                bool flag2 = !enumerator2.MoveNext();
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(flag == flag2, "key count mismatch");
                if (flag)
                {
                    return;
                }
                this.m_keyMap[enumerator2.Current.Name] = enumerator.Current.Name;
            }
        }

        public override bool Equals(object obj)
        {
            System.Data.Query.PlanCompiler.ForeignKeyConstraint constraint = obj as System.Data.Query.PlanCompiler.ForeignKeyConstraint;
            return ((constraint != null) && constraint.m_extentPair.Equals(this.m_extentPair));
        }

        public override int GetHashCode() => 
            this.m_extentPair.GetHashCode();

        internal bool GetParentProperty(string childPropertyName, out string parentPropertyName)
        {
            this.BuildKeyMap();
            return this.m_keyMap.TryGetValue(childPropertyName, out parentPropertyName);
        }

        internal List<string> ChildKeys =>
            this.m_childKeys;

        internal RelationshipMultiplicity ChildMultiplicity =>
            this.m_constraint.ToRole.RelationshipMultiplicity;

        internal ExtentPair Pair =>
            this.m_extentPair;

        internal List<string> ParentKeys =>
            this.m_parentKeys;
    }
}

