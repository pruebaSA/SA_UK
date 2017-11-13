namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal class ConstraintManager
    {
        private Dictionary<EntityContainer, EntityContainer> m_entityContainerMap = new Dictionary<EntityContainer, EntityContainer>();
        private Dictionary<ExtentPair, List<ForeignKeyConstraint>> m_parentChildRelationships = new Dictionary<ExtentPair, List<ForeignKeyConstraint>>();

        internal ConstraintManager()
        {
        }

        private static bool IsBinary(RelationshipType relationshipType)
        {
            int num = 0;
            foreach (EdmMember member in relationshipType.Members)
            {
                if (member is RelationshipEndMember)
                {
                    num++;
                    if (num > 2)
                    {
                        return false;
                    }
                }
            }
            return (num == 2);
        }

        internal bool IsParentChildRelationship(EntitySetBase table1, EntitySetBase table2, out List<ForeignKeyConstraint> constraints)
        {
            this.LoadRelationships(table1.EntityContainer);
            this.LoadRelationships(table2.EntityContainer);
            ExtentPair key = new ExtentPair(table1, table2);
            return this.m_parentChildRelationships.TryGetValue(key, out constraints);
        }

        internal void LoadRelationships(EntityContainer entityContainer)
        {
            if (!this.m_entityContainerMap.ContainsKey(entityContainer))
            {
                foreach (EntitySetBase base2 in entityContainer.BaseEntitySets)
                {
                    RelationshipSet relationshipSet = base2 as RelationshipSet;
                    if (relationshipSet != null)
                    {
                        RelationshipType elementType = relationshipSet.ElementType;
                        AssociationType type2 = elementType as AssociationType;
                        if ((type2 != null) && IsBinary(elementType))
                        {
                            foreach (ReferentialConstraint constraint in type2.ReferentialConstraints)
                            {
                                List<ForeignKeyConstraint> list;
                                ForeignKeyConstraint item = new ForeignKeyConstraint(elementType, relationshipSet, constraint);
                                if (!this.m_parentChildRelationships.TryGetValue(item.Pair, out list))
                                {
                                    list = new List<ForeignKeyConstraint>();
                                    this.m_parentChildRelationships[item.Pair] = list;
                                }
                                list.Add(item);
                            }
                        }
                    }
                }
                this.m_entityContainerMap[entityContainer] = entityContainer;
            }
        }
    }
}

