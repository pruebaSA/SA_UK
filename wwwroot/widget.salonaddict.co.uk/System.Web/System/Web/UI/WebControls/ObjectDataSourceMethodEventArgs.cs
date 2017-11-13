namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceMethodEventArgs : CancelEventArgs
    {
        private IOrderedDictionary _inputParameters;

        public ObjectDataSourceMethodEventArgs(IOrderedDictionary inputParameters)
        {
            this._inputParameters = inputParameters;
        }

        public IOrderedDictionary InputParameters =>
            this._inputParameters;
    }
}

