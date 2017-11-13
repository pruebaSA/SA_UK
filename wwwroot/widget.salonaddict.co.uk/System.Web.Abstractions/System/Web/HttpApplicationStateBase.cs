namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpApplicationStateBase : NameObjectCollectionBase, ICollection, IEnumerable
    {
        protected HttpApplicationStateBase()
        {
        }

        public virtual void Add(string name, object value)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(int index)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual string GetKey(int index)
        {
            throw new NotImplementedException();
        }

        public virtual void Lock()
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(string name)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public virtual void Set(string name, object value)
        {
            throw new NotImplementedException();
        }

        public virtual void UnLock()
        {
            throw new NotImplementedException();
        }

        public virtual string[] AllKeys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpApplicationStateBase Contents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual object this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpStaticObjectsCollectionBase StaticObjects
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

