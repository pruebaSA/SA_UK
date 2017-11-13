namespace System.Web.UI
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class PropertyEntry
    {
        private string _filter;
        private int _index;
        private string _name;
        private int _order;
        private System.Reflection.PropertyInfo _propertyInfo;
        private System.Type _type;

        internal PropertyEntry()
        {
        }

        public System.Type DeclaringType =>
            this._propertyInfo?.DeclaringType;

        public string Filter
        {
            get => 
                this._filter;
            set
            {
                this._filter = value;
            }
        }

        internal int Index
        {
            get => 
                this._index;
            set
            {
                this._index = value;
            }
        }

        public string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }

        internal int Order
        {
            get => 
                this._order;
            set
            {
                this._order = value;
            }
        }

        public System.Reflection.PropertyInfo PropertyInfo
        {
            get => 
                this._propertyInfo;
            set
            {
                this._propertyInfo = value;
            }
        }

        public System.Type Type
        {
            get => 
                this._type;
            set
            {
                this._type = value;
            }
        }
    }
}

