namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;

    internal sealed class ObjectViewQueryResultData<TElement> : IObjectViewData<TElement>
    {
        private List<TElement> _bindingList;
        private bool _canEditItems;
        private bool _canModifyList;
        private EntitySet _entitySet;
        private ObjectContext _objectContext;

        internal ObjectViewQueryResultData(IEnumerable queryResults, ObjectContext objectContext, bool forceReadOnlyList, EntitySet entitySet)
        {
            bool flag = typeof(IEntityWithChangeTracker).IsAssignableFrom(typeof(TElement));
            this._objectContext = objectContext;
            this._entitySet = entitySet;
            this._canEditItems = flag;
            this._canModifyList = (!forceReadOnlyList && flag) && (this._objectContext != null);
            this._bindingList = new List<TElement>();
            foreach (TElement local in queryResults)
            {
                this._bindingList.Add(local);
            }
        }

        public int Add(TElement item, bool isAddNew)
        {
            this.EnsureEntitySet();
            if (!isAddNew)
            {
                this._objectContext.AddObject(TypeHelpers.GetFullName(this._entitySet), item);
            }
            this._bindingList.Add(item);
            return (this._bindingList.Count - 1);
        }

        public void Clear()
        {
            while (0 < this._bindingList.Count)
            {
                TElement item = this._bindingList[this._bindingList.Count - 1];
                this.Remove(item, false);
            }
        }

        public void CommitItemAt(int index)
        {
            this.EnsureEntitySet();
            TElement entity = this._bindingList[index];
            this._objectContext.AddObject(TypeHelpers.GetFullName(this._entitySet), entity);
        }

        public void EnsureCanAddNew()
        {
            this.EnsureEntitySet();
        }

        private void EnsureEntitySet()
        {
            if (this._entitySet == null)
            {
                throw EntityUtil.CannotResolveTheEntitySetforGivenEntity(typeof(TElement));
            }
        }

        public ListChangedEventArgs OnCollectionChanged(object sender, CollectionChangeEventArgs e, ObjectViewListener listener)
        {
            ListChangedEventArgs args = null;
            if (e.Element.GetType().IsAssignableFrom(typeof(TElement)) && this._bindingList.Contains((TElement) e.Element))
            {
                TElement element = (TElement) e.Element;
                int index = this._bindingList.IndexOf(element);
                if ((index >= 0) && (e.Action == CollectionChangeAction.Remove))
                {
                    this._bindingList.Remove(element);
                    listener.UnregisterEntityEvents(element);
                    args = new ListChangedEventArgs(ListChangedType.ItemDeleted, index, -1);
                }
            }
            return args;
        }

        public bool Remove(TElement item, bool isCancelNew)
        {
            if (isCancelNew)
            {
                return this._bindingList.Remove(item);
            }
            ObjectStateEntry entry = this._objectContext.ObjectStateManager.FindObjectStateEntry(item);
            if (entry != null)
            {
                entry.Delete();
                return true;
            }
            return false;
        }

        public bool AllowEdit =>
            this._canEditItems;

        public bool AllowNew =>
            (this._canModifyList && (this._entitySet != null));

        public bool AllowRemove =>
            this._canModifyList;

        public bool FiresEventOnAdd =>
            false;

        public bool FiresEventOnClear =>
            false;

        public bool FiresEventOnRemove =>
            true;

        public IList<TElement> List =>
            this._bindingList;
    }
}

