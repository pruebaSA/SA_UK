namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal interface IObjectViewData<T>
    {
        int Add(T item, bool isAddNew);
        void Clear();
        void CommitItemAt(int index);
        void EnsureCanAddNew();
        ListChangedEventArgs OnCollectionChanged(object sender, CollectionChangeEventArgs e, ObjectViewListener listener);
        bool Remove(T item, bool isCancelNew);

        bool AllowEdit { get; }

        bool AllowNew { get; }

        bool AllowRemove { get; }

        bool FiresEventOnAdd { get; }

        bool FiresEventOnClear { get; }

        bool FiresEventOnRemove { get; }

        IList<T> List { get; }
    }
}

