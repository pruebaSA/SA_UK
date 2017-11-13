namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal abstract class ChangeTracker
    {
        protected ChangeTracker()
        {
        }

        internal abstract void AcceptChanges();
        internal static ChangeTracker CreateChangeTracker(CommonDataServices dataServices, bool asReadOnly)
        {
            if (asReadOnly)
            {
                return new ReadOnlyChangeTracker();
            }
            return new StandardChangeTracker(dataServices);
        }

        internal abstract void FastTrack(object obj);
        internal abstract IEnumerable<TrackedObject> GetInterestingObjects();
        internal abstract TrackedObject GetTrackedObject(object obj);
        internal abstract bool IsTracked(object obj);
        internal abstract void StopTracking(object obj);
        internal abstract TrackedObject Track(object obj);
        internal abstract TrackedObject Track(object obj, bool recurse);

        private class ReadOnlyChangeTracker : ChangeTracker
        {
            internal override void AcceptChanges()
            {
            }

            internal override void FastTrack(object obj)
            {
            }

            internal override IEnumerable<TrackedObject> GetInterestingObjects() => 
                new TrackedObject[0];

            internal override TrackedObject GetTrackedObject(object obj) => 
                null;

            internal override bool IsTracked(object obj) => 
                false;

            internal override void StopTracking(object obj)
            {
            }

            internal override TrackedObject Track(object obj) => 
                null;

            internal override TrackedObject Track(object obj, bool recurse) => 
                null;
        }

        private class StandardChangeTracker : ChangeTracker
        {
            private Dictionary<object, StandardTrackedObject> items;
            private PropertyChangingEventHandler onPropertyChanging;
            private CommonDataServices services;

            internal StandardChangeTracker(CommonDataServices services)
            {
                this.services = services;
                this.items = new Dictionary<object, StandardTrackedObject>();
                this.onPropertyChanging = new PropertyChangingEventHandler(this.OnPropertyChanging);
            }

            internal override void AcceptChanges()
            {
                List<StandardTrackedObject> list = new List<StandardTrackedObject>(this.items.Values);
                foreach (TrackedObject obj2 in list)
                {
                    obj2.AcceptChanges();
                }
            }

            private void Attach(object obj)
            {
                INotifyPropertyChanging changing = obj as INotifyPropertyChanging;
                if (changing != null)
                {
                    changing.PropertyChanging += this.onPropertyChanging;
                }
                else
                {
                    this.OnPropertyChanging(obj, null);
                }
            }

            private void Detach(object obj)
            {
                INotifyPropertyChanging changing = obj as INotifyPropertyChanging;
                if (changing != null)
                {
                    changing.PropertyChanging -= this.onPropertyChanging;
                }
            }

            internal override void FastTrack(object obj)
            {
                this.Attach(obj);
            }

            internal override IEnumerable<TrackedObject> GetInterestingObjects()
            {
                foreach (StandardTrackedObject iteratorVariable0 in this.items.Values)
                {
                    if (iteratorVariable0.IsInteresting)
                    {
                        yield return iteratorVariable0;
                    }
                }
            }

            internal override TrackedObject GetTrackedObject(object obj)
            {
                StandardTrackedObject obj2;
                if (!this.items.TryGetValue(obj, out obj2) && this.IsFastTracked(obj))
                {
                    return this.PromoteFastTrackedObject(obj);
                }
                return obj2;
            }

            private bool IsFastTracked(object obj)
            {
                MetaType rowType = this.services.Model.GetTable(obj.GetType()).RowType;
                return this.services.IsCachedObject(rowType, obj);
            }

            private static bool IsSameDiscriminator(object discriminator1, object discriminator2) => 
                ((discriminator1 == discriminator2) || (((discriminator1 != null) && (discriminator2 != null)) && discriminator1.Equals(discriminator2)));

            internal override bool IsTracked(object obj)
            {
                if (!this.items.ContainsKey(obj))
                {
                    return this.IsFastTracked(obj);
                }
                return true;
            }

            private void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
            {
                StandardTrackedObject obj2;
                if (this.items.TryGetValue(sender, out obj2))
                {
                    obj2.StartTracking();
                }
                else if (this.IsFastTracked(sender))
                {
                    this.PromoteFastTrackedObject(sender).StartTracking();
                }
            }

            private StandardTrackedObject PromoteFastTrackedObject(object obj)
            {
                Type type = obj.GetType();
                MetaType inheritanceType = this.services.Model.GetTable(type).RowType.GetInheritanceType(type);
                return this.PromoteFastTrackedObject(inheritanceType, obj);
            }

            private StandardTrackedObject PromoteFastTrackedObject(MetaType type, object obj)
            {
                StandardTrackedObject obj2 = new StandardTrackedObject(this, type, obj, obj);
                this.items.Add(obj, obj2);
                return obj2;
            }

            internal override void StopTracking(object obj)
            {
                this.Detach(obj);
                this.items.Remove(obj);
            }

            internal override TrackedObject Track(object obj) => 
                this.Track(obj, false);

            internal override TrackedObject Track(object obj, bool recurse)
            {
                MetaType metaType = this.services.Model.GetMetaType(obj.GetType());
                Dictionary<object, object> visited = new Dictionary<object, object>();
                return this.Track(metaType, obj, visited, recurse, 1);
            }

            private TrackedObject Track(MetaType mt, object obj, Dictionary<object, object> visited, bool recurse, int level)
            {
                StandardTrackedObject trackedObject = (StandardTrackedObject) this.GetTrackedObject(obj);
                if ((trackedObject == null) && !visited.ContainsKey(obj))
                {
                    bool isWeaklyTracked = level > 1;
                    trackedObject = new StandardTrackedObject(this, mt, obj, obj, isWeaklyTracked);
                    if (trackedObject.HasDeferredLoaders)
                    {
                        throw System.Data.Linq.Error.CannotAttachAddNonNewEntities();
                    }
                    this.items.Add(obj, trackedObject);
                    this.Attach(obj);
                    visited.Add(obj, obj);
                    if (!recurse)
                    {
                        return trackedObject;
                    }
                    foreach (RelatedItem item in this.services.GetParents(mt, obj))
                    {
                        this.Track(item.Type, item.Item, visited, recurse, level + 1);
                    }
                    foreach (RelatedItem item2 in this.services.GetChildren(mt, obj))
                    {
                        this.Track(item2.Type, item2.Item, visited, recurse, level + 1);
                    }
                }
                return trackedObject;
            }

            private static MetaType TypeFromDiscriminator(MetaType root, object discriminator)
            {
                foreach (MetaType type in root.InheritanceTypes)
                {
                    if (IsSameDiscriminator(discriminator, type.InheritanceCode))
                    {
                        return type;
                    }
                }
                return root.InheritanceDefault;
            }


            private class StandardTrackedObject : TrackedObject
            {
                private object current;
                private BitArray dirtyMemberCache;
                private bool haveInitializedDeferredLoaders;
                private bool isWeaklyTracked;
                private object original;
                private State state;
                private ChangeTracker.StandardChangeTracker tracker;
                private MetaType type;

                internal StandardTrackedObject(ChangeTracker.StandardChangeTracker tracker, MetaType type, object current, object original)
                {
                    if (current == null)
                    {
                        throw System.Data.Linq.Error.ArgumentNull("current");
                    }
                    this.tracker = tracker;
                    this.type = type.GetInheritanceType(current.GetType());
                    this.current = current;
                    this.original = original;
                    this.state = State.PossiblyModified;
                    this.dirtyMemberCache = new BitArray(this.type.DataMembers.Count);
                }

                internal StandardTrackedObject(ChangeTracker.StandardChangeTracker tracker, MetaType type, object current, object original, bool isWeaklyTracked) : this(tracker, type, current, original)
                {
                    this.isWeaklyTracked = isWeaklyTracked;
                }

                internal override void AcceptChanges()
                {
                    if (this.IsWeaklyTracked)
                    {
                        this.InitializeDeferredLoaders();
                        this.isWeaklyTracked = false;
                    }
                    if (this.IsDeleted)
                    {
                        this.ConvertToDead();
                    }
                    else if (this.IsNew)
                    {
                        this.InitializeDeferredLoaders();
                        this.ConvertToUnmodified();
                    }
                    else if (this.IsPossiblyModified)
                    {
                        this.ConvertToUnmodified();
                    }
                }

                private void AssignMember(object instance, MetaDataMember mm, object value)
                {
                    if (!(this.current is INotifyPropertyChanging))
                    {
                        mm.StorageAccessor.SetBoxedValue(ref instance, value);
                    }
                    else
                    {
                        mm.MemberAccessor.SetBoxedValue(ref instance, value);
                    }
                }

                internal override bool CanInferDelete()
                {
                    if ((this.state == State.Modified) || (this.state == State.PossiblyModified))
                    {
                        foreach (MetaAssociation association in this.Type.Associations)
                        {
                            if (((association.DeleteOnNull && association.IsForeignKey) && (!association.IsNullable && !association.IsMany)) && (association.ThisMember.StorageAccessor.HasAssignedValue(this.Current) && (association.ThisMember.StorageAccessor.GetBoxedValue(this.Current) == null)))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                internal override void ConvertToDead()
                {
                    this.state = State.Dead;
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToDeleted()
                {
                    this.state = State.Deleted;
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToModified()
                {
                    this.state = State.Modified;
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToNew()
                {
                    this.original = null;
                    this.state = State.New;
                }

                internal override void ConvertToPossiblyModified()
                {
                    this.state = State.PossiblyModified;
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToPossiblyModified(object originalState)
                {
                    this.state = State.PossiblyModified;
                    this.original = this.CreateDataCopy(originalState);
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToRemoved()
                {
                    this.state = State.Removed;
                    this.isWeaklyTracked = false;
                }

                internal override void ConvertToUnmodified()
                {
                    this.state = State.PossiblyModified;
                    if (this.current is INotifyPropertyChanging)
                    {
                        this.original = this.current;
                    }
                    else
                    {
                        this.original = this.CreateDataCopy(this.current);
                    }
                    this.ResetDirtyMemberTracking();
                    this.isWeaklyTracked = false;
                }

                internal override object CreateDataCopy(object instance)
                {
                    System.Type type = instance.GetType();
                    object obj2 = Activator.CreateInstance(this.Type.Type);
                    foreach (MetaDataMember member in this.tracker.services.Model.GetTable(type).RowType.InheritanceRoot.GetInheritanceType(type).PersistentDataMembers)
                    {
                        if ((this.Type.Type == type) || member.DeclaringType.Type.IsAssignableFrom(type))
                        {
                            if (member.IsDeferred)
                            {
                                if (!member.IsAssociation)
                                {
                                    if (member.StorageAccessor.HasValue(instance))
                                    {
                                        object boxedValue = member.DeferredValueAccessor.GetBoxedValue(instance);
                                        member.DeferredValueAccessor.SetBoxedValue(ref obj2, boxedValue);
                                    }
                                    else
                                    {
                                        IEnumerable enumerable = this.tracker.services.GetDeferredSourceFactory(member).CreateDeferredSource(obj2);
                                        member.DeferredSourceAccessor.SetBoxedValue(ref obj2, enumerable);
                                    }
                                }
                            }
                            else
                            {
                                object obj4 = member.StorageAccessor.GetBoxedValue(instance);
                                member.StorageAccessor.SetBoxedValue(ref obj2, obj4);
                            }
                        }
                    }
                    return obj2;
                }

                private IEnumerable<MetaDataMember> GetAssociationsForKey(MetaDataMember key)
                {
                    foreach (MetaDataMember iteratorVariable0 in this.type.PersistentDataMembers)
                    {
                        if (iteratorVariable0.IsAssociation && iteratorVariable0.Association.ThisKey.Contains(key))
                        {
                            yield return iteratorVariable0;
                        }
                    }
                }

                internal override IEnumerable<ModifiedMemberInfo> GetModifiedMembers()
                {
                    foreach (MetaDataMember iteratorVariable0 in this.type.PersistentDataMembers)
                    {
                        if (this.IsModifiedMember(iteratorVariable0))
                        {
                            object boxedValue = iteratorVariable0.MemberAccessor.GetBoxedValue(this.current);
                            if ((this.original != null) && iteratorVariable0.StorageAccessor.HasValue(this.original))
                            {
                                object original = iteratorVariable0.MemberAccessor.GetBoxedValue(this.original);
                                yield return new ModifiedMemberInfo(iteratorVariable0.Member, boxedValue, original);
                            }
                            else if ((this.original == null) || (iteratorVariable0.IsDeferred && !iteratorVariable0.StorageAccessor.HasLoadedValue(this.current)))
                            {
                                yield return new ModifiedMemberInfo(iteratorVariable0.Member, boxedValue, null);
                            }
                        }
                    }
                }

                private string GetState()
                {
                    switch (this.state)
                    {
                        case State.New:
                        case State.Deleted:
                        case State.Removed:
                        case State.Dead:
                            return this.state.ToString();
                    }
                    if (this.IsModified)
                    {
                        return "Modified";
                    }
                    return "Unmodified";
                }

                internal override bool HasChangedValue(MetaDataMember mm)
                {
                    if (this.current != this.original)
                    {
                        if (mm.IsAssociation && mm.Association.IsMany)
                        {
                            return mm.StorageAccessor.HasAssignedValue(this.original);
                        }
                        if (mm.StorageAccessor.HasValue(this.current))
                        {
                            if ((this.original != null) && mm.StorageAccessor.HasValue(this.original))
                            {
                                if (this.dirtyMemberCache.Get(mm.Ordinal))
                                {
                                    return true;
                                }
                                object boxedValue = mm.MemberAccessor.GetBoxedValue(this.original);
                                return !object.Equals(mm.MemberAccessor.GetBoxedValue(this.current), boxedValue);
                            }
                            if (mm.IsDeferred && mm.StorageAccessor.HasAssignedValue(this.current))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                internal override bool HasChangedValues()
                {
                    if (this.current != this.original)
                    {
                        if (this.IsNew)
                        {
                            return true;
                        }
                        foreach (MetaDataMember member in this.type.PersistentDataMembers)
                        {
                            if (!member.IsAssociation && this.HasChangedValue(member))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                private bool HasDeferredLoader(MetaDataMember deferredMember)
                {
                    if (!deferredMember.IsDeferred)
                    {
                        return false;
                    }
                    MetaAccessor storageAccessor = deferredMember.StorageAccessor;
                    if (storageAccessor.HasAssignedValue(this.current) || storageAccessor.HasLoadedValue(this.current))
                    {
                        return false;
                    }
                    IEnumerable boxedValue = (IEnumerable) deferredMember.DeferredSourceAccessor.GetBoxedValue(this.current);
                    return (boxedValue != null);
                }

                private void InitializeDeferredLoader(MetaDataMember deferredMember)
                {
                    MetaAccessor storageAccessor = deferredMember.StorageAccessor;
                    if (!storageAccessor.HasAssignedValue(this.current) && !storageAccessor.HasLoadedValue(this.current))
                    {
                        MetaAccessor deferredSourceAccessor = deferredMember.DeferredSourceAccessor;
                        IEnumerable boxedValue = (IEnumerable) deferredSourceAccessor.GetBoxedValue(this.current);
                        if (boxedValue == null)
                        {
                            boxedValue = this.tracker.services.GetDeferredSourceFactory(deferredMember).CreateDeferredSource(this.current);
                            deferredSourceAccessor.SetBoxedValue(ref this.current, boxedValue);
                        }
                        else if ((boxedValue != null) && !this.haveInitializedDeferredLoaders)
                        {
                            throw System.Data.Linq.Error.CannotAttachAddNonNewEntities();
                        }
                    }
                }

                internal override void InitializeDeferredLoaders()
                {
                    if (this.tracker.services.Context.DeferredLoadingEnabled)
                    {
                        foreach (MetaAssociation association in this.Type.Associations)
                        {
                            if (!this.IsPendingGeneration(association.ThisKey))
                            {
                                this.InitializeDeferredLoader(association.ThisMember);
                            }
                        }
                        foreach (MetaDataMember member in from p in this.Type.PersistentDataMembers
                            where p.IsDeferred && !p.IsAssociation
                            select p)
                        {
                            if (!this.IsPendingGeneration(this.Type.IdentityMembers))
                            {
                                this.InitializeDeferredLoader(member);
                            }
                        }
                        this.haveInitializedDeferredLoaders = true;
                    }
                }

                internal override bool IsMemberPendingGeneration(MetaDataMember keyMember)
                {
                    if (this.IsNew && keyMember.IsDbGenerated)
                    {
                        return true;
                    }
                    foreach (MetaAssociation association in this.type.Associations)
                    {
                        if (association.IsForeignKey)
                        {
                            int index = association.ThisKey.IndexOf(keyMember);
                            if (index > -1)
                            {
                                object boxedValue = null;
                                if (association.ThisMember.IsDeferred)
                                {
                                    boxedValue = association.ThisMember.DeferredValueAccessor.GetBoxedValue(this.current);
                                }
                                else
                                {
                                    boxedValue = association.ThisMember.StorageAccessor.GetBoxedValue(this.current);
                                }
                                if ((boxedValue != null) && !association.IsMany)
                                {
                                    ChangeTracker.StandardChangeTracker.StandardTrackedObject trackedObject = (ChangeTracker.StandardChangeTracker.StandardTrackedObject) this.tracker.GetTrackedObject(boxedValue);
                                    if (trackedObject != null)
                                    {
                                        MetaDataMember member = association.OtherKey[index];
                                        return trackedObject.IsMemberPendingGeneration(member);
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }

                private bool IsModifiedMember(MetaDataMember member)
                {
                    if (((member.IsAssociation || member.IsPrimaryKey) || (member.IsVersion || member.IsDbGenerated)) || !member.StorageAccessor.HasAssignedValue(this.current))
                    {
                        return false;
                    }
                    return ((this.state == State.Modified) || ((this.state == State.PossiblyModified) && this.HasChangedValue(member)));
                }

                internal override bool IsPendingGeneration(IEnumerable<MetaDataMember> key)
                {
                    if (this.IsNew)
                    {
                        foreach (MetaDataMember member in key)
                        {
                            if (this.IsMemberPendingGeneration(member))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                internal override void Refresh(RefreshMode mode, object freshInstance)
                {
                    this.SynchDependentData();
                    this.UpdateDirtyMemberCache();
                    System.Type c = freshInstance.GetType();
                    foreach (MetaDataMember member in this.type.PersistentDataMembers)
                    {
                        RefreshMode mode2 = member.IsDbGenerated ? RefreshMode.OverwriteCurrentValues : mode;
                        if (((mode2 != RefreshMode.KeepCurrentValues) && !member.IsAssociation) && ((this.Type.Type == c) || member.DeclaringType.Type.IsAssignableFrom(c)))
                        {
                            object boxedValue = member.StorageAccessor.GetBoxedValue(freshInstance);
                            this.RefreshMember(member, mode2, boxedValue);
                        }
                    }
                    this.original = this.CreateDataCopy(freshInstance);
                    if (mode == RefreshMode.OverwriteCurrentValues)
                    {
                        this.ResetDirtyMemberTracking();
                    }
                }

                internal override void RefreshMember(MetaDataMember mm, RefreshMode mode, object freshValue)
                {
                    if ((mode != RefreshMode.KeepCurrentValues) && (!this.HasChangedValue(mm) || (mode == RefreshMode.OverwriteCurrentValues)))
                    {
                        object boxedValue = mm.StorageAccessor.GetBoxedValue(this.current);
                        if (!object.Equals(freshValue, boxedValue))
                        {
                            mm.StorageAccessor.SetBoxedValue(ref this.current, freshValue);
                            foreach (MetaDataMember member in this.GetAssociationsForKey(mm))
                            {
                                if (!member.Association.IsMany)
                                {
                                    IEnumerable source = this.tracker.services.GetDeferredSourceFactory(member).CreateDeferredSource(this.current);
                                    if (member.StorageAccessor.HasValue(this.current))
                                    {
                                        this.AssignMember(this.current, member, source.Cast<object>().SingleOrDefault<object>());
                                    }
                                }
                            }
                        }
                    }
                }

                private void ResetDirtyMemberTracking()
                {
                    this.dirtyMemberCache.SetAll(false);
                }

                internal void StartTracking()
                {
                    if (this.original == this.current)
                    {
                        this.original = this.CreateDataCopy(this.current);
                    }
                }

                internal override void SynchDependentData()
                {
                    foreach (MetaAssociation association in this.Type.Associations)
                    {
                        MetaDataMember thisMember = association.ThisMember;
                        if (association.IsForeignKey)
                        {
                            bool flag = thisMember.StorageAccessor.HasAssignedValue(this.current);
                            bool flag2 = thisMember.StorageAccessor.HasLoadedValue(this.current);
                            if (flag || flag2)
                            {
                                object boxedValue = thisMember.StorageAccessor.GetBoxedValue(this.current);
                                if (boxedValue != null)
                                {
                                    int num = 0;
                                    int count = association.ThisKey.Count;
                                    while (num < count)
                                    {
                                        MetaDataMember member2 = association.ThisKey[num];
                                        MetaDataMember member3 = association.OtherKey[num];
                                        object obj3 = member3.StorageAccessor.GetBoxedValue(boxedValue);
                                        member2.StorageAccessor.SetBoxedValue(ref this.current, obj3);
                                        num++;
                                    }
                                }
                                else if (association.IsNullable)
                                {
                                    if (thisMember.IsDeferred || ((this.original != null) && (thisMember.MemberAccessor.GetBoxedValue(this.original) != null)))
                                    {
                                        int num3 = 0;
                                        int num4 = association.ThisKey.Count;
                                        while (num3 < num4)
                                        {
                                            MetaDataMember mm = association.ThisKey[num3];
                                            if (mm.CanBeNull)
                                            {
                                                if ((this.original != null) && this.HasChangedValue(mm))
                                                {
                                                    if (mm.StorageAccessor.GetBoxedValue(this.current) != null)
                                                    {
                                                        throw System.Data.Linq.Error.InconsistentAssociationAndKeyChange(mm.Member.Name, thisMember.Member.Name);
                                                    }
                                                }
                                                else
                                                {
                                                    mm.StorageAccessor.SetBoxedValue(ref this.current, null);
                                                }
                                            }
                                            num3++;
                                        }
                                    }
                                }
                                else if (!flag2)
                                {
                                    StringBuilder builder = new StringBuilder();
                                    foreach (MetaDataMember member5 in association.ThisKey)
                                    {
                                        if (builder.Length > 0)
                                        {
                                            builder.Append(", ");
                                        }
                                        builder.AppendFormat("{0}.{1}", this.Type.Name.ToString(), member5.Name);
                                    }
                                    throw System.Data.Linq.Error.CouldNotRemoveRelationshipBecauseOneSideCannotBeNull(association.OtherType.Name, this.Type.Name, builder);
                                }
                            }
                        }
                    }
                    if (this.type.HasInheritance)
                    {
                        if (this.original != null)
                        {
                            object discriminator = this.type.Discriminator.MemberAccessor.GetBoxedValue(this.current);
                            MetaType type = ChangeTracker.StandardChangeTracker.TypeFromDiscriminator(this.type, discriminator);
                            object obj5 = this.type.Discriminator.MemberAccessor.GetBoxedValue(this.original);
                            MetaType type2 = ChangeTracker.StandardChangeTracker.TypeFromDiscriminator(this.type, obj5);
                            if (type != type2)
                            {
                                throw System.Data.Linq.Error.CannotChangeInheritanceType(obj5, discriminator, this.original.GetType().Name, type);
                            }
                        }
                        else
                        {
                            MetaType inheritanceType = this.type.GetInheritanceType(this.current.GetType());
                            if (inheritanceType.HasInheritanceCode)
                            {
                                object inheritanceCode = inheritanceType.InheritanceCode;
                                this.type.Discriminator.MemberAccessor.SetBoxedValue(ref this.current, inheritanceCode);
                            }
                        }
                    }
                }

                public override string ToString() => 
                    (this.type.Name + ":" + this.GetState());

                private void UpdateDirtyMemberCache()
                {
                    foreach (MetaDataMember member in this.type.PersistentDataMembers)
                    {
                        if ((!member.IsAssociation || !member.Association.IsMany) && (!this.dirtyMemberCache.Get(member.Ordinal) && this.HasChangedValue(member)))
                        {
                            this.dirtyMemberCache.Set(member.Ordinal, true);
                        }
                    }
                }

                internal override object Current =>
                    this.current;

                internal override bool HasDeferredLoaders
                {
                    get
                    {
                        foreach (MetaAssociation association in this.Type.Associations)
                        {
                            if (this.HasDeferredLoader(association.ThisMember))
                            {
                                return true;
                            }
                        }
                        foreach (MetaDataMember member in from p in this.Type.PersistentDataMembers
                            where p.IsDeferred && !p.IsAssociation
                            select p)
                        {
                            if (this.HasDeferredLoader(member))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }

                internal override bool IsDead =>
                    (this.state == State.Dead);

                internal override bool IsDeleted =>
                    (this.state == State.Deleted);

                internal override bool IsInteresting =>
                    (((((this.state == State.New) || (this.state == State.Deleted)) || (this.state == State.Modified)) || ((this.state == State.PossiblyModified) && (this.current != this.original))) || this.CanInferDelete());

                internal override bool IsModified =>
                    ((this.state == State.Modified) || (((this.state == State.PossiblyModified) && (this.current != this.original)) && this.HasChangedValues()));

                internal override bool IsNew =>
                    (this.state == State.New);

                internal override bool IsPossiblyModified
                {
                    get
                    {
                        if (this.state != State.Modified)
                        {
                            return (this.state == State.PossiblyModified);
                        }
                        return true;
                    }
                }

                internal override bool IsRemoved =>
                    (this.state == State.Removed);

                internal override bool IsUnmodified
                {
                    get
                    {
                        if (this.state != State.PossiblyModified)
                        {
                            return false;
                        }
                        if (this.current != this.original)
                        {
                            return !this.HasChangedValues();
                        }
                        return true;
                    }
                }

                internal override bool IsWeaklyTracked =>
                    this.isWeaklyTracked;

                internal override object Original =>
                    this.original;

                internal override MetaType Type =>
                    this.type;



                private enum State
                {
                    New,
                    Deleted,
                    PossiblyModified,
                    Modified,
                    Removed,
                    Dead
                }
            }
        }
    }
}

