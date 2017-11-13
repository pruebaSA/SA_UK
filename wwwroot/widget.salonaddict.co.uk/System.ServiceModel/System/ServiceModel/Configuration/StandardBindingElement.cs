namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public abstract class StandardBindingElement : ConfigurationElement, IBindingConfigurationElement, IConfigurationContextProviderInternal
    {
        [SecurityCritical]
        private EvaluationContextHelper contextHelper;
        private ConfigurationPropertyCollection properties;

        protected StandardBindingElement() : this(null)
        {
        }

        protected StandardBindingElement(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
        }

        public void ApplyConfiguration(Binding binding)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            }
            if (binding.GetType() != this.BindingElementType)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("ConfigInvalidTypeForBinding", new object[] { this.BindingElementType.AssemblyQualifiedName, binding.GetType().AssemblyQualifiedName }));
            }
            binding.CloseTimeout = this.CloseTimeout;
            binding.OpenTimeout = this.OpenTimeout;
            binding.ReceiveTimeout = this.ReceiveTimeout;
            binding.SendTimeout = this.SendTimeout;
            this.OnApplyConfiguration(binding);
        }

        protected internal virtual void InitializeFrom(Binding binding)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            }
            if (binding.GetType() != this.BindingElementType)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("ConfigInvalidTypeForBinding", new object[] { this.BindingElementType.AssemblyQualifiedName, binding.GetType().AssemblyQualifiedName }));
            }
            this.CloseTimeout = binding.CloseTimeout;
            this.OpenTimeout = binding.OpenTimeout;
            this.ReceiveTimeout = binding.ReceiveTimeout;
            this.SendTimeout = binding.SendTimeout;
        }

        protected abstract void OnApplyConfiguration(Binding binding);
        [SecurityCritical]
        protected override void Reset(ConfigurationElement parentElement)
        {
            this.contextHelper.OnReset(parentElement);
            base.Reset(parentElement);
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        [SecurityCritical]
        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            this.contextHelper.GetOriginalContext(this);

        protected abstract Type BindingElementType { get; }

        [TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("closeTimeout", DefaultValue="00:01:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan CloseTimeout
        {
            get => 
                ((TimeSpan) base["closeTimeout"]);
            set
            {
                base["closeTimeout"] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("name", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string Name
        {
            get => 
                ((string) base["name"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["name"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("openTimeout", DefaultValue="00:01:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan OpenTimeout
        {
            get => 
                ((TimeSpan) base["openTimeout"]);
            set
            {
                base["openTimeout"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("name", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("closeTimeout", typeof(TimeSpan), TimeSpan.Parse("00:01:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("openTimeout", typeof(TimeSpan), TimeSpan.Parse("00:01:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("receiveTimeout", typeof(TimeSpan), TimeSpan.Parse("00:10:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("sendTimeout", typeof(TimeSpan), TimeSpan.Parse("00:01:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("receiveTimeout", DefaultValue="00:10:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan ReceiveTimeout
        {
            get => 
                ((TimeSpan) base["receiveTimeout"]);
            set
            {
                base["receiveTimeout"] = value;
            }
        }

        [ConfigurationProperty("sendTimeout", DefaultValue="00:01:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan SendTimeout
        {
            get => 
                ((TimeSpan) base["sendTimeout"]);
            set
            {
                base["sendTimeout"] = value;
            }
        }
    }
}

