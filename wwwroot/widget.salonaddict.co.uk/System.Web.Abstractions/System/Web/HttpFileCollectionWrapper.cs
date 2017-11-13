namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpFileCollectionWrapper : HttpFileCollectionBase
    {
        private HttpFileCollection _collection;

        public HttpFileCollectionWrapper(HttpFileCollection httpFileCollection)
        {
            if (httpFileCollection == null)
            {
                throw new ArgumentNullException("httpFileCollection");
            }
            this._collection = httpFileCollection;
        }

        public override void CopyTo(Array dest, int index)
        {
            this._collection.CopyTo(dest, index);
        }

        public override HttpPostedFileBase Get(int index)
        {
            HttpPostedFile httpPostedFile = this._collection.Get(index);
            if (httpPostedFile == null)
            {
                return null;
            }
            return new HttpPostedFileWrapper(httpPostedFile);
        }

        public override HttpPostedFileBase Get(string name)
        {
            HttpPostedFile httpPostedFile = this._collection.Get(name);
            if (httpPostedFile == null)
            {
                return null;
            }
            return new HttpPostedFileWrapper(httpPostedFile);
        }

        public override IEnumerator GetEnumerator() => 
            this._collection.GetEnumerator();

        public override string GetKey(int index) => 
            this._collection.GetKey(index);

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this._collection.GetObjectData(info, context);
        }

        public override void OnDeserialization(object sender)
        {
            this._collection.OnDeserialization(sender);
        }

        public override string[] AllKeys =>
            this._collection.AllKeys;

        public override int Count =>
            this._collection.Count;

        public override bool IsSynchronized =>
            this._collection.IsSynchronized;

        public override HttpPostedFileBase this[string name]
        {
            get
            {
                HttpPostedFile httpPostedFile = this._collection[name];
                if (httpPostedFile == null)
                {
                    return null;
                }
                return new HttpPostedFileWrapper(httpPostedFile);
            }
        }

        public override HttpPostedFileBase this[int index]
        {
            get
            {
                HttpPostedFile httpPostedFile = this._collection[index];
                if (httpPostedFile == null)
                {
                    return null;
                }
                return new HttpPostedFileWrapper(httpPostedFile);
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys =>
            this._collection.Keys;

        public override object SyncRoot =>
            this._collection.SyncRoot;
    }
}

