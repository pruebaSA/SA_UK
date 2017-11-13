namespace System.Data.Objects
{
    using System;
    using System.Data;

    internal sealed class RelationshipEntry : ObjectStateEntry
    {
        private RelationshipEntry _nextKey0;
        private RelationshipEntry _nextKey1;

        internal RelationshipEntry(ObjectStateManager cache, EntityState state, RelationshipWrapper wrapper) : base(cache, state, wrapper)
        {
        }

        internal void ChangeRelatedEnd(EntityKey oldKey, EntityKey newKey)
        {
            if (oldKey.Equals(this.Key0))
            {
                if (oldKey.Equals(this.Key1))
                {
                    base.Wrapper = new RelationshipWrapper(base.Wrapper.AssociationSet, newKey);
                }
                else
                {
                    base.Wrapper = new RelationshipWrapper(base.Wrapper, 0, newKey);
                }
            }
            else
            {
                base.Wrapper = new RelationshipWrapper(base.Wrapper, 1, newKey);
            }
        }

        internal RelationshipEntry GetNextRelationshipEnd(EntityKey entityKey)
        {
            if (!entityKey.Equals(this.Key0))
            {
                return this.NextKey1;
            }
            return this.NextKey0;
        }

        internal void SetNextRelationshipEnd(EntityKey entityKey, RelationshipEntry nextEnd)
        {
            if (entityKey.Equals(this.Key0))
            {
                this.NextKey0 = nextEnd;
            }
            else
            {
                this.NextKey1 = nextEnd;
            }
        }

        internal EntityKey Key0 =>
            base.Wrapper.Key0;

        internal EntityKey Key1 =>
            base.Wrapper.Key1;

        internal RelationshipEntry NextKey0
        {
            get => 
                this._nextKey0;
            set
            {
                this._nextKey0 = value;
            }
        }

        internal RelationshipEntry NextKey1
        {
            get => 
                this._nextKey1;
            set
            {
                this._nextKey1 = value;
            }
        }
    }
}

