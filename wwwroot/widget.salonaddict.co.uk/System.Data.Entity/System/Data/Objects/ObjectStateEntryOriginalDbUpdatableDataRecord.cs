namespace System.Data.Objects
{
    using System;
    using System.Data;

    internal sealed class ObjectStateEntryOriginalDbUpdatableDataRecord : CurrentValueRecord
    {
        internal ObjectStateEntryOriginalDbUpdatableDataRecord(ObjectStateEntry cacheEntry, StateManagerTypeMetadata metadata, object userObject) : base(cacheEntry, metadata, userObject)
        {
            EntityUtil.CheckArgumentNull<ObjectStateEntry>(cacheEntry, "cacheEntry");
            EntityUtil.CheckArgumentNull<object>(userObject, "userObject");
            EntityUtil.CheckArgumentNull<StateManagerTypeMetadata>(metadata, "metadata");
            EntityState state = cacheEntry.State;
            if (((state != EntityState.Unchanged) && (state != EntityState.Deleted)) && (state != EntityState.Modified))
            {
                throw EntityUtil.CannotCreateObjectStateEntryOriginalDbUpdatableDataRecord();
            }
        }

        protected override object GetRecordValue(int ordinal) => 
            base._cacheEntry.GetOriginalEntityValue(base._metadata, ordinal, base._userObject, ObjectStateValueRecord.OriginalUpdatable);

        protected override void SetRecordValue(int ordinal, object value)
        {
            base._cacheEntry.SetOriginalEntityValue(base._metadata, ordinal, base._userObject, value);
        }
    }
}

