﻿namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RootBuilder : TemplateBuilder
    {
        private IDictionary _builtObjects;
        private MainTagNameToTypeMapper _typeMapper;

        public RootBuilder()
        {
        }

        public RootBuilder(TemplateParser parser)
        {
        }

        public override Type GetChildControlType(string tagName, IDictionary attribs) => 
            this._typeMapper.GetControlType(tagName, attribs, true);

        internal override void PrepareNoCompilePageSupport()
        {
            base.PrepareNoCompilePageSupport();
            this._typeMapper = null;
        }

        internal void SetTypeMapper(MainTagNameToTypeMapper typeMapper)
        {
            this._typeMapper = typeMapper;
        }

        public IDictionary BuiltObjects
        {
            get
            {
                if (this._builtObjects == null)
                {
                    this._builtObjects = new Hashtable(ReferenceKeyComparer.Default);
                }
                return this._builtObjects;
            }
        }

        private class ReferenceKeyComparer : IComparer, IEqualityComparer
        {
            internal static readonly RootBuilder.ReferenceKeyComparer Default = new RootBuilder.ReferenceKeyComparer();

            int IComparer.Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                return 1;
            }

            bool IEqualityComparer.Equals(object x, object y) => 
                object.ReferenceEquals(x, y);

            int IEqualityComparer.GetHashCode(object obj) => 
                obj.GetHashCode();
        }
    }
}

