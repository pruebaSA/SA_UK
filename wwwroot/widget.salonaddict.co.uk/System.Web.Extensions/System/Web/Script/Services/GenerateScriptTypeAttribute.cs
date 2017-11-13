namespace System.Web.Script.Services
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class GenerateScriptTypeAttribute : Attribute
    {
        private System.Type _type;
        private string _typeId;

        public GenerateScriptTypeAttribute(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this._type = type;
        }

        public string ScriptTypeId
        {
            get => 
                (this._typeId ?? string.Empty);
            set
            {
                this._typeId = value;
            }
        }

        public System.Type Type =>
            this._type;
    }
}

