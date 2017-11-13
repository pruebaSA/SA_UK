namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public abstract class BaseChannelObjectWithProperties : IDictionary, ICollection, IEnumerable
    {
        protected BaseChannelObjectWithProperties()
        {
        }

        public virtual void Add(object key, object value)
        {
            throw new NotSupportedException();
        }

        public virtual void Clear()
        {
            throw new NotSupportedException();
        }

        public virtual bool Contains(object key)
        {
            if (key != null)
            {
                ICollection keys = this.Keys;
                if (keys == null)
                {
                    return false;
                }
                string strA = key as string;
                foreach (object obj2 in keys)
                {
                    if (strA != null)
                    {
                        string strB = obj2 as string;
                        if (strB != null)
                        {
                            if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                continue;
                            }
                            return true;
                        }
                    }
                    if (key.Equals(obj2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public virtual IDictionaryEnumerator GetEnumerator() => 
            new DictionaryEnumeratorByKeys(this);

        public virtual void Remove(object key)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new DictionaryEnumeratorByKeys(this);

        public virtual int Count
        {
            get
            {
                ICollection keys = this.Keys;
                return keys?.Count;
            }
        }

        public virtual bool IsFixedSize =>
            true;

        public virtual bool IsReadOnly =>
            false;

        public virtual bool IsSynchronized =>
            false;

        public virtual object this[object key]
        {
            get => 
                null;
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual ICollection Keys =>
            null;

        public virtual IDictionary Properties =>
            this;

        public virtual object SyncRoot =>
            this;

        public virtual ICollection Values
        {
            get
            {
                ICollection keys = this.Keys;
                if (keys == null)
                {
                    return null;
                }
                ArrayList list = new ArrayList();
                foreach (object obj2 in keys)
                {
                    list.Add(this[obj2]);
                }
                return list;
            }
        }
    }
}

