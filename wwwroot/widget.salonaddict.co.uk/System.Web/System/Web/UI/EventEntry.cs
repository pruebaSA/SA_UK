namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class EventEntry
    {
        private string _handlerMethodName;
        private Type _handlerType;
        private string _name;

        public string HandlerMethodName
        {
            get => 
                this._handlerMethodName;
            set
            {
                this._handlerMethodName = value;
            }
        }

        public Type HandlerType
        {
            get => 
                this._handlerType;
            set
            {
                this._handlerType = value;
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
    }
}

