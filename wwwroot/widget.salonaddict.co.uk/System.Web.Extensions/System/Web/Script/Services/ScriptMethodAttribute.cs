namespace System.Web.Script.Services
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Method), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptMethodAttribute : Attribute
    {
        private System.Web.Script.Services.ResponseFormat _responseFormat;
        private bool _useHttpGet;
        private bool _xmlSerializeString;

        public System.Web.Script.Services.ResponseFormat ResponseFormat
        {
            get => 
                this._responseFormat;
            set
            {
                this._responseFormat = value;
            }
        }

        public bool UseHttpGet
        {
            get => 
                this._useHttpGet;
            set
            {
                this._useHttpGet = value;
            }
        }

        public bool XmlSerializeString
        {
            get => 
                this._xmlSerializeString;
            set
            {
                this._xmlSerializeString = value;
            }
        }
    }
}

