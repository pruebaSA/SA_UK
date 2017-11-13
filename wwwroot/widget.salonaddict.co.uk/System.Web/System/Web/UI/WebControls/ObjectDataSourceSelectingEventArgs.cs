namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceSelectingEventArgs : ObjectDataSourceMethodEventArgs
    {
        private DataSourceSelectArguments _arguments;
        private bool _executingSelectCount;

        public ObjectDataSourceSelectingEventArgs(IOrderedDictionary inputParameters, DataSourceSelectArguments arguments, bool executingSelectCount) : base(inputParameters)
        {
            this._arguments = arguments;
            this._executingSelectCount = executingSelectCount;
        }

        public DataSourceSelectArguments Arguments =>
            this._arguments;

        public bool ExecutingSelectCount =>
            this._executingSelectCount;
    }
}

