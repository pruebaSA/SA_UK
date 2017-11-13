namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Reflection;

    internal class ObjectView<TElement> : IBindingList, IList, ICollection, IEnumerable, ICancelAddNew, IObjectView
    {
        private int _addNewIndex;
        private ObjectViewListener _listener;
        private bool _suspendEvent;
        private IObjectViewData<TElement> _viewData;

        public event ListChangedEventHandler ListChanged;

        internal ObjectView(IObjectViewData<TElement> viewData, object eventDataSource)
        {
            this._addNewIndex = -1;
            this._viewData = viewData;
            this._listener = new ObjectViewListener(this, (IList) this._viewData.List, eventDataSource);
        }

        public void CopyTo(Array array, int index)
        {
            ((IList) this._viewData.List).CopyTo(array, index);
        }

        private void EnsureWritableList()
        {
            if (((IList) this).IsReadOnly)
            {
                throw EntityUtil.WriteOperationNotAllowedOnReadOnlyBindingList();
            }
        }

        public IEnumerator GetEnumerator() => 
            this._viewData.List.GetEnumerator();

        private void OnListChanged(ListChangedEventArgs changeArgs)
        {
            if ((this.onListChanged != null) && !this._suspendEvent)
            {
                this.onListChanged(this, changeArgs);
            }
        }

        private void OnListChanged(ListChangedType listchangedType, int newIndex, int oldIndex)
        {
            ListChangedEventArgs changeArgs = new ListChangedEventArgs(listchangedType, newIndex, oldIndex);
            this.OnListChanged(changeArgs);
        }

        int IList.Add(object value)
        {
            this.EnsureWritableList();
            EntityUtil.CheckArgumentNull<object>(value, "value");
            if (!(value is TElement))
            {
                throw EntityUtil.IncompatibleArgument();
            }
            ((ICancelAddNew) this).EndNew(this._addNewIndex);
            int index = ((IList) this).IndexOf(value);
            if (index == -1)
            {
                index = this._viewData.Add((TElement) value, false);
                if (!this._viewData.FiresEventOnAdd)
                {
                    this._listener.RegisterEntityEvents(value);
                    this.OnListChanged(ListChangedType.ItemAdded, index, -1);
                }
            }
            return index;
        }

        void IList.Clear()
        {
            this.EnsureWritableList();
            ((ICancelAddNew) this).EndNew(this._addNewIndex);
            if (this._viewData.FiresEventOnClear)
            {
                this._viewData.Clear();
            }
            else
            {
                try
                {
                    this._suspendEvent = true;
                    this._viewData.Clear();
                }
                finally
                {
                    this._suspendEvent = false;
                }
                this.OnListChanged(ListChangedType.Reset, -1, -1);
            }
        }

        bool IList.Contains(object value) => 
            ((value is TElement) && this._viewData.List.Contains((TElement) value));

        int IList.IndexOf(object value)
        {
            if (value is TElement)
            {
                return this._viewData.List.IndexOf((TElement) value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            throw EntityUtil.IndexBasedInsertIsNotSupported();
        }

        void IList.Remove(object value)
        {
            this.EnsureWritableList();
            EntityUtil.CheckArgumentNull<object>(value, "value");
            if (!(value is TElement))
            {
                throw EntityUtil.IncompatibleArgument();
            }
            ((ICancelAddNew) this).EndNew(this._addNewIndex);
            TElement item = (TElement) value;
            int index = this._viewData.List.IndexOf(item);
            if (this._viewData.Remove(item, false) && !this._viewData.FiresEventOnRemove)
            {
                this._listener.UnregisterEntityEvents(item);
                this.OnListChanged(ListChangedType.ItemDeleted, index, -1);
            }
        }

        void IList.RemoveAt(int index)
        {
            ((IList) this).Remove(((IList) this)[index]);
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw EntityUtil.NotSupported();
        }

        object IBindingList.AddNew()
        {
            this.EnsureWritableList();
            if (this.IsElementTypeAbstract)
            {
                throw EntityUtil.AddNewOperationNotAllowedOnAbstractBindingList();
            }
            this._viewData.EnsureCanAddNew();
            ((ICancelAddNew) this).EndNew(this._addNewIndex);
            TElement item = (TElement) Activator.CreateInstance(typeof(TElement));
            this._addNewIndex = this._viewData.Add(item, true);
            this._listener.RegisterEntityEvents(item);
            this.OnListChanged(ListChangedType.ItemAdded, this._addNewIndex, -1);
            return item;
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw EntityUtil.NotSupported();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw EntityUtil.NotSupported();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw EntityUtil.NotSupported();
        }

        void IBindingList.RemoveSort()
        {
            throw EntityUtil.NotSupported();
        }

        void ICancelAddNew.CancelNew(int itemIndex)
        {
            if ((this._addNewIndex >= 0) && (itemIndex == this._addNewIndex))
            {
                TElement entity = this._viewData.List[this._addNewIndex];
                this._listener.UnregisterEntityEvents(entity);
                int newIndex = this._addNewIndex;
                this._addNewIndex = -1;
                try
                {
                    this._suspendEvent = true;
                    this._viewData.Remove(entity, true);
                }
                finally
                {
                    this._suspendEvent = false;
                }
                this.OnListChanged(ListChangedType.ItemDeleted, newIndex, -1);
            }
        }

        void ICancelAddNew.EndNew(int itemIndex)
        {
            if ((this._addNewIndex >= 0) && (itemIndex == this._addNewIndex))
            {
                this._viewData.CommitItemAt(this._addNewIndex);
                this._addNewIndex = -1;
            }
        }

        void IObjectView.CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            TElement local = default(TElement);
            if (this._addNewIndex >= 0)
            {
                local = this[this._addNewIndex];
            }
            ListChangedEventArgs changeArgs = this._viewData.OnCollectionChanged(sender, e, this._listener);
            if (this._addNewIndex >= 0)
            {
                if (this._addNewIndex >= this.Count)
                {
                    this._addNewIndex = ((IList) this).IndexOf(local);
                }
                else
                {
                    TElement local2 = this[this._addNewIndex];
                    if (!local2.Equals(local))
                    {
                        this._addNewIndex = ((IList) this).IndexOf(local);
                    }
                }
            }
            if (changeArgs != null)
            {
                this.OnListChanged(changeArgs);
            }
        }

        void IObjectView.EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = ((IList) this).IndexOf((TElement) sender);
            this.OnListChanged(ListChangedType.ItemChanged, index, index);
        }

        public int Count =>
            this._viewData.List.Count;

        private bool IsElementTypeAbstract =>
            typeof(TElement).IsAbstract;

        public TElement this[int index]
        {
            get => 
                this._viewData.List[index];
            set
            {
                throw EntityUtil.CannotReplacetheEntityorRow();
            }
        }

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            (!this._viewData.AllowNew && !this._viewData.AllowRemove);

        object IList.this[int index]
        {
            get => 
                this._viewData.List[index];
            set
            {
                throw EntityUtil.CannotReplacetheEntityorRow();
            }
        }

        bool IBindingList.AllowEdit =>
            this._viewData.AllowEdit;

        bool IBindingList.AllowNew =>
            (this._viewData.AllowNew && !this.IsElementTypeAbstract);

        bool IBindingList.AllowRemove =>
            this._viewData.AllowRemove;

        bool IBindingList.IsSorted =>
            false;

        ListSortDirection IBindingList.SortDirection
        {
            get
            {
                throw EntityUtil.NotSupported();
            }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get
            {
                throw EntityUtil.NotSupported();
            }
        }

        bool IBindingList.SupportsChangeNotification =>
            true;

        bool IBindingList.SupportsSearching =>
            false;

        bool IBindingList.SupportsSorting =>
            false;
    }
}

