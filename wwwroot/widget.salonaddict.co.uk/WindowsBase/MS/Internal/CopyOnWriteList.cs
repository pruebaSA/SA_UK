namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;

    [FriendAccessAllowed]
    internal class CopyOnWriteList
    {
        private ArrayList _LiveList;
        private ArrayList _readonlyWrapper;
        private object _syncRoot;

        public CopyOnWriteList() : this(null)
        {
        }

        public CopyOnWriteList(object syncRoot)
        {
            this._LiveList = new ArrayList();
            if (syncRoot == null)
            {
                syncRoot = new object();
            }
            this._syncRoot = syncRoot;
        }

        public virtual bool Add(object obj)
        {
            lock (this._syncRoot)
            {
                if (this.Find(obj) >= 0)
                {
                    return false;
                }
                return this.Internal_Add(obj);
            }
        }

        protected void DoCopyOnWriteCheck()
        {
            if (this._readonlyWrapper != null)
            {
                this._LiveList = (ArrayList) this._LiveList.Clone();
                this._readonlyWrapper = null;
            }
        }

        private int Find(object obj)
        {
            for (int i = 0; i < this._LiveList.Count; i++)
            {
                if (obj == this._LiveList[i])
                {
                    return i;
                }
            }
            return -1;
        }

        protected bool Internal_Add(object obj)
        {
            this.DoCopyOnWriteCheck();
            this._LiveList.Add(obj);
            return true;
        }

        protected bool Internal_Insert(int index, object obj)
        {
            this.DoCopyOnWriteCheck();
            this._LiveList.Insert(index, obj);
            return true;
        }

        public virtual bool Remove(object obj)
        {
            lock (this._syncRoot)
            {
                int index = this.Find(obj);
                if (index < 0)
                {
                    return false;
                }
                return this.RemoveAt(index);
            }
        }

        protected bool RemoveAt(int index)
        {
            if ((index < 0) || (index >= this._LiveList.Count))
            {
                return false;
            }
            this.DoCopyOnWriteCheck();
            this._LiveList.RemoveAt(index);
            return true;
        }

        public ArrayList List
        {
            get
            {
                lock (this._syncRoot)
                {
                    if (this._readonlyWrapper == null)
                    {
                        this._readonlyWrapper = ArrayList.ReadOnly(this._LiveList);
                    }
                    return this._readonlyWrapper;
                }
            }
        }

        protected ArrayList LiveList =>
            this._LiveList;

        protected object SyncRoot =>
            this._syncRoot;
    }
}

