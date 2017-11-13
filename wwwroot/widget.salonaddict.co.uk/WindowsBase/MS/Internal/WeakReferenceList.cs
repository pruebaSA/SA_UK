namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;

    [FriendAccessAllowed]
    internal class WeakReferenceList : CopyOnWriteList, IEnumerable
    {
        public WeakReferenceList() : base(null)
        {
        }

        public WeakReferenceList(object syncRoot) : base(syncRoot)
        {
        }

        public override bool Add(object obj) => 
            this.Add(obj, false);

        public bool Add(object obj, bool skipFind)
        {
            lock (base.SyncRoot)
            {
                if (skipFind)
                {
                    if (base.LiveList.Count == base.LiveList.Capacity)
                    {
                        this.Purge();
                    }
                }
                else if (this.FindWeakReference(obj) >= 0)
                {
                    return false;
                }
                return base.Internal_Add(new WeakReference(obj));
            }
        }

        private int FindWeakReference(object obj)
        {
            bool flag = true;
            int num = -1;
            while (flag)
            {
                flag = false;
                ArrayList liveList = base.LiveList;
                for (int i = 0; i < liveList.Count; i++)
                {
                    WeakReference reference = (WeakReference) liveList[i];
                    if (reference.IsAlive)
                    {
                        if (obj != reference.Target)
                        {
                            continue;
                        }
                        num = i;
                        break;
                    }
                    flag = true;
                }
                if (flag)
                {
                    this.Purge();
                }
            }
            return num;
        }

        public WeakReferenceListEnumerator GetEnumerator() => 
            new WeakReferenceListEnumerator(base.List);

        public bool Insert(int index, object obj)
        {
            lock (base.SyncRoot)
            {
                if (this.FindWeakReference(obj) >= 0)
                {
                    return false;
                }
                return base.Internal_Insert(index, new WeakReference(obj));
            }
        }

        private void Purge()
        {
            ArrayList liveList = base.LiveList;
            int count = liveList.Count;
            int index = 0;
            while (index < count)
            {
                WeakReference reference = (WeakReference) liveList[index];
                if (!reference.IsAlive)
                {
                    break;
                }
                index++;
            }
            if (index < count)
            {
                base.DoCopyOnWriteCheck();
                liveList = base.LiveList;
                for (int i = index + 1; i < count; i++)
                {
                    WeakReference reference2 = (WeakReference) liveList[i];
                    if (reference2.IsAlive)
                    {
                        liveList[index++] = liveList[i];
                    }
                }
                if (index < count)
                {
                    liveList.RemoveRange(index, count - index);
                    int num4 = index << 1;
                    if (num4 < liveList.Capacity)
                    {
                        liveList.Capacity = num4;
                    }
                }
            }
        }

        public override bool Remove(object obj)
        {
            lock (base.SyncRoot)
            {
                int index = this.FindWeakReference(obj);
                if (index < 0)
                {
                    return false;
                }
                return base.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count
        {
            get
            {
                lock (base.SyncRoot)
                {
                    return base.LiveList.Count;
                }
            }
        }
    }
}

