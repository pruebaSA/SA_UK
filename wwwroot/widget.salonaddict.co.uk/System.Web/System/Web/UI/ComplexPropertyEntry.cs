namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ComplexPropertyEntry : BuilderPropertyEntry
    {
        private bool _isCollectionItem;
        private bool _readOnly;

        internal ComplexPropertyEntry()
        {
        }

        internal ComplexPropertyEntry(bool isCollectionItem)
        {
            this._isCollectionItem = isCollectionItem;
        }

        public bool IsCollectionItem =>
            this._isCollectionItem;

        public bool ReadOnly
        {
            get => 
                this._readOnly;
            set
            {
                this._readOnly = value;
            }
        }
    }
}

