﻿namespace System.ServiceModel.Activation.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;

    public sealed class NetTcpSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static NetTcpSection GetSection()
        {
            NetTcpSection section = (NetTcpSection) ConfigurationManager.GetSection(System.ServiceModel.Activation.Configuration.ConfigurationStrings.NetTcpSectionPath);
            if (section == null)
            {
                section = new NetTcpSection();
            }
            return section;
        }

        protected override void InitializeDefault()
        {
            this.AllowAccounts.SetDefaultIdentifiers();
        }

        [ConfigurationProperty("allowAccounts")]
        public SecurityIdentifierElementCollection AllowAccounts =>
            ((SecurityIdentifierElementCollection) base["allowAccounts"]);

        [ConfigurationProperty("listenBacklog", DefaultValue=10), IntegerValidator(MinValue=1)]
        public int ListenBacklog
        {
            get => 
                ((int) base["listenBacklog"]);
            set
            {
                base["listenBacklog"] = value;
            }
        }

        [ConfigurationProperty("maxPendingAccepts", DefaultValue=2), IntegerValidator(MinValue=1)]
        public int MaxPendingAccepts
        {
            get => 
                ((int) base["maxPendingAccepts"]);
            set
            {
                base["maxPendingAccepts"] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxPendingConnections", DefaultValue=100)]
        public int MaxPendingConnections
        {
            get => 
                ((int) base["maxPendingConnections"]);
            set
            {
                base["maxPendingConnections"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("allowAccounts", typeof(SecurityIdentifierElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("listenBacklog", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxPendingConnections", typeof(int), 100, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxPendingAccepts", typeof(int), 2, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("receiveTimeout", typeof(TimeSpan), TimeSpan.Parse("00:00:10"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("teredoEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("receiveTimeout", DefaultValue="00:00:10"), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan ReceiveTimeout
        {
            get => 
                ((TimeSpan) base["receiveTimeout"]);
            set
            {
                base["receiveTimeout"] = value;
            }
        }

        [ConfigurationProperty("teredoEnabled", DefaultValue=false)]
        public bool TeredoEnabled
        {
            get => 
                ((bool) base["teredoEnabled"]);
            set
            {
                base["teredoEnabled"] = value;
            }
        }
    }
}

