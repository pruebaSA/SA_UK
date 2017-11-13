namespace System.Windows
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;

    public abstract class WeakEventManager : DispatcherObject
    {
        private WeakEventTable _table = WeakEventTable.CurrentWeakEventTable;

        protected WeakEventManager()
        {
        }

        [FriendAccessAllowed]
        internal static bool Cleanup() => 
            WeakEventTable.Cleanup();

        protected void DeliverEvent(object sender, EventArgs args)
        {
            ListenerList empty;
            using (this.Table.ReadLock)
            {
                empty = (ListenerList) this.Table[this, sender];
                if (empty == null)
                {
                    empty = ListenerList.Empty;
                }
                empty.BeginUse();
            }
            try
            {
                this.DeliverEventToList(sender, args, empty);
            }
            finally
            {
                empty.EndUse();
            }
        }

        protected void DeliverEventToList(object sender, EventArgs args, ListenerList list)
        {
            bool flag = false;
            Type managerType = base.GetType();
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                IWeakEventListener listener = list[num];
                if (listener != null)
                {
                    bool condition = listener.ReceiveWeakEvent(managerType, sender, args);
                    if (!condition)
                    {
                        Invariant.Assert(condition, System.Windows.SR.Get("ListenerDidNotHandleEvent"), System.Windows.SR.Get("ListenerDidNotHandleEventDetail", new object[] { listener.GetType(), managerType }));
                    }
                }
                else
                {
                    flag = true;
                }
                num++;
            }
            if (flag)
            {
                this.ScheduleCleanup();
            }
        }

        protected static WeakEventManager GetCurrentManager(Type managerType) => 
            WeakEventTable.CurrentWeakEventTable[managerType];

        protected void ProtectedAddListener(object source, IWeakEventListener listener)
        {
            using (this.Table.WriteLock)
            {
                ListenerList list = (ListenerList) this.Table[this, source];
                if (list == null)
                {
                    list = new ListenerList();
                    this.Table[this, source] = list;
                    this.StartListening(source);
                }
                if (ListenerList.PrepareForWriting(ref list))
                {
                    this.Table[this, source] = list;
                }
                list.Add(listener);
                this.ScheduleCleanup();
            }
        }

        protected void ProtectedRemoveListener(object source, IWeakEventListener listener)
        {
            if (source != null)
            {
                using (this.Table.WriteLock)
                {
                    ListenerList list = (ListenerList) this.Table[this, source];
                    if (list != null)
                    {
                        if (ListenerList.PrepareForWriting(ref list))
                        {
                            this.Table[this, source] = list;
                        }
                        list.Remove(listener);
                        if (list.IsEmpty)
                        {
                            this.Table.Remove(this, source);
                            this.StopListening(source);
                        }
                    }
                }
            }
        }

        protected virtual bool Purge(object source, object data, bool purgeAll)
        {
            bool flag = false;
            bool isEmpty = purgeAll || (source == null);
            if (!isEmpty)
            {
                ListenerList list = (ListenerList) data;
                if (ListenerList.PrepareForWriting(ref list) && (source != null))
                {
                    this.Table[this, source] = list;
                }
                if (list.Purge())
                {
                    flag = true;
                }
                isEmpty = list.IsEmpty;
            }
            if (isEmpty && (source != null))
            {
                this.StopListening(source);
                if (!purgeAll)
                {
                    this.Table.Remove(this, source);
                    flag = true;
                }
            }
            return flag;
        }

        internal bool PurgeInternal(object source, object data, bool purgeAll) => 
            this.Purge(source, data, purgeAll);

        protected void Remove(object source)
        {
            this.Table.Remove(this, source);
        }

        protected void ScheduleCleanup()
        {
            this.Table.ScheduleCleanup();
        }

        [FriendAccessAllowed]
        internal static void SetCleanupEnabled(bool value)
        {
            WeakEventTable.CurrentWeakEventTable.IsCleanupEnabled = value;
        }

        protected static void SetCurrentManager(Type managerType, WeakEventManager manager)
        {
            WeakEventTable.CurrentWeakEventTable[managerType] = manager;
        }

        protected abstract void StartListening(object source);
        protected abstract void StopListening(object source);

        protected object this[object source]
        {
            get => 
                this.Table[this, source];
            set
            {
                this.Table[this, source] = value;
            }
        }

        protected IDisposable ReadLock =>
            this.Table.ReadLock;

        private WeakEventTable Table =>
            this._table;

        protected IDisposable WriteLock =>
            this.Table.WriteLock;

        protected class ListenerList
        {
            private FrugalObjectList<WeakReference> _list;
            private int _users;
            private static WeakEventManager.ListenerList s_empty = new WeakEventManager.ListenerList();

            public ListenerList() : this(new FrugalObjectList<WeakReference>())
            {
            }

            private ListenerList(FrugalObjectList<WeakReference> list)
            {
                this._list = list;
            }

            public ListenerList(int capacity) : this(new FrugalObjectList<WeakReference>(capacity))
            {
            }

            public void Add(IWeakEventListener listener)
            {
                Invariant.Assert(this._users == 0, "Cannot modify a ListenerList that is in use");
                this._list.Add(new WeakReference(listener));
            }

            public bool BeginUse() => 
                (Interlocked.Increment(ref this._users) != 1);

            public WeakEventManager.ListenerList Clone() => 
                new WeakEventManager.ListenerList(this._list.Clone());

            public void EndUse()
            {
                Interlocked.Decrement(ref this._users);
            }

            public static bool PrepareForWriting(ref WeakEventManager.ListenerList list)
            {
                bool flag = list.BeginUse();
                list.EndUse();
                if (flag)
                {
                    list = list.Clone();
                }
                return flag;
            }

            public bool Purge()
            {
                Invariant.Assert(this._users == 0, "Cannot modify a ListenerList that is in use");
                bool flag = false;
                for (int i = this._list.Count - 1; i >= 0; i--)
                {
                    if (this._list[i].Target == null)
                    {
                        this._list.RemoveAt(i);
                        flag = true;
                    }
                }
                return flag;
            }

            public void Remove(IWeakEventListener listener)
            {
                Invariant.Assert(this._users == 0, "Cannot modify a ListenerList that is in use");
                for (int i = this._list.Count - 1; i >= 0; i--)
                {
                    if (this._list[i].Target == listener)
                    {
                        this._list.RemoveAt(i);
                        return;
                    }
                }
            }

            public int Count =>
                this._list.Count;

            public static WeakEventManager.ListenerList Empty =>
                s_empty;

            public bool IsEmpty =>
                (this._list.Count == 0);

            public IWeakEventListener this[int index] =>
                ((IWeakEventListener) this._list[index].Target);
        }
    }
}

