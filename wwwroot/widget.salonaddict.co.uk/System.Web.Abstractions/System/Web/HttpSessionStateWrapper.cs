﻿namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web.SessionState;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpSessionStateWrapper : HttpSessionStateBase
    {
        private readonly HttpSessionState _session;

        public HttpSessionStateWrapper(HttpSessionState httpSessionState)
        {
            if (httpSessionState == null)
            {
                throw new ArgumentNullException("httpSessionState");
            }
            this._session = httpSessionState;
        }

        public override void Abandon()
        {
            this._session.Abandon();
        }

        public override void Add(string name, object value)
        {
            this._session.Add(name, value);
        }

        public override void Clear()
        {
            this._session.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            this._session.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator() => 
            this._session.GetEnumerator();

        public override void Remove(string name)
        {
            this._session.Remove(name);
        }

        public override void RemoveAll()
        {
            this._session.RemoveAll();
        }

        public override void RemoveAt(int index)
        {
            this._session.RemoveAt(index);
        }

        public override int CodePage
        {
            get => 
                this._session.CodePage;
            set
            {
                this._session.CodePage = value;
            }
        }

        public override HttpSessionStateBase Contents =>
            this;

        public override HttpCookieMode CookieMode =>
            this._session.CookieMode;

        public override int Count =>
            this._session.Count;

        public override bool IsCookieless =>
            this._session.IsCookieless;

        public override bool IsNewSession =>
            this._session.IsNewSession;

        public override bool IsReadOnly =>
            this._session.IsReadOnly;

        public override bool IsSynchronized =>
            this._session.IsSynchronized;

        public override object this[int index]
        {
            get => 
                this._session[index];
            set
            {
                this._session[index] = value;
            }
        }

        public override object this[string name]
        {
            get => 
                this._session[name];
            set
            {
                this._session[name] = value;
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys =>
            this._session.Keys;

        public override int LCID
        {
            get => 
                this._session.LCID;
            set
            {
                this._session.LCID = value;
            }
        }

        public override SessionStateMode Mode =>
            this._session.Mode;

        public override string SessionID =>
            this._session.SessionID;

        public override HttpStaticObjectsCollectionBase StaticObjects =>
            new HttpStaticObjectsCollectionWrapper(this._session.StaticObjects);

        public override object SyncRoot =>
            this._session.SyncRoot;

        public override int Timeout
        {
            get => 
                this._session.Timeout;
            set
            {
                this._session.Timeout = value;
            }
        }
    }
}

