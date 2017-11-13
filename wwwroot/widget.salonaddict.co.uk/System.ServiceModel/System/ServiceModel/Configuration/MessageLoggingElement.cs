namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class MessageLoggingElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [ConfigurationProperty("filters", DefaultValue=null)]
        public XPathMessageFilterElementCollection Filters =>
            ((XPathMessageFilterElementCollection) base["filters"]);

        [ConfigurationProperty("logEntireMessage", DefaultValue=false)]
        public bool LogEntireMessage
        {
            get => 
                ((bool) base["logEntireMessage"]);
            set
            {
                base["logEntireMessage"] = value;
            }
        }

        [ConfigurationProperty("logMalformedMessages", DefaultValue=false)]
        public bool LogMalformedMessages
        {
            get => 
                ((bool) base["logMalformedMessages"]);
            set
            {
                base["logMalformedMessages"] = value;
            }
        }

        [ConfigurationProperty("logMessagesAtServiceLevel", DefaultValue=false)]
        public bool LogMessagesAtServiceLevel
        {
            get => 
                ((bool) base["logMessagesAtServiceLevel"]);
            set
            {
                base["logMessagesAtServiceLevel"] = value;
            }
        }

        [ConfigurationProperty("logMessagesAtTransportLevel", DefaultValue=false)]
        public bool LogMessagesAtTransportLevel
        {
            get => 
                ((bool) base["logMessagesAtTransportLevel"]);
            set
            {
                base["logMessagesAtTransportLevel"] = value;
            }
        }

        [IntegerValidator(MinValue=-1), ConfigurationProperty("maxMessagesToLog", DefaultValue=0x2710)]
        public int MaxMessagesToLog
        {
            get => 
                ((int) base["maxMessagesToLog"]);
            set
            {
                base["maxMessagesToLog"] = value;
            }
        }

        [IntegerValidator(MinValue=-1), ConfigurationProperty("maxSizeOfMessageToLog", DefaultValue=0x40000)]
        public int MaxSizeOfMessageToLog
        {
            get => 
                ((int) base["maxSizeOfMessageToLog"]);
            set
            {
                base["maxSizeOfMessageToLog"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("logEntireMessage", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("logMalformedMessages", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("logMessagesAtServiceLevel", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("logMessagesAtTransportLevel", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxMessagesToLog", typeof(int), 0x2710, null, new IntegerValidator(-1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxSizeOfMessageToLog", typeof(int), 0x40000, null, new IntegerValidator(-1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("filters", typeof(XPathMessageFilterElementCollection), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

