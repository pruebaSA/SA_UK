namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Linq;

    internal class AssociationSetMetadata
    {
        internal readonly System.Data.Common.Utils.Set<AssociationEndMember> IncludedValueEnds;
        internal readonly System.Data.Common.Utils.Set<AssociationEndMember> OptionalEnds;
        internal readonly System.Data.Common.Utils.Set<AssociationEndMember> RequiredEnds;

        internal AssociationSetMetadata(IEnumerable<AssociationEndMember> requiredEnds)
        {
            if (requiredEnds.Any<AssociationEndMember>())
            {
                this.RequiredEnds = new System.Data.Common.Utils.Set<AssociationEndMember>(requiredEnds);
            }
            FixSet(ref this.RequiredEnds);
            FixSet(ref this.OptionalEnds);
            FixSet(ref this.IncludedValueEnds);
        }

        internal AssociationSetMetadata(System.Data.Common.Utils.Set<EntitySet> affectedTables, AssociationSet associationSet, MetadataWorkspace workspace)
        {
            bool flag = 1 < affectedTables.Count;
            ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = associationSet.AssociationSetEnds;
            foreach (EntitySet set in affectedTables)
            {
                foreach (EntitySet set2 in EntitySetRetriever.GetEntitySets(workspace.GetCqtView(set).Query))
                {
                    foreach (AssociationSetEnd end in associationSetEnds)
                    {
                        if (end.EntitySet.EdmEquals(set2))
                        {
                            if (flag)
                            {
                                AddEnd(ref this.RequiredEnds, end.CorrespondingAssociationEndMember);
                            }
                            else if ((this.RequiredEnds == null) || !this.RequiredEnds.Contains(end.CorrespondingAssociationEndMember))
                            {
                                AddEnd(ref this.OptionalEnds, end.CorrespondingAssociationEndMember);
                            }
                        }
                    }
                }
            }
            FixSet(ref this.RequiredEnds);
            FixSet(ref this.OptionalEnds);
            foreach (ReferentialConstraint constraint in associationSet.ElementType.ReferentialConstraints)
            {
                AssociationEndMember fromRole = (AssociationEndMember) constraint.FromRole;
                if (!this.RequiredEnds.Contains(fromRole) && !this.OptionalEnds.Contains(fromRole))
                {
                    AddEnd(ref this.IncludedValueEnds, fromRole);
                }
            }
            FixSet(ref this.IncludedValueEnds);
        }

        private static void AddEnd(ref System.Data.Common.Utils.Set<AssociationEndMember> set, AssociationEndMember element)
        {
            if (set == null)
            {
                set = new System.Data.Common.Utils.Set<AssociationEndMember>();
            }
            set.Add(element);
        }

        private static void FixSet(ref System.Data.Common.Utils.Set<AssociationEndMember> set)
        {
            if (set == null)
            {
                set = System.Data.Common.Utils.Set<AssociationEndMember>.Empty;
            }
            else
            {
                set.MakeReadOnly();
            }
        }

        internal bool HasEnds
        {
            get
            {
                if ((0 >= this.RequiredEnds.Count) && (0 >= this.OptionalEnds.Count))
                {
                    return (0 < this.IncludedValueEnds.Count);
                }
                return true;
            }
        }
    }
}

