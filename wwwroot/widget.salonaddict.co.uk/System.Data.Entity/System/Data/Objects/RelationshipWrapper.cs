namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Metadata.Edm;

    internal sealed class RelationshipWrapper : IEquatable<RelationshipWrapper>
    {
        internal readonly System.Data.Metadata.Edm.AssociationSet AssociationSet;
        internal readonly EntityKey Key0;
        internal readonly EntityKey Key1;

        internal RelationshipWrapper(System.Data.Metadata.Edm.AssociationSet extent, EntityKey key)
        {
            this.AssociationSet = extent;
            this.Key0 = key;
            this.Key1 = key;
        }

        internal RelationshipWrapper(System.Data.Metadata.Edm.AssociationSet extent, KeyValuePair<string, EntityKey> roleAndKey1, KeyValuePair<string, EntityKey> roleAndKey2) : this(extent, roleAndKey1.Key, roleAndKey1.Value, roleAndKey2.Key, roleAndKey2.Value)
        {
        }

        internal RelationshipWrapper(RelationshipWrapper wrapper, int ordinal, EntityKey key)
        {
            this.AssociationSet = wrapper.AssociationSet;
            this.Key0 = (ordinal == 0) ? key : wrapper.Key0;
            this.Key1 = (ordinal == 0) ? wrapper.Key1 : key;
        }

        internal RelationshipWrapper(System.Data.Metadata.Edm.AssociationSet extent, string role0, EntityKey key0, string role1, EntityKey key1)
        {
            this.AssociationSet = extent;
            if (extent.ElementType.AssociationEndMembers[0].Name == role0)
            {
                this.Key0 = key0;
                this.Key1 = key1;
            }
            else
            {
                this.Key0 = key1;
                this.Key1 = key0;
            }
        }

        public bool Equals(RelationshipWrapper wrapper) => 
            (object.ReferenceEquals(this, wrapper) || ((((wrapper != null) && object.ReferenceEquals(this.AssociationSet, wrapper.AssociationSet)) && this.Key0.Equals(wrapper.Key0)) && this.Key1.Equals(wrapper.Key1)));

        public override bool Equals(object obj) => 
            this.Equals(obj as RelationshipWrapper);

        internal AssociationEndMember GetAssociationEndMember(EntityKey key) => 
            this.AssociationEndMembers[(this.Key0 != key) ? 1 : 0];

        internal EntityKey GetEntityKey(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return this.Key0;

                case 1:
                    return this.Key1;
            }
            throw EntityUtil.ArgumentOutOfRange("ordinal");
        }

        public override int GetHashCode() => 
            (this.AssociationSet.Name.GetHashCode() ^ (this.Key0.GetHashCode() + this.Key1.GetHashCode()));

        internal EntityKey GetOtherEntityKey(EntityKey key)
        {
            if (this.Key0 == key)
            {
                return this.Key1;
            }
            if (!(this.Key1 == key))
            {
                return null;
            }
            return this.Key0;
        }

        internal ReadOnlyMetadataCollection<AssociationEndMember> AssociationEndMembers =>
            this.AssociationSet.ElementType.AssociationEndMembers;
    }
}

