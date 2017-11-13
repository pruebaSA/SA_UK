namespace System.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpFileCollection : NameObjectCollectionBase
    {
        private HttpPostedFile[] _all;
        private string[] _allKeys;

        internal HttpFileCollection() : base(Misc.CaseInsensitiveInvariantKeyComparer)
        {
        }

        internal void AddFile(string key, HttpPostedFile file)
        {
            this.ThrowIfMaxHttpCollectionKeysExceeded();
            this._all = null;
            this._allKeys = null;
            base.BaseAdd(key, file);
        }

        public void CopyTo(Array dest, int index)
        {
            if (this._all == null)
            {
                int count = this.Count;
                this._all = new HttpPostedFile[count];
                for (int i = 0; i < count; i++)
                {
                    this._all[i] = this.Get(i);
                }
            }
            if (this._all != null)
            {
                this._all.CopyTo(dest, index);
            }
        }

        public HttpPostedFile Get(int index) => 
            ((HttpPostedFile) base.BaseGet(index));

        public HttpPostedFile Get(string name) => 
            ((HttpPostedFile) base.BaseGet(name));

        public string GetKey(int index) => 
            base.BaseGetKey(index);

        private void ThrowIfMaxHttpCollectionKeysExceeded()
        {
            if (this.Count >= AppSettings.MaxHttpCollectionKeys)
            {
                throw new InvalidOperationException();
            }
        }

        public string[] AllKeys
        {
            get
            {
                if (this._allKeys == null)
                {
                    this._allKeys = base.BaseGetAllKeys();
                }
                return this._allKeys;
            }
        }

        public HttpPostedFile this[string name] =>
            this.Get(name);

        public HttpPostedFile this[int index] =>
            this.Get(index);
    }
}

