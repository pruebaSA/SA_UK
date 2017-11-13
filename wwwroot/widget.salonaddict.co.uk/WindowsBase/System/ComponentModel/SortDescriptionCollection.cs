namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Runtime.CompilerServices;

    public class SortDescriptionCollection : Collection<SortDescription>, INotifyCollectionChanged
    {
        public static readonly SortDescriptionCollection Empty = new EmptySortDescriptionCollection();

        protected event NotifyCollectionChangedEventHandler CollectionChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged;

        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        protected override void InsertItem(int index, SortDescription item)
        {
            item.Seal();
            base.InsertItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

        protected override void RemoveItem(int index)
        {
            SortDescription item = base[index];
            base.RemoveItem(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        protected override void SetItem(int index, SortDescription item)
        {
            item.Seal();
            SortDescription description = base[index];
            base.SetItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, description, index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        private class EmptySortDescriptionCollection : SortDescriptionCollection, IList, ICollection, IEnumerable
        {
            protected override void ClearItems()
            {
                throw new NotSupportedException();
            }

            protected override void InsertItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }

            protected override void RemoveItem(int index)
            {
                throw new NotSupportedException();
            }

            protected override void SetItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }

            bool IList.IsFixedSize =>
                true;

            bool IList.IsReadOnly =>
                true;
        }
    }
}

