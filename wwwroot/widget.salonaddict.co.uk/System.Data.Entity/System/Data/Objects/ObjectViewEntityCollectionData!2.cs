namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Objects.DataClasses;

    internal sealed class ObjectViewEntityCollectionData<TViewElement, TItemElement> : IObjectViewData<TViewElement> where TViewElement: TItemElement where TItemElement: class, IEntityWithRelationships
    {
        private List<TViewElement> _bindingList;
        private readonly bool _canEditItems;
        private EntityCollection<TItemElement> _entityCollection;
        private bool _itemCommitPending;

        internal ObjectViewEntityCollectionData(EntityCollection<TItemElement> entityCollection)
        {
            this._entityCollection = entityCollection;
            this._canEditItems = typeof(IEntityWithChangeTracker).IsAssignableFrom(typeof(TViewElement));
            this._bindingList = new List<TViewElement>(entityCollection.Count);
            foreach (TViewElement local in entityCollection)
            {
                this._bindingList.Add(local);
            }
        }

        public int Add(TViewElement item, bool isAddNew)
        {
            if (isAddNew)
            {
                this._bindingList.Add(item);
            }
            else
            {
                this._entityCollection.Add((TItemElement) item);
            }
            return (this._bindingList.Count - 1);
        }

        public void Clear()
        {
            if (0 < this._bindingList.Count)
            {
                List<IEntityWithRelationships> list = new List<IEntityWithRelationships>();
                foreach (object obj2 in this._bindingList)
                {
                    list.Add(obj2 as IEntityWithRelationships);
                }
                this._entityCollection.BulkDeleteAll(list);
            }
        }

        public void CommitItemAt(int index)
        {
            TViewElement local = this._bindingList[index];
            try
            {
                this._itemCommitPending = true;
                this._entityCollection.Add((TItemElement) local);
            }
            finally
            {
                this._itemCommitPending = false;
            }
        }

        public void EnsureCanAddNew()
        {
        }

        public ListChangedEventArgs OnCollectionChanged(object sender, CollectionChangeEventArgs e, ObjectViewListener listener)
        {
            ListChangedEventArgs args = null;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if ((e.Element is TViewElement) && !this._itemCommitPending)
                    {
                        TViewElement element = (TViewElement) e.Element;
                        this._bindingList.Add(element);
                        listener.RegisterEntityEvents(element);
                        args = new ListChangedEventArgs(ListChangedType.ItemAdded, this._bindingList.Count - 1, -1);
                    }
                    return args;

                case CollectionChangeAction.Remove:
                    if (e.Element is TViewElement)
                    {
                        TViewElement item = (TViewElement) e.Element;
                        int index = this._bindingList.IndexOf(item);
                        if (index != -1)
                        {
                            this._bindingList.Remove(item);
                            listener.UnregisterEntityEvents(item);
                            args = new ListChangedEventArgs(ListChangedType.ItemDeleted, index, -1);
                        }
                    }
                    return args;

                case CollectionChangeAction.Refresh:
                    foreach (TViewElement local3 in this._bindingList)
                    {
                        listener.UnregisterEntityEvents(local3);
                    }
                    this._bindingList.Clear();
                    foreach (TViewElement local4 in this._entityCollection)
                    {
                        this._bindingList.Add(local4);
                        listener.RegisterEntityEvents(local4);
                    }
                    return new ListChangedEventArgs(ListChangedType.Reset, -1, -1);
            }
            return args;
        }

        public bool Remove(TViewElement item, bool isCancelNew)
        {
            if (isCancelNew)
            {
                return this._bindingList.Remove(item);
            }
            return this._entityCollection.Remove((TItemElement) item);
        }

        public bool AllowEdit =>
            this._canEditItems;

        public bool AllowNew =>
            !this._entityCollection.IsReadOnly;

        public bool AllowRemove =>
            !this._entityCollection.IsReadOnly;

        public bool FiresEventOnAdd =>
            true;

        public bool FiresEventOnClear =>
            true;

        public bool FiresEventOnRemove =>
            true;

        public IList<TViewElement> List =>
            this._bindingList;
    }
}

