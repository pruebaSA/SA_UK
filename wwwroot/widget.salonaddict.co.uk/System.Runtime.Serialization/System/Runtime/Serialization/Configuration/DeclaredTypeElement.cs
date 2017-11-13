namespace System.Runtime.Serialization.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    public sealed class DeclaredTypeElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public DeclaredTypeElement()
        {
        }

        public DeclaredTypeElement(string typeName) : this()
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
            }
            this.Type = typeName;
        }

        protected override void PostDeserialize()
        {
            if (!base.EvaluationContext.IsMachineLevel)
            {
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                }
                catch (SecurityException exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.Runtime.Serialization.SR.GetString("ConfigDataContractSerializerSectionLoadError"), exception));
                }
            }
        }

        [ConfigurationProperty("", DefaultValue=null, Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public TypeElementCollection KnownTypes =>
            ((TypeElementCollection) base[""]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(TypeElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
                        new ConfigurationProperty("type", typeof(string), string.Empty, null, new DeclaredTypeValidator(), ConfigurationPropertyOptions.IsKey)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [DeclaredTypeValidator, ConfigurationProperty("type", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey)]
        public string Type
        {
            get => 
                ((string) base["type"]);
            set
            {
                base["type"] = value;
            }
        }
    }
}

