namespace System.Data.Objects
{
    using System;
    using System.Data;

    internal sealed class ObjectStateEntryDbUpdatableDataRecord : CurrentValueRecord
    {
        internal ObjectStateEntryDbUpdatableDataRecord(ObjectStateEntry cacheEntry) : base(cacheEntry)
        {
            EntityUtil.CheckArgumentNull<ObjectStateEntry>(cacheEntry, "cacheEntry");
            switch (cacheEntry.State)
            {
                case EntityState.Unchanged:
                case EntityState.Added:
                case EntityState.Modified:
                    return;
            }
            throw EntityUtil.CannotCreateObjectStateEntryDbUpdatableDataRecord();
        }

        internal ObjectStateEntryDbUpdatableDataRecord(ObjectStateEntry cacheEntry, StateManagerTypeMetadata metadata, object userObject) : base(cacheEntry, metadata, userObject)
        {
            EntityUtil.CheckArgumentNull<ObjectStateEntry>(cacheEntry, "cacheEntry");
            EntityUtil.CheckArgumentNull<object>(userObject, "userObject");
            EntityUtil.CheckArgumentNull<StateManagerTypeMetadata>(metadata, "metadata");
            switch (cacheEntry.State)
            {
                case EntityState.Unchanged:
                case EntityState.Added:
                case EntityState.Modified:
                    return;
            }
            throw EntityUtil.CannotCreateObjectStateEntryDbUpdatableDataRecord();
        }

        protected override object GetRecordValue(int ordinal)
        {
            if (base._cacheEntry.IsRelationship)
            {
                return base._cacheEntry.GetCurrentRelationValue(ordinal);
            }
            return base._cacheEntry.GetCurrentEntityValue(base._metadata, ordinal, base._userObject, ObjectStateValueRecord.CurrentUpdatable);
        }

        protected override void SetRecordValue(int ordinal, object value)
        {
            if (base._cacheEntry.IsRelationship)
            {
                throw EntityUtil.CantModifyRelationValues();
            }
            base._cacheEntry.SetCurrentEntityValue(base._metadata, ordinal, base._userObject, value);
        }
    }
}

