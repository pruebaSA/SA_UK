namespace System.Data.Linq
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Reflection;

    public sealed class MemberChangeConflict
    {
        private ObjectChangeConflict conflict;
        private object currentValue;
        private object databaseValue;
        private bool isResolved;
        private MetaDataMember metaMember;
        private object originalValue;

        internal MemberChangeConflict(ObjectChangeConflict conflict, MetaDataMember metaMember)
        {
            this.conflict = conflict;
            this.metaMember = metaMember;
            this.originalValue = metaMember.StorageAccessor.GetBoxedValue(conflict.Original);
            this.databaseValue = metaMember.StorageAccessor.GetBoxedValue(conflict.Database);
            this.currentValue = metaMember.StorageAccessor.GetBoxedValue(conflict.TrackedObject.Current);
        }

        public void Resolve(RefreshMode refreshMode)
        {
            this.conflict.TrackedObject.RefreshMember(this.metaMember, refreshMode, this.databaseValue);
            this.isResolved = true;
            this.conflict.OnMemberResolved();
        }

        public void Resolve(object value)
        {
            this.conflict.TrackedObject.RefreshMember(this.metaMember, RefreshMode.OverwriteCurrentValues, value);
            this.isResolved = true;
            this.conflict.OnMemberResolved();
        }

        public object CurrentValue =>
            this.currentValue;

        public object DatabaseValue =>
            this.databaseValue;

        public bool IsModified =>
            this.conflict.TrackedObject.HasChangedValue(this.metaMember);

        public bool IsResolved =>
            this.isResolved;

        public MemberInfo Member =>
            this.metaMember.Member;

        public object OriginalValue =>
            this.originalValue;
    }
}

