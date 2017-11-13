namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class FormViewUpdateEventArgs : CancelEventArgs
    {
        private object _commandArgument;
        private OrderedDictionary _keys;
        private OrderedDictionary _oldValues;
        private OrderedDictionary _values;

        public FormViewUpdateEventArgs(object commandArgument) : base(false)
        {
            this._commandArgument = commandArgument;
        }

        public object CommandArgument =>
            this._commandArgument;

        public IOrderedDictionary Keys
        {
            get
            {
                if (this._keys == null)
                {
                    this._keys = new OrderedDictionary();
                }
                return this._keys;
            }
        }

        public IOrderedDictionary NewValues
        {
            get
            {
                if (this._values == null)
                {
                    this._values = new OrderedDictionary();
                }
                return this._values;
            }
        }

        public IOrderedDictionary OldValues
        {
            get
            {
                if (this._oldValues == null)
                {
                    this._oldValues = new OrderedDictionary();
                }
                return this._oldValues;
            }
        }
    }
}

