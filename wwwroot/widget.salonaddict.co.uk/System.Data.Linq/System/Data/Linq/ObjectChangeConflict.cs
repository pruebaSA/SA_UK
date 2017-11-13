namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq.Mapping;
    using System.Linq;

    public sealed class ObjectChangeConflict
    {
        private object database;
        private bool? isDeleted;
        private bool isResolved;
        private ReadOnlyCollection<MemberChangeConflict> memberConflicts;
        private object original;
        private ChangeConflictSession session;
        private System.Data.Linq.TrackedObject trackedObject;

        internal ObjectChangeConflict(ChangeConflictSession session, System.Data.Linq.TrackedObject trackedObject)
        {
            this.session = session;
            this.trackedObject = trackedObject;
            this.original = trackedObject.CreateDataCopy(trackedObject.Original);
        }

        internal ObjectChangeConflict(ChangeConflictSession session, System.Data.Linq.TrackedObject trackedObject, bool isDeleted) : this(session, trackedObject)
        {
            this.isDeleted = new bool?(isDeleted);
        }

        private bool AreEqual(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }
            int index = 0;
            int length = a1.Length;
            while (index < length)
            {
                if (a1[index] != a2[index])
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private bool AreEqual(char[] a1, char[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }
            int index = 0;
            int length = a1.Length;
            while (index < length)
            {
                if (a1[index] != a2[index])
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private bool AreEqual(MetaDataMember member, object v1, object v2)
        {
            if ((v1 == null) && (v2 == null))
            {
                return true;
            }
            if ((v1 == null) || (v2 == null))
            {
                return false;
            }
            if (member.Type == typeof(char[]))
            {
                return this.AreEqual((char[]) v1, (char[]) v2);
            }
            if (member.Type == typeof(byte[]))
            {
                return this.AreEqual((byte[]) v1, (byte[]) v2);
            }
            return object.Equals(v1, v2);
        }

        private bool HasMemberConflict(MetaDataMember member)
        {
            object boxedValue = member.StorageAccessor.GetBoxedValue(this.original);
            if (!member.DeclaringType.Type.IsAssignableFrom(this.database.GetType()))
            {
                return false;
            }
            object obj3 = member.StorageAccessor.GetBoxedValue(this.database);
            return !this.AreEqual(member, boxedValue, obj3);
        }

        internal void OnMemberResolved()
        {
            if (!this.IsResolved && (this.memberConflicts.AsEnumerable<MemberChangeConflict>().Count<MemberChangeConflict>(m => m.IsResolved) == this.memberConflicts.Count))
            {
                this.Resolve(RefreshMode.KeepCurrentValues, false);
            }
        }

        public void Resolve()
        {
            this.Resolve(RefreshMode.KeepCurrentValues, true);
        }

        public void Resolve(RefreshMode refreshMode)
        {
            this.Resolve(refreshMode, false);
        }

        public void Resolve(RefreshMode refreshMode, bool autoResolveDeletes)
        {
            if (autoResolveDeletes && this.IsDeleted)
            {
                this.ResolveDelete();
            }
            else
            {
                if (this.Database == null)
                {
                    throw System.Data.Linq.Error.RefreshOfDeletedObject();
                }
                this.trackedObject.Refresh(refreshMode, this.Database);
                this.isResolved = true;
            }
        }

        private void ResolveDelete()
        {
            if (!this.trackedObject.IsDeleted)
            {
                this.trackedObject.ConvertToDeleted();
            }
            this.trackedObject.AcceptChanges();
            this.isResolved = true;
        }

        internal object Database
        {
            get
            {
                if (this.database == null)
                {
                    DataContext refreshContext = this.session.RefreshContext;
                    object[] keyValues = CommonDataServices.GetKeyValues(this.trackedObject.Type, this.original);
                    this.database = refreshContext.Services.GetObjectByKey(this.trackedObject.Type, keyValues);
                }
                return this.database;
            }
        }

        public bool IsDeleted
        {
            get
            {
                if (this.isDeleted.HasValue)
                {
                    return this.isDeleted.Value;
                }
                return (this.Database == null);
            }
        }

        public bool IsResolved =>
            this.isResolved;

        public ReadOnlyCollection<MemberChangeConflict> MemberConflicts
        {
            get
            {
                if (this.memberConflicts == null)
                {
                    List<MemberChangeConflict> list = new List<MemberChangeConflict>();
                    if (this.Database != null)
                    {
                        foreach (MetaDataMember member in this.trackedObject.Type.PersistentDataMembers)
                        {
                            if (!member.IsAssociation && this.HasMemberConflict(member))
                            {
                                list.Add(new MemberChangeConflict(this, member));
                            }
                        }
                    }
                    this.memberConflicts = list.AsReadOnly();
                }
                return this.memberConflicts;
            }
        }

        public object Object =>
            this.trackedObject.Current;

        internal object Original =>
            this.original;

        internal ChangeConflictSession Session =>
            this.session;

        internal System.Data.Linq.TrackedObject TrackedObject =>
            this.trackedObject;
    }
}

