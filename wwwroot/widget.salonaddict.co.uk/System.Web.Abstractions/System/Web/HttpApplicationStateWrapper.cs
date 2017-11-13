namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpApplicationStateWrapper : HttpApplicationStateBase
    {
        private HttpApplicationState _application;

        public HttpApplicationStateWrapper(HttpApplicationState httpApplicationState)
        {
            if (httpApplicationState == null)
            {
                throw new ArgumentNullException("httpApplicationState");
            }
            this._application = httpApplicationState;
        }

        public override void Add(string name, object value)
        {
            this._application.Add(name, value);
        }

        public override void Clear()
        {
            this._application.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            ((ICollection) this._application).CopyTo(array, index);
        }

        public override object Get(int index) => 
            this._application.Get(index);

        public override object Get(string name) => 
            this._application.Get(name);

        public override IEnumerator GetEnumerator() => 
            this._application.GetEnumerator();

        public override string GetKey(int index) => 
            this._application.GetKey(index);

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this._application.GetObjectData(info, context);
        }

        public override void Lock()
        {
            this._application.Lock();
        }

        public override void OnDeserialization(object sender)
        {
            this._application.OnDeserialization(sender);
        }

        public override void Remove(string name)
        {
            this._application.Remove(name);
        }

        public override void RemoveAll()
        {
            this._application.RemoveAll();
        }

        public override void RemoveAt(int index)
        {
            this._application.RemoveAt(index);
        }

        public override void Set(string name, object value)
        {
            this._application.Set(name, value);
        }

        public override void UnLock()
        {
            this._application.UnLock();
        }

        public override string[] AllKeys =>
            this._application.AllKeys;

        public override HttpApplicationStateBase Contents =>
            this;

        public override int Count =>
            this._application.Count;

        public override bool IsSynchronized =>
            this._application.IsSynchronized;

        public override object this[int index] =>
            this._application[index];

        public override object this[string name]
        {
            get => 
                this._application[name];
            set
            {
                this._application[name] = value;
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys =>
            this._application.Keys;

        public override HttpStaticObjectsCollectionBase StaticObjects =>
            new HttpStaticObjectsCollectionWrapper(this._application.StaticObjects);

        public override object SyncRoot =>
            this._application.SyncRoot;
    }
}

