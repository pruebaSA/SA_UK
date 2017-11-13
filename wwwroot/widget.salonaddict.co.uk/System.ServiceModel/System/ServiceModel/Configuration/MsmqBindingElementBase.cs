namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public abstract class MsmqBindingElementBase : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        protected MsmqBindingElementBase() : this(null)
        {
        }

        protected MsmqBindingElementBase(string name) : base(name)
        {
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            MsmqBindingBase base2 = (MsmqBindingBase) binding;
            this.DeadLetterQueue = base2.DeadLetterQueue;
            if (base2.CustomDeadLetterQueue != null)
            {
                this.CustomDeadLetterQueue = base2.CustomDeadLetterQueue;
            }
            this.Durable = base2.Durable;
            this.ExactlyOnce = base2.ExactlyOnce;
            this.MaxReceivedMessageSize = base2.MaxReceivedMessageSize;
            this.MaxRetryCycles = base2.MaxRetryCycles;
            this.ReceiveErrorHandling = base2.ReceiveErrorHandling;
            this.ReceiveRetryCount = base2.ReceiveRetryCount;
            this.RetryCycleDelay = base2.RetryCycleDelay;
            this.TimeToLive = base2.TimeToLive;
            this.UseSourceJournal = base2.UseSourceJournal;
            this.UseMsmqTracing = base2.UseMsmqTracing;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            MsmqBindingBase base2 = (MsmqBindingBase) binding;
            if (this.CustomDeadLetterQueue != null)
            {
                base2.CustomDeadLetterQueue = this.CustomDeadLetterQueue;
            }
            base2.DeadLetterQueue = this.DeadLetterQueue;
            base2.Durable = this.Durable;
            base2.ExactlyOnce = this.ExactlyOnce;
            base2.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            base2.MaxRetryCycles = this.MaxRetryCycles;
            base2.ReceiveErrorHandling = this.ReceiveErrorHandling;
            base2.ReceiveRetryCount = this.ReceiveRetryCount;
            base2.RetryCycleDelay = this.RetryCycleDelay;
            base2.TimeToLive = this.TimeToLive;
            base2.UseSourceJournal = this.UseSourceJournal;
            base2.UseMsmqTracing = this.UseMsmqTracing;
        }

        [ConfigurationProperty("customDeadLetterQueue", DefaultValue=null)]
        public Uri CustomDeadLetterQueue
        {
            get => 
                ((Uri) base["customDeadLetterQueue"]);
            set
            {
                base["customDeadLetterQueue"] = value;
            }
        }

        [ServiceModelEnumValidator(typeof(DeadLetterQueueHelper)), ConfigurationProperty("deadLetterQueue", DefaultValue=1)]
        public System.ServiceModel.DeadLetterQueue DeadLetterQueue
        {
            get => 
                ((System.ServiceModel.DeadLetterQueue) base["deadLetterQueue"]);
            set
            {
                base["deadLetterQueue"] = value;
            }
        }

        [ConfigurationProperty("durable", DefaultValue=true)]
        public bool Durable
        {
            get => 
                ((bool) base["durable"]);
            set
            {
                base["durable"] = value;
            }
        }

        [ConfigurationProperty("exactlyOnce", DefaultValue=true)]
        public bool ExactlyOnce
        {
            get => 
                ((bool) base["exactlyOnce"]);
            set
            {
                base["exactlyOnce"] = value;
            }
        }

        [ConfigurationProperty("maxReceivedMessageSize", DefaultValue=0x10000L), LongValidator(MinValue=0L)]
        public long MaxReceivedMessageSize
        {
            get => 
                ((long) base["maxReceivedMessageSize"]);
            set
            {
                base["maxReceivedMessageSize"] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxRetryCycles", DefaultValue=2)]
        public int MaxRetryCycles
        {
            get => 
                ((int) base["maxRetryCycles"]);
            set
            {
                base["maxRetryCycles"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("customDeadLetterQueue", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("deadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), System.ServiceModel.DeadLetterQueue.System, null, new ServiceModelEnumValidator(typeof(DeadLetterQueueHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("durable", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("exactlyOnce", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(0L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxRetryCycles", typeof(int), 2, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("receiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), System.ServiceModel.ReceiveErrorHandling.Fault, null, new ServiceModelEnumValidator(typeof(ReceiveErrorHandlingHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("receiveRetryCount", typeof(int), 5, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("retryCycleDelay", typeof(TimeSpan), TimeSpan.Parse("00:30:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("timeToLive", typeof(TimeSpan), TimeSpan.Parse("1.00:00:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useSourceJournal", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useMsmqTracing", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("receiveErrorHandling", DefaultValue=0), ServiceModelEnumValidator(typeof(ReceiveErrorHandlingHelper))]
        public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling
        {
            get => 
                ((System.ServiceModel.ReceiveErrorHandling) base["receiveErrorHandling"]);
            set
            {
                base["receiveErrorHandling"] = value;
            }
        }

        [ConfigurationProperty("receiveRetryCount", DefaultValue=5), IntegerValidator(MinValue=0)]
        public int ReceiveRetryCount
        {
            get => 
                ((int) base["receiveRetryCount"]);
            set
            {
                base["receiveRetryCount"] = value;
            }
        }

        [ConfigurationProperty("retryCycleDelay", DefaultValue="00:30:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan RetryCycleDelay
        {
            get => 
                ((TimeSpan) base["retryCycleDelay"]);
            set
            {
                base["retryCycleDelay"] = value;
            }
        }

        [TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("timeToLive", DefaultValue="1.00:00:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan TimeToLive
        {
            get => 
                ((TimeSpan) base["timeToLive"]);
            set
            {
                base["timeToLive"] = value;
            }
        }

        [ConfigurationProperty("useMsmqTracing", DefaultValue=false)]
        public bool UseMsmqTracing
        {
            get => 
                ((bool) base["useMsmqTracing"]);
            set
            {
                base["useMsmqTracing"] = value;
            }
        }

        [ConfigurationProperty("useSourceJournal", DefaultValue=false)]
        public bool UseSourceJournal
        {
            get => 
                ((bool) base["useSourceJournal"]);
            set
            {
                base["useSourceJournal"] = value;
            }
        }
    }
}

