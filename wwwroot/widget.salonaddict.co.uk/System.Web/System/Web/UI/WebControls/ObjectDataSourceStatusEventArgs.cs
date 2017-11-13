namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceStatusEventArgs : EventArgs
    {
        private int _affectedRows;
        private System.Exception _exception;
        private bool _exceptionHandled;
        private IDictionary _outputParameters;
        private object _returnValue;

        public ObjectDataSourceStatusEventArgs(object returnValue, IDictionary outputParameters) : this(returnValue, outputParameters, null)
        {
        }

        public ObjectDataSourceStatusEventArgs(object returnValue, IDictionary outputParameters, System.Exception exception)
        {
            this._affectedRows = -1;
            this._returnValue = returnValue;
            this._outputParameters = outputParameters;
            this._exception = exception;
        }

        public int AffectedRows
        {
            get => 
                this._affectedRows;
            set
            {
                this._affectedRows = value;
            }
        }

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

        public IDictionary OutputParameters =>
            this._outputParameters;

        public object ReturnValue =>
            this._returnValue;
    }
}

