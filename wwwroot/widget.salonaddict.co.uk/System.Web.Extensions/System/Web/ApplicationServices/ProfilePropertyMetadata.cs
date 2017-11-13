namespace System.Web.ApplicationServices
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Web;

    [DataContract, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProfilePropertyMetadata
    {
        private bool _allowAnonymousAccess;
        private string _defaultValue;
        private bool _isReadOnly;
        private string _propertyName;
        private int _serializeAs;
        private string _typeName;

        [DataMember]
        public bool AllowAnonymousAccess
        {
            get => 
                this._allowAnonymousAccess;
            set
            {
                this._allowAnonymousAccess = value;
            }
        }

        [DataMember]
        public string DefaultValue
        {
            get => 
                this._defaultValue;
            set
            {
                this._defaultValue = value;
            }
        }

        [DataMember]
        public bool IsReadOnly
        {
            get => 
                this._isReadOnly;
            set
            {
                this._isReadOnly = value;
            }
        }

        [DataMember]
        public string PropertyName
        {
            get => 
                this._propertyName;
            set
            {
                this._propertyName = value;
            }
        }

        [DataMember]
        public int SerializeAs
        {
            get => 
                this._serializeAs;
            set
            {
                this._serializeAs = value;
            }
        }

        [DataMember]
        public string TypeName
        {
            get => 
                this._typeName;
            set
            {
                this._typeName = value;
            }
        }
    }
}

