namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class RelPropertyHelper
    {
        private HashSet<RelProperty> _interestingRelProperties;
        private Dictionary<EntityTypeBase, List<RelProperty>> _relPropertyMap = new Dictionary<EntityTypeBase, List<RelProperty>>();

        internal RelPropertyHelper(MetadataWorkspace ws, HashSet<RelProperty> interestingRelProperties)
        {
            this._interestingRelProperties = interestingRelProperties;
            foreach (RelationshipType type in ws.GetItems<RelationshipType>(DataSpace.CSpace))
            {
                this.ProcessRelationship(type);
            }
        }

        private void AddRelProperty(AssociationType associationType, AssociationEndMember fromEnd, AssociationEndMember toEnd)
        {
            if (toEnd.RelationshipMultiplicity != RelationshipMultiplicity.Many)
            {
                RelProperty item = new RelProperty(associationType, fromEnd, toEnd);
                if ((this._interestingRelProperties != null) && this._interestingRelProperties.Contains(item))
                {
                    List<RelProperty> list;
                    EntityTypeBase elementType = ((RefType) fromEnd.TypeUsage.EdmType).ElementType;
                    if (!this._relPropertyMap.TryGetValue(elementType, out list))
                    {
                        list = new List<RelProperty>();
                        this._relPropertyMap[elementType] = list;
                    }
                    list.Add(item);
                }
            }
        }

        internal IEnumerable<RelProperty> GetDeclaredOnlyRelProperties(EntityTypeBase entityType)
        {
            List<RelProperty> iteratorVariable0;
            if (this._relPropertyMap.TryGetValue(entityType, out iteratorVariable0))
            {
                foreach (RelProperty iteratorVariable1 in iteratorVariable0)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        internal IEnumerable<RelProperty> GetRelProperties(EntityTypeBase entityType)
        {
            if (entityType.BaseType != null)
            {
                foreach (RelProperty iteratorVariable0 in this.GetRelProperties(entityType.BaseType as EntityTypeBase))
                {
                    yield return iteratorVariable0;
                }
            }
            IEnumerator<RelProperty> enumerator = this.GetDeclaredOnlyRelProperties(entityType).GetEnumerator();
            while (enumerator.MoveNext())
            {
                RelProperty current = enumerator.Current;
                yield return current;
            }
        }

        private void ProcessRelationship(RelationshipType relationshipType)
        {
            AssociationType associationType = relationshipType as AssociationType;
            if ((associationType != null) && (associationType.AssociationEndMembers.Count == 2))
            {
                AssociationEndMember fromEnd = associationType.AssociationEndMembers[0];
                AssociationEndMember toEnd = associationType.AssociationEndMembers[1];
                this.AddRelProperty(associationType, fromEnd, toEnd);
                this.AddRelProperty(associationType, toEnd, fromEnd);
            }
        }


    }
}

