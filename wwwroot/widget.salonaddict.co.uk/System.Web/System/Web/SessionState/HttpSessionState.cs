namespace System.Web.SessionState
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpSessionState : ICollection, IEnumerable
    {
        private IHttpSessionState _container;

        internal HttpSessionState(IHttpSessionState container)
        {
            this._container = container;
        }

        public void Abandon()
        {
            this._container.Abandon();
        }

        public void Add(string name, object value)
        {
            this._container[name] = value;
        }

        public void Clear()
        {
            this._container.Clear();
        }

        public void CopyTo(Array array, int index)
        {
            this._container.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this._container.GetEnumerator();

        public void Remove(string name)
        {
            this._container.Remove(name);
        }

        public void RemoveAll()
        {
            this.Clear();
        }

        public void RemoveAt(int index)
        {
            this._container.RemoveAt(index);
        }

        public int CodePage
        {
            get => 
                this._container.CodePage;
            set
            {
                this._container.CodePage = value;
            }
        }

        internal IHttpSessionState Container =>
            this._container;

        public HttpSessionState Contents =>
            this;

        public HttpCookieMode CookieMode =>
            this._container.CookieMode;

        public int Count =>
            this._container.Count;

        public bool IsCookieless =>
            this._container.IsCookieless;

        public bool IsNewSession =>
            this._container.IsNewSession;

        public bool IsReadOnly =>
            this._container.IsReadOnly;

        public bool IsSynchronized =>
            this._container.IsSynchronized;

        public object this[string name]
        {
            get => 
                this._container[name];
            set
            {
                this._container[name] = value;
            }
        }

        public object this[int index]
        {
            get => 
                this._container[index];
            set
            {
                this._container[index] = value;
            }
        }

        public NameObjectCollectionBase.KeysCollection Keys =>
            this._container.Keys;

        public int LCID
        {
            get => 
                this._container.LCID;
            set
            {
                this._container.LCID = value;
            }
        }

        public SessionStateMode Mode =>
            this._container.Mode;

        public string SessionID =>
            this._container.SessionID;

        public HttpStaticObjectsCollection StaticObjects =>
            this._container.StaticObjects;

        public object SyncRoot =>
            this._container.SyncRoot;

        public int Timeout
        {
            get => 
                this._container.Timeout;
            set
            {
                this._container.Timeout = value;
            }
        }
    }
}

