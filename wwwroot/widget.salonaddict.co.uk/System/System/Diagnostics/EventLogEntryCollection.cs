﻿namespace System.Diagnostics
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class EventLogEntryCollection : ICollection, IEnumerable
    {
        private EventLog log;

        internal EventLogEntryCollection(EventLog log)
        {
            this.log = log;
        }

        public void CopyTo(EventLogEntry[] entries, int index)
        {
            ((ICollection) this).CopyTo(entries, index);
        }

        internal EventLogEntry GetEntryAtNoThrow(int index) => 
            this.log.GetEntryAtNoThrow(index);

        public IEnumerator GetEnumerator() => 
            new EntriesEnumerator(this);

        void ICollection.CopyTo(Array array, int index)
        {
            EventLogEntry[] allEntries = this.log.GetAllEntries();
            Array.Copy(allEntries, 0, array, index, allEntries.Length);
        }

        public int Count =>
            this.log.EntryCount;

        public virtual EventLogEntry this[int index] =>
            this.log.GetEntryAt(index);

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;

        private class EntriesEnumerator : IEnumerator
        {
            private EventLogEntry cachedEntry;
            private EventLogEntryCollection entries;
            private int num = -1;

            internal EntriesEnumerator(EventLogEntryCollection entries)
            {
                this.entries = entries;
            }

            public bool MoveNext()
            {
                this.num++;
                this.cachedEntry = this.entries.GetEntryAtNoThrow(this.num);
                return (this.cachedEntry != null);
            }

            public void Reset()
            {
                this.num = -1;
            }

            public object Current
            {
                get
                {
                    if (this.cachedEntry == null)
                    {
                        throw new InvalidOperationException(SR.GetString("NoCurrentEntry"));
                    }
                    return this.cachedEntry;
                }
            }
        }
    }
}

