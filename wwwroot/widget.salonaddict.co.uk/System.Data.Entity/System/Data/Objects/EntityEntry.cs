namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal sealed class EntityEntry : ObjectStateEntry
    {
        private int _countRelationshipEnds;
        private RelationshipEntry _headRelationshipEnds;

        internal EntityEntry(EntityKey entityKey, EntitySet extent, ObjectStateManager cache, StateManagerTypeMetadata typeMetadata) : base(entityKey, extent, cache, typeMetadata)
        {
        }

        internal EntityEntry(object entity, EntityKey entityKey, EntitySet extent, ObjectStateManager cache, StateManagerTypeMetadata typeMetadata, bool isAdded) : base(entity, entityKey, extent, cache, typeMetadata, isAdded)
        {
        }

        internal void AddRelationshipEnd(RelationshipEntry item)
        {
            item.SetNextRelationshipEnd(base.EntityKey, this._headRelationshipEnds);
            this._headRelationshipEnds = item;
            this._countRelationshipEnds++;
        }

        internal bool ContainsRelationshipEnd(RelationshipEntry item)
        {
            for (RelationshipEntry entry = this._headRelationshipEnds; entry != null; entry = entry.GetNextRelationshipEnd(base.EntityKey))
            {
                if (object.ReferenceEquals(entry, item))
                {
                    return true;
                }
            }
            return false;
        }

        internal RelationshipEndEnumerable GetRelationshipEnds() => 
            new RelationshipEndEnumerable(this);

        internal void RemoveRelationshipEnd(RelationshipEntry item)
        {
            RelationshipEntry objB = this._headRelationshipEnds;
            RelationshipEntry entry2 = null;
            while (objB != null)
            {
                if (object.ReferenceEquals(item, objB))
                {
                    if (base.EntityKey.Equals(objB.Key0))
                    {
                        if (entry2 == null)
                        {
                            this._headRelationshipEnds = objB.NextKey0;
                        }
                        else
                        {
                            entry2.SetNextRelationshipEnd(base.EntityKey, objB.NextKey0);
                        }
                        objB.NextKey0 = null;
                    }
                    else
                    {
                        if (entry2 == null)
                        {
                            this._headRelationshipEnds = objB.NextKey1;
                        }
                        else
                        {
                            entry2.SetNextRelationshipEnd(base.EntityKey, objB.NextKey1);
                        }
                        objB.NextKey1 = null;
                    }
                    this._countRelationshipEnds--;
                    return;
                }
                entry2 = objB;
                objB = objB.GetNextRelationshipEnd(base.EntityKey);
            }
        }

        internal void UpdateRelationshipEnds(EntityKey oldKey, EntityEntry promotedEntry)
        {
            int num = 0;
            RelationshipEntry nextRelationshipEnd = this._headRelationshipEnds;
            while (nextRelationshipEnd != null)
            {
                RelationshipEntry item = nextRelationshipEnd;
                nextRelationshipEnd = nextRelationshipEnd.GetNextRelationshipEnd(oldKey);
                item.ChangeRelatedEnd(oldKey, base.EntityKey);
                if ((promotedEntry != null) && !promotedEntry.ContainsRelationshipEnd(item))
                {
                    promotedEntry.AddRelationshipEnd(item);
                }
                num++;
            }
            if (promotedEntry != null)
            {
                this._headRelationshipEnds = null;
                this._countRelationshipEnds = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RelationshipEndEnumerable : IEnumerable<RelationshipEntry>, IEnumerable<IEntityStateEntry>, IEnumerable
        {
            internal static readonly RelationshipEntry[] EmptyRelationshipEntryArray;
            private readonly EntityEntry _entityEntry;
            internal RelationshipEndEnumerable(EntityEntry entityEntry)
            {
                this._entityEntry = entityEntry;
            }

            public EntityEntry.RelationshipEndEnumerator GetEnumerator() => 
                new EntityEntry.RelationshipEndEnumerator(this._entityEntry);

            IEnumerator<IEntityStateEntry> IEnumerable<IEntityStateEntry>.GetEnumerator() => 
                this.GetEnumerator();

            IEnumerator<RelationshipEntry> IEnumerable<RelationshipEntry>.GetEnumerator() => 
                this.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            internal RelationshipEntry[] ToArray()
            {
                RelationshipEntry[] entryArray = null;
                if ((this._entityEntry != null) && (0 < this._entityEntry._countRelationshipEnds))
                {
                    RelationshipEntry nextRelationshipEnd = this._entityEntry._headRelationshipEnds;
                    entryArray = new RelationshipEntry[this._entityEntry._countRelationshipEnds];
                    for (int i = 0; i < entryArray.Length; i++)
                    {
                        entryArray[i] = nextRelationshipEnd;
                        nextRelationshipEnd = nextRelationshipEnd.GetNextRelationshipEnd(this._entityEntry.EntityKey);
                    }
                }
                return (entryArray ?? EmptyRelationshipEntryArray);
            }

            static RelationshipEndEnumerable()
            {
                EmptyRelationshipEntryArray = new RelationshipEntry[0];
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RelationshipEndEnumerator : IEnumerator<RelationshipEntry>, IEnumerator<IEntityStateEntry>, IDisposable, IEnumerator
        {
            private readonly EntityEntry _entityEntry;
            private RelationshipEntry _current;
            internal RelationshipEndEnumerator(EntityEntry entityEntry)
            {
                this._entityEntry = entityEntry;
                this._current = null;
            }

            public RelationshipEntry Current =>
                this._current;
            IEntityStateEntry IEnumerator<IEntityStateEntry>.Current =>
                this._current;
            object IEnumerator.Current =>
                this._current;
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this._entityEntry != null)
                {
                    if (this._current == null)
                    {
                        this._current = this._entityEntry._headRelationshipEnds;
                    }
                    else
                    {
                        this._current = this._current.GetNextRelationshipEnd(this._entityEntry.EntityKey);
                    }
                }
                return (null != this._current);
            }

            public void Reset()
            {
            }
        }
    }
}

