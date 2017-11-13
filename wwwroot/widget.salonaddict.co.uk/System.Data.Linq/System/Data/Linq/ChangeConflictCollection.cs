namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class ChangeConflictCollection : ICollection<ObjectChangeConflict>, ICollection, IEnumerable<ObjectChangeConflict>, IEnumerable
    {
        private List<ObjectChangeConflict> conflicts = new List<ObjectChangeConflict>();

        internal ChangeConflictCollection()
        {
        }

        public void Clear()
        {
            this.conflicts.Clear();
        }

        public bool Contains(ObjectChangeConflict item) => 
            this.conflicts.Contains(item);

        public void CopyTo(ObjectChangeConflict[] array, int arrayIndex)
        {
            this.conflicts.CopyTo(array, arrayIndex);
        }

        internal void Fill(List<ObjectChangeConflict> conflictList)
        {
            this.conflicts = conflictList;
        }

        public IEnumerator<ObjectChangeConflict> GetEnumerator() => 
            this.conflicts.GetEnumerator();

        public bool Remove(ObjectChangeConflict item) => 
            this.conflicts.Remove(item);

        public void ResolveAll(RefreshMode mode)
        {
            this.ResolveAll(mode, true);
        }

        public void ResolveAll(RefreshMode mode, bool autoResolveDeletes)
        {
            foreach (ObjectChangeConflict conflict in this.conflicts)
            {
                if (!conflict.IsResolved)
                {
                    conflict.Resolve(mode, autoResolveDeletes);
                }
            }
        }

        void ICollection<ObjectChangeConflict>.Add(ObjectChangeConflict item)
        {
            throw Error.CannotAddChangeConflicts();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.conflicts.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.conflicts.GetEnumerator();

        public int Count =>
            this.conflicts.Count;

        public ObjectChangeConflict this[int index] =>
            this.conflicts[index];

        bool ICollection<ObjectChangeConflict>.IsReadOnly =>
            true;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            null;
    }
}

