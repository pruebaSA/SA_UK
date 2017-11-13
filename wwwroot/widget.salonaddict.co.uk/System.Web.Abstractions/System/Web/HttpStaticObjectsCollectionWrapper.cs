namespace System.Web
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpStaticObjectsCollectionWrapper : HttpStaticObjectsCollectionBase
    {
        private HttpStaticObjectsCollection _collection;

        public HttpStaticObjectsCollectionWrapper(HttpStaticObjectsCollection httpStaticObjectsCollection)
        {
            if (httpStaticObjectsCollection == null)
            {
                throw new ArgumentNullException("httpStaticObjectsCollection");
            }
            this._collection = httpStaticObjectsCollection;
        }

        public override void CopyTo(Array array, int index)
        {
            this._collection.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator() => 
            this._collection.GetEnumerator();

        public override object GetObject(string name) => 
            this._collection.GetObject(name);

        public override void Serialize(BinaryWriter writer)
        {
            this._collection.Serialize(writer);
        }

        public override int Count =>
            this._collection.Count;

        public override bool IsReadOnly =>
            this._collection.IsReadOnly;

        public override bool IsSynchronized =>
            this._collection.IsSynchronized;

        public override object this[string name] =>
            this._collection[name];

        public override bool NeverAccessed =>
            this._collection.NeverAccessed;

        public override object SyncRoot =>
            this._collection.SyncRoot;
    }
}

