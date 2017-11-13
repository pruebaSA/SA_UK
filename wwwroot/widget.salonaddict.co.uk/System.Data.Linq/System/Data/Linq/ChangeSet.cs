namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

    public sealed class ChangeSet
    {
        private ReadOnlyCollection<object> deletes;
        private ReadOnlyCollection<object> inserts;
        private ReadOnlyCollection<object> updates;

        internal ChangeSet(ReadOnlyCollection<object> inserts, ReadOnlyCollection<object> deletes, ReadOnlyCollection<object> updates)
        {
            this.inserts = inserts;
            this.deletes = deletes;
            this.updates = updates;
        }

        public override string ToString() => 
            ("{" + string.Format(CultureInfo.InvariantCulture, "Inserts: {0}, Deletes: {1}, Updates: {2}", new object[] { this.Inserts.Count, this.Deletes.Count, this.Updates.Count }) + "}");

        public IList<object> Deletes =>
            this.deletes;

        public IList<object> Inserts =>
            this.inserts;

        public IList<object> Updates =>
            this.updates;
    }
}

