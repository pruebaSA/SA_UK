namespace System.Runtime.Serialization.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.Serialization;
    using System.Xml;

    public sealed class ParameterElement : ConfigurationElement
    {
        internal readonly Guid identity;
        private ConfigurationPropertyCollection properties;

        public ParameterElement()
        {
            this.identity = Guid.NewGuid();
        }

        public ParameterElement(int index) : this()
        {
            this.Index = index;
        }

        public ParameterElement(string typeName) : this()
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
            }
            this.Type = typeName;
        }

        internal System.Type GetType(string rootType, System.Type[] typeArgs) => 
            TypeElement.GetType(rootType, typeArgs, this.Type, this.Index, this.Parameters);

        protected override void PostDeserialize()
        {
            this.Validate();
        }

        protected override void PreSerialize(XmlWriter writer)
        {
            this.Validate();
        }

        private void Validate()
        {
            PropertyInformationCollection properties = base.ElementInformation.Properties;
            if ((properties["index"].ValueOrigin == PropertyValueOrigin.Default) && (properties["type"].ValueOrigin == PropertyValueOrigin.Default))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.Runtime.Serialization.SR.GetString("ConfigMustSetTypeOrIndex")));
            }
            if ((properties["index"].ValueOrigin != PropertyValueOrigin.Default) && (properties["type"].ValueOrigin != PropertyValueOrigin.Default))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.Runtime.Serialization.SR.GetString("ConfigMustOnlySetTypeOrIndex")));
            }
            if ((properties["index"].ValueOrigin != PropertyValueOrigin.Default) && (this.Parameters.Count > 0))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.Runtime.Serialization.SR.GetString("ConfigMustOnlyAddParamsWithType")));
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("index", DefaultValue=0)]
        public int Index
        {
            get => 
                ((int) base["index"]);
            set
            {
                base["index"] = value;
            }
        }

        [ConfigurationProperty("", DefaultValue=null, Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ParameterElementCollection Parameters =>
            ((ParameterElementCollection) base[""]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
                        new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("type", DefaultValue="")]
        public string Type
        {
            get => 
                ((string) base["type"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["type"] = value;
            }
        }
    }
}

