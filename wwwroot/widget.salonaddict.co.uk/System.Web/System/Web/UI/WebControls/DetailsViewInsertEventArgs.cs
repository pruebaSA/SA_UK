namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DetailsViewInsertEventArgs : CancelEventArgs
    {
        private object _commandArgument;
        private OrderedDictionary _values;

        public DetailsViewInsertEventArgs(object commandArgument) : base(false)
        {
            this._commandArgument = commandArgument;
        }

        public object CommandArgument =>
            this._commandArgument;

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

