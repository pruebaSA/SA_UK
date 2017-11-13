﻿namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpFileCollectionBase : NameObjectCollectionBase, ICollection, IEnumerable
    {
        protected HttpFileCollectionBase()
        {
        }

        public virtual void CopyTo(Array dest, int index)
        {
            throw new NotImplementedException();
        }

        public virtual HttpPostedFileBase Get(int index)
        {
            throw new NotImplementedException();
        }

        public virtual HttpPostedFileBase Get(string name)
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

        public virtual string[] AllKeys
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

        public virtual HttpPostedFileBase this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpPostedFileBase this[int index]
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

