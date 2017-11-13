namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewDeletedEventArgs : EventArgs
    {
        private int _affectedRows;
        private System.Exception _exception;
        private bool _exceptionHandled;
        private IOrderedDictionary _keys;
        private IOrderedDictionary _values;

        public ListViewDeletedEventArgs(int affectedRows, System.Exception exception)
        {
            this._affectedRows = affectedRows;
            this._exceptionHandled = false;
            this._exception = exception;
        }

        internal void SetKeys(IOrderedDictionary keys)
        {
            this._keys = keys;
        }

        internal void SetValues(IOrderedDictionary values)
        {
            this._values = values;
        }

        public int AffectedRows =>
            this._affectedRows;

        public System.Exception Exception =>
            this._exception;

        public bool ExceptionHandled
        {
            get => 
                this._exceptionHandled;
            set
            {
                this._exceptionHandled = value;
            }
        }

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

        public IOrderedDictionary Values
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
    }
}

