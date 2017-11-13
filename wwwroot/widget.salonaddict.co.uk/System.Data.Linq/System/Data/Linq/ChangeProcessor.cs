﻿namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class ChangeProcessor
    {
        private ChangeDirector changeDirector;
        private DataContext context;
        private EdgeMap currentParentEdges;
        private EdgeMap originalChildEdges;
        private ReferenceMap originalChildReferences;
        private CommonDataServices services;
        private ChangeTracker tracker;

        internal ChangeProcessor(CommonDataServices services, DataContext context)
        {
            this.services = services;
            this.context = context;
            this.tracker = services.ChangeTracker;
            this.changeDirector = services.ChangeDirector;
            this.currentParentEdges = new EdgeMap();
            this.originalChildEdges = new EdgeMap();
            this.originalChildReferences = new ReferenceMap();
        }

        internal void ApplyInferredDeletions()
        {
            foreach (TrackedObject obj2 in this.tracker.GetInterestingObjects())
            {
                if (obj2.CanInferDelete())
                {
                    if (obj2.IsNew)
                    {
                        obj2.ConvertToRemoved();
                    }
                    else if (obj2.IsPossiblyModified || obj2.IsModified)
                    {
                        obj2.ConvertToDeleted();
                    }
                }
            }
        }

        private void BuildDependencyOrderedList(TrackedObject item, List<TrackedObject> list, Dictionary<TrackedObject, VisitState> visited)
        {
            VisitState state;
            if (visited.TryGetValue(item, out state))
            {
                if (state == VisitState.Before)
                {
                    throw System.Data.Linq.Error.CycleDetected();
                }
            }
            else
            {
                visited[item] = VisitState.Before;
                if (item.IsInteresting)
                {
                    if (item.IsDeleted)
                    {
                        foreach (TrackedObject obj2 in this.originalChildReferences[item])
                        {
                            if (obj2 != item)
                            {
                                this.BuildDependencyOrderedList(obj2, list, visited);
                            }
                        }
                    }
                    else
                    {
                        foreach (MetaAssociation association in item.Type.Associations)
                        {
                            if (association.IsForeignKey)
                            {
                                TrackedObject obj3 = this.currentParentEdges[association, item];
                                if (obj3 != null)
                                {
                                    if (obj3.IsNew)
                                    {
                                        if ((obj3 != item) || (item.Type.DBGeneratedIdentityMember != null))
                                        {
                                            this.BuildDependencyOrderedList(obj3, list, visited);
                                        }
                                    }
                                    else if (association.IsUnique || association.ThisKeyIsPrimaryKey)
                                    {
                                        TrackedObject obj4 = this.originalChildEdges[association, obj3];
                                        if ((obj4 != null) && (obj3 != item))
                                        {
                                            this.BuildDependencyOrderedList(obj4, list, visited);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    list.Add(item);
                }
                visited[item] = VisitState.After;
            }
        }

        private void BuildEdgeMaps()
        {
            this.currentParentEdges.Clear();
            this.originalChildEdges.Clear();
            this.originalChildReferences.Clear();
            List<TrackedObject> list = new List<TrackedObject>(this.tracker.GetInterestingObjects());
            foreach (TrackedObject obj2 in list)
            {
                bool isNew = obj2.IsNew;
                foreach (MetaAssociation association in obj2.Type.Associations)
                {
                    if (association.IsForeignKey)
                    {
                        TrackedObject otherItem = this.GetOtherItem(association, obj2.Current);
                        TrackedObject from = this.GetOtherItem(association, obj2.Original);
                        bool flag2 = ((otherItem != null) && otherItem.IsDeleted) || ((from != null) && from.IsDeleted);
                        bool flag3 = (otherItem != null) && otherItem.IsNew;
                        if ((isNew || flag2) || (flag3 || this.HasAssociationChanged(association, obj2)))
                        {
                            if (otherItem != null)
                            {
                                this.currentParentEdges.Add(association, obj2, otherItem);
                            }
                            if (from != null)
                            {
                                if (association.IsUnique)
                                {
                                    this.originalChildEdges.Add(association, from, obj2);
                                }
                                this.originalChildReferences.Add(from, obj2);
                            }
                        }
                    }
                }
            }
        }

        private static void CheckForInvalidChanges(TrackedObject tracked)
        {
            foreach (MetaDataMember member in tracked.Type.PersistentDataMembers)
            {
                if (((member.IsPrimaryKey || member.IsDbGenerated) || member.IsVersion) && tracked.HasChangedValue(member))
                {
                    if (member.IsPrimaryKey)
                    {
                        throw System.Data.Linq.Error.IdentityChangeNotAllowed(member.Name, tracked.Type.Name);
                    }
                    throw System.Data.Linq.Error.DbGeneratedChangeNotAllowed(member.Name, tracked.Type.Name);
                }
            }
        }

        private void ClearForeignKeyReferences(TrackedObject to)
        {
            foreach (MetaAssociation association in to.Type.Associations)
            {
                if ((association.IsForeignKey && (association.OtherMember != null)) && (association.OtherMember.IsAssociation && association.OtherKeyIsPrimaryKey))
                {
                    object[] foreignKeyValues = CommonDataServices.GetForeignKeyValues(association, to.Current);
                    object instance = this.services.IdentityManager.Find(association.OtherType, foreignKeyValues);
                    if (instance != null)
                    {
                        if (association.OtherMember.Association.IsMany)
                        {
                            IList boxedValue = association.OtherMember.MemberAccessor.GetBoxedValue(instance) as IList;
                            if ((boxedValue != null) && !boxedValue.IsFixedSize)
                            {
                                boxedValue.Remove(to.Current);
                                this.ClearForeignKeysHelper(association, to.Current);
                            }
                        }
                        else
                        {
                            association.OtherMember.MemberAccessor.SetBoxedValue(ref instance, null);
                            this.ClearForeignKeysHelper(association, to.Current);
                        }
                    }
                }
                else if (association.IsForeignKey)
                {
                    this.ClearForeignKeysHelper(association, to.Current);
                }
            }
        }

        private void ClearForeignKeysHelper(MetaAssociation assoc, object trackedInstance)
        {
            assoc.ThisMember.MemberAccessor.SetBoxedValue(ref trackedInstance, null);
            int num = 0;
            int count = assoc.ThisKey.Count;
            while (num < count)
            {
                MetaDataMember member = assoc.ThisKey[num];
                if (member.CanBeNull)
                {
                    member.StorageAccessor.SetBoxedValue(ref trackedInstance, null);
                }
                num++;
            }
        }

        private static int Compare(TrackedObject x, int xOrdinal, TrackedObject y, int yOrdinal)
        {
            if (x == y)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            int num = x.IsNew ? 0 : (x.IsDeleted ? 2 : 1);
            int num2 = y.IsNew ? 0 : (y.IsDeleted ? 2 : 1);
            if (num < num2)
            {
                return -1;
            }
            if (num > num2)
            {
                return 1;
            }
            if (!x.IsNew)
            {
                if (x.Type != y.Type)
                {
                    return string.CompareOrdinal(x.Type.Type.FullName, y.Type.Type.FullName);
                }
                int num3 = 0;
                foreach (MetaDataMember member in x.Type.IdentityMembers)
                {
                    object boxedValue = member.StorageAccessor.GetBoxedValue(x.Current);
                    object obj3 = member.StorageAccessor.GetBoxedValue(y.Current);
                    if (boxedValue == null)
                    {
                        if (obj3 != null)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        IComparable comparable = boxedValue as IComparable;
                        if (comparable != null)
                        {
                            num3 = comparable.CompareTo(obj3);
                            if (num3 != 0)
                            {
                                return num3;
                            }
                        }
                    }
                }
            }
            return xOrdinal.CompareTo(yOrdinal);
        }

        private static ChangeConflictException CreateChangeConflictException(int totalUpdatesAttempted, int failedUpdates)
        {
            string rowNotFoundOrChanged = System.Data.Linq.Strings.RowNotFoundOrChanged;
            if (totalUpdatesAttempted > 1)
            {
                rowNotFoundOrChanged = System.Data.Linq.Strings.UpdatesFailedMessage(failedUpdates, totalUpdatesAttempted);
            }
            return new ChangeConflictException(rowNotFoundOrChanged);
        }

        internal ChangeSet GetChangeSet()
        {
            List<object> list = new List<object>();
            List<object> list2 = new List<object>();
            List<object> list3 = new List<object>();
            this.ObserveUntrackedObjects();
            this.ApplyInferredDeletions();
            foreach (TrackedObject obj2 in this.tracker.GetInterestingObjects())
            {
                if (obj2.IsNew)
                {
                    obj2.SynchDependentData();
                    list.Add(obj2.Current);
                }
                else if (obj2.IsDeleted)
                {
                    list2.Add(obj2.Current);
                }
                else if (obj2.IsPossiblyModified)
                {
                    obj2.SynchDependentData();
                    if (obj2.IsModified)
                    {
                        list3.Add(obj2.Current);
                    }
                }
            }
            return new ChangeSet(list.AsReadOnly(), list2.AsReadOnly(), list3.AsReadOnly());
        }

        internal string GetChangeText()
        {
            this.ObserveUntrackedObjects();
            this.ApplyInferredDeletions();
            this.BuildEdgeMaps();
            StringBuilder appendTo = new StringBuilder();
            foreach (TrackedObject obj2 in this.GetOrderedList())
            {
                if (obj2.IsNew)
                {
                    obj2.SynchDependentData();
                    this.changeDirector.AppendInsertText(obj2, appendTo);
                }
                else if (obj2.IsDeleted)
                {
                    this.changeDirector.AppendDeleteText(obj2, appendTo);
                }
                else if (obj2.IsPossiblyModified)
                {
                    obj2.SynchDependentData();
                    if (obj2.IsModified)
                    {
                        this.changeDirector.AppendUpdateText(obj2, appendTo);
                    }
                }
            }
            return appendTo.ToString();
        }

        private List<TrackedObject> GetOrderedList()
        {
            List<TrackedObject> objects = this.tracker.GetInterestingObjects().ToList<TrackedObject>();
            List<int> list = Enumerable.Range(0, objects.Count).ToList<int>();
            list.Sort((x, y) => Compare(objects[x], x, objects[y], y));
            List<TrackedObject> list2 = (from i in list select objects[i]).ToList<TrackedObject>();
            Dictionary<TrackedObject, VisitState> visited = new Dictionary<TrackedObject, VisitState>();
            List<TrackedObject> list3 = new List<TrackedObject>();
            foreach (TrackedObject obj2 in list2)
            {
                this.BuildDependencyOrderedList(obj2, list3, visited);
            }
            return list3;
        }

        private TrackedObject GetOtherItem(MetaAssociation assoc, object instance)
        {
            if (instance == null)
            {
                return null;
            }
            object boxedValue = null;
            if (assoc.ThisMember.StorageAccessor.HasAssignedValue(instance) || assoc.ThisMember.StorageAccessor.HasLoadedValue(instance))
            {
                boxedValue = assoc.ThisMember.MemberAccessor.GetBoxedValue(instance);
            }
            else if (assoc.OtherKeyIsPrimaryKey)
            {
                object[] foreignKeyValues = CommonDataServices.GetForeignKeyValues(assoc, instance);
                boxedValue = this.services.GetCachedObject(assoc.OtherType, foreignKeyValues);
            }
            if (boxedValue == null)
            {
                return null;
            }
            return this.tracker.GetTrackedObject(boxedValue);
        }

        private bool HasAssociationChanged(MetaAssociation assoc, TrackedObject item)
        {
            if ((item.Original != null) && (item.Current != null))
            {
                if (assoc.ThisMember.StorageAccessor.HasAssignedValue(item.Current) || assoc.ThisMember.StorageAccessor.HasLoadedValue(item.Current))
                {
                    return (this.GetOtherItem(assoc, item.Current) != this.GetOtherItem(assoc, item.Original));
                }
                object[] foreignKeyValues = CommonDataServices.GetForeignKeyValues(assoc, item.Current);
                object[] objArray2 = CommonDataServices.GetForeignKeyValues(assoc, item.Original);
                int index = 0;
                int length = foreignKeyValues.Length;
                while (index < length)
                {
                    if (!object.Equals(foreignKeyValues[index], objArray2[index]))
                    {
                        return true;
                    }
                    index++;
                }
            }
            return false;
        }

        internal void ObserveUntrackedObjects()
        {
            Dictionary<object, object> visited = new Dictionary<object, object>();
            List<TrackedObject> list = new List<TrackedObject>(this.tracker.GetInterestingObjects());
            foreach (TrackedObject obj2 in list)
            {
                this.ObserveUntrackedObjects(obj2.Type, obj2.Current, visited);
            }
        }

        private void ObserveUntrackedObjects(MetaType type, object item, Dictionary<object, object> visited)
        {
            if (!visited.ContainsKey(item))
            {
                visited.Add(item, item);
                TrackedObject trackedObject = this.tracker.GetTrackedObject(item);
                if (trackedObject == null)
                {
                    trackedObject = this.tracker.Track(item);
                    trackedObject.ConvertToNew();
                }
                else if (trackedObject.IsDead || trackedObject.IsRemoved)
                {
                    return;
                }
                foreach (RelatedItem item2 in this.services.GetParents(type, item))
                {
                    this.ObserveUntrackedObjects(item2.Type, item2.Item, visited);
                }
                if (trackedObject.IsNew && !trackedObject.IsPendingGeneration(trackedObject.Type.IdentityMembers))
                {
                    trackedObject.SynchDependentData();
                }
                foreach (RelatedItem item3 in this.services.GetChildren(type, item))
                {
                    this.ObserveUntrackedObjects(item3.Type, item3.Item, visited);
                }
            }
        }

        private void PostProcessUpdates(List<TrackedObject> insertedItems, List<TrackedObject> deletedItems)
        {
            foreach (TrackedObject obj2 in deletedItems)
            {
                this.services.RemoveCachedObjectLike(obj2.Type, obj2.Original);
                this.ClearForeignKeyReferences(obj2);
            }
            foreach (TrackedObject obj3 in insertedItems)
            {
                if (this.services.InsertLookupCachedObject(obj3.Type, obj3.Current) != obj3.Current)
                {
                    throw new DuplicateKeyException(obj3.Current, System.Data.Linq.Strings.DatabaseGeneratedAlreadyExistingKey);
                }
                obj3.InitializeDeferredLoaders();
            }
        }

        private static void SendOnValidate(MetaType type, TrackedObject item, ChangeAction changeAction)
        {
            if (type != null)
            {
                SendOnValidate(type.InheritanceBase, item, changeAction);
                if (type.OnValidateMethod != null)
                {
                    try
                    {
                        type.OnValidateMethod.Invoke(item.Current, new object[] { changeAction });
                    }
                    catch (TargetInvocationException exception)
                    {
                        if (exception.InnerException != null)
                        {
                            throw exception.InnerException;
                        }
                        throw;
                    }
                }
            }
        }

        internal void SubmitChanges(ConflictMode failureMode)
        {
            this.TrackUntrackedObjects();
            this.ApplyInferredDeletions();
            this.BuildEdgeMaps();
            List<TrackedObject> orderedList = this.GetOrderedList();
            ValidateAll(orderedList);
            int totalUpdatesAttempted = 0;
            ChangeConflictSession session = new ChangeConflictSession(this.context);
            List<ObjectChangeConflict> conflictList = new List<ObjectChangeConflict>();
            List<TrackedObject> deletedItems = new List<TrackedObject>();
            List<TrackedObject> insertedItems = new List<TrackedObject>();
            foreach (TrackedObject obj2 in orderedList)
            {
                try
                {
                    if (obj2.IsNew)
                    {
                        obj2.SynchDependentData();
                        this.changeDirector.Insert(obj2);
                        insertedItems.Add(obj2);
                    }
                    else if (obj2.IsDeleted)
                    {
                        totalUpdatesAttempted++;
                        if (this.changeDirector.Delete(obj2) == 0)
                        {
                            conflictList.Add(new ObjectChangeConflict(session, obj2, false));
                        }
                        else
                        {
                            deletedItems.Add(obj2);
                        }
                    }
                    else if (obj2.IsPossiblyModified)
                    {
                        obj2.SynchDependentData();
                        if (obj2.IsModified)
                        {
                            CheckForInvalidChanges(obj2);
                            totalUpdatesAttempted++;
                            if (this.changeDirector.Update(obj2) <= 0)
                            {
                                conflictList.Add(new ObjectChangeConflict(session, obj2));
                            }
                        }
                    }
                }
                catch (ChangeConflictException)
                {
                    conflictList.Add(new ObjectChangeConflict(session, obj2));
                }
                if ((conflictList.Count > 0) && (failureMode == ConflictMode.FailOnFirstConflict))
                {
                    break;
                }
            }
            if (conflictList.Count > 0)
            {
                this.context.ChangeConflicts.Fill(conflictList);
                throw CreateChangeConflictException(totalUpdatesAttempted, conflictList.Count);
            }
            this.PostProcessUpdates(insertedItems, deletedItems);
        }

        internal void TrackUntrackedObjects()
        {
            Dictionary<object, object> visited = new Dictionary<object, object>();
            List<TrackedObject> list = new List<TrackedObject>(this.tracker.GetInterestingObjects());
            foreach (TrackedObject obj2 in list)
            {
                this.TrackUntrackedObjects(obj2.Type, obj2.Current, visited);
            }
        }

        private void TrackUntrackedObjects(MetaType type, object item, Dictionary<object, object> visited)
        {
            if (!visited.ContainsKey(item))
            {
                visited.Add(item, item);
                TrackedObject trackedObject = this.tracker.GetTrackedObject(item);
                if (trackedObject == null)
                {
                    trackedObject = this.tracker.Track(item);
                    trackedObject.ConvertToNew();
                }
                else if (trackedObject.IsDead || trackedObject.IsRemoved)
                {
                    return;
                }
                foreach (RelatedItem item2 in this.services.GetParents(type, item))
                {
                    this.TrackUntrackedObjects(item2.Type, item2.Item, visited);
                }
                if (trackedObject.IsNew)
                {
                    trackedObject.InitializeDeferredLoaders();
                    if (!trackedObject.IsPendingGeneration(trackedObject.Type.IdentityMembers))
                    {
                        trackedObject.SynchDependentData();
                        object obj3 = this.services.InsertLookupCachedObject(trackedObject.Type, item);
                        if (obj3 != item)
                        {
                            TrackedObject obj4 = this.tracker.GetTrackedObject(obj3);
                            if (!obj4.IsDeleted && !obj4.CanInferDelete())
                            {
                                if (!obj4.IsDead)
                                {
                                    throw new DuplicateKeyException(item, System.Data.Linq.Strings.CantAddAlreadyExistingKey);
                                }
                            }
                            else
                            {
                                trackedObject.ConvertToPossiblyModified(obj4.Original);
                                obj4.ConvertToDead();
                                this.services.RemoveCachedObjectLike(trackedObject.Type, item);
                                this.services.InsertLookupCachedObject(trackedObject.Type, item);
                            }
                        }
                    }
                    else
                    {
                        object cachedObjectLike = this.services.GetCachedObjectLike(trackedObject.Type, item);
                        if (cachedObjectLike != null)
                        {
                            TrackedObject obj6 = this.tracker.GetTrackedObject(cachedObjectLike);
                            if (obj6.IsDeleted || obj6.CanInferDelete())
                            {
                                trackedObject.ConvertToPossiblyModified(obj6.Original);
                                obj6.ConvertToDead();
                                this.services.RemoveCachedObjectLike(trackedObject.Type, item);
                                this.services.InsertLookupCachedObject(trackedObject.Type, item);
                            }
                        }
                    }
                }
                foreach (RelatedItem item3 in this.services.GetChildren(type, item))
                {
                    this.TrackUntrackedObjects(item3.Type, item3.Item, visited);
                }
            }
        }

        private static void ValidateAll(IEnumerable<TrackedObject> list)
        {
            foreach (TrackedObject obj2 in list)
            {
                if (obj2.IsNew)
                {
                    obj2.SynchDependentData();
                    if (obj2.Type.HasAnyValidateMethod)
                    {
                        SendOnValidate(obj2.Type, obj2, ChangeAction.Insert);
                    }
                }
                else if (obj2.IsDeleted)
                {
                    if (obj2.Type.HasAnyValidateMethod)
                    {
                        SendOnValidate(obj2.Type, obj2, ChangeAction.Delete);
                    }
                }
                else if (obj2.IsPossiblyModified)
                {
                    obj2.SynchDependentData();
                    if (obj2.IsModified && obj2.Type.HasAnyValidateMethod)
                    {
                        SendOnValidate(obj2.Type, obj2, ChangeAction.Update);
                    }
                }
            }
        }

        private class EdgeMap
        {
            private Dictionary<MetaAssociation, Dictionary<TrackedObject, TrackedObject>> associations = new Dictionary<MetaAssociation, Dictionary<TrackedObject, TrackedObject>>();

            internal EdgeMap()
            {
            }

            internal void Add(MetaAssociation assoc, TrackedObject from, TrackedObject to)
            {
                Dictionary<TrackedObject, TrackedObject> dictionary;
                if (!this.associations.TryGetValue(assoc, out dictionary))
                {
                    dictionary = new Dictionary<TrackedObject, TrackedObject>();
                    this.associations.Add(assoc, dictionary);
                }
                dictionary.Add(from, to);
            }

            internal void Clear()
            {
                this.associations.Clear();
            }

            internal TrackedObject this[MetaAssociation assoc, TrackedObject from]
            {
                get
                {
                    Dictionary<TrackedObject, TrackedObject> dictionary;
                    TrackedObject obj2;
                    if (this.associations.TryGetValue(assoc, out dictionary) && dictionary.TryGetValue(from, out obj2))
                    {
                        return obj2;
                    }
                    return null;
                }
            }
        }

        private class ReferenceMap
        {
            private static TrackedObject[] Empty = new TrackedObject[0];
            private Dictionary<TrackedObject, List<TrackedObject>> references = new Dictionary<TrackedObject, List<TrackedObject>>();

            internal ReferenceMap()
            {
            }

            internal void Add(TrackedObject from, TrackedObject to)
            {
                List<TrackedObject> list;
                if (!this.references.TryGetValue(from, out list))
                {
                    list = new List<TrackedObject>();
                    this.references.Add(from, list);
                }
                if (!list.Contains(to))
                {
                    list.Add(to);
                }
            }

            internal void Clear()
            {
                this.references.Clear();
            }

            internal IEnumerable<TrackedObject> this[TrackedObject from]
            {
                get
                {
                    List<TrackedObject> list;
                    if (this.references.TryGetValue(from, out list))
                    {
                        return list;
                    }
                    return Empty;
                }
            }
        }

        private enum VisitState
        {
            Before,
            After
        }
    }
}

