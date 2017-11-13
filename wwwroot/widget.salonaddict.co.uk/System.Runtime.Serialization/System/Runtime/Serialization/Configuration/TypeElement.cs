namespace System.Runtime.Serialization.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.Serialization;

    public sealed class TypeElement : ConfigurationElement
    {
        private string key;
        private ConfigurationPropertyCollection properties;

        public TypeElement()
        {
            this.key = Guid.NewGuid().ToString();
        }

        public TypeElement(string typeName) : this()
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
            }
            this.Type = typeName;
        }

        internal System.Type GetType(string rootType, System.Type[] typeArgs) => 
            GetType(rootType, typeArgs, this.Type, this.Index, this.Parameters);

        internal static System.Type GetType(string rootType, System.Type[] typeArgs, string type, int index, ParameterElementCollection parameters)
        {
            if (string.IsNullOrEmpty(type))
            {
                if ((typeArgs != null) && (index < typeArgs.Length))
                {
                    return typeArgs[index];
                }
                int num = (typeArgs == null) ? 0 : typeArgs.Length;
                if (num == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.Runtime.Serialization.SR.GetString("KnownTypeConfigIndexOutOfBoundsZero", new object[] { rootType, num, index }));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.Runtime.Serialization.SR.GetString("KnownTypeConfigIndexOutOfBounds", new object[] { rootType, num, index }));
            }
            System.Type type2 = System.Type.GetType(type, true);
            if (!type2.IsGenericTypeDefinition)
            {
                return type2;
            }
            if (parameters.Count != type2.GetGenericArguments().Length)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.Runtime.Serialization.SR.GetString("KnownTypeConfigGenericParamMismatch", new object[] { type, type2.GetGenericArguments().Length, parameters.Count }));
            }
            System.Type[] typeArguments = new System.Type[parameters.Count];
            for (int i = 0; i < typeArguments.Length; i++)
            {
                typeArguments[i] = parameters[i].GetType(rootType, typeArgs);
            }
            return type2.MakeGenericType(typeArguments);
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            TypeElement element = (TypeElement) parentElement;
            this.key = element.key;
            base.Reset(parentElement);
        }

        [ConfigurationProperty("index", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int Index
        {
            get => 
                ((int) base["index"]);
            set
            {
                base["index"] = value;
            }
        }

        internal string Key =>
            this.key;

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
                        new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
                        new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None)
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
                base["type"] = value;
            }
        }
    }
}

