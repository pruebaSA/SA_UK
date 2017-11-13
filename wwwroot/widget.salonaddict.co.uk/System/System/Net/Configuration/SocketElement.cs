namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Net;

    public sealed class SocketElement : ConfigurationElement
    {
        private readonly ConfigurationProperty alwaysUseCompletionPortsForAccept = new ConfigurationProperty("alwaysUseCompletionPortsForAccept", typeof(bool), false, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty alwaysUseCompletionPortsForConnect = new ConfigurationProperty("alwaysUseCompletionPortsForConnect", typeof(bool), false, ConfigurationPropertyOptions.None);
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        public SocketElement()
        {
            this.properties.Add(this.alwaysUseCompletionPortsForAccept);
            this.properties.Add(this.alwaysUseCompletionPortsForConnect);
        }

        protected override void PostDeserialize()
        {
            if (!base.EvaluationContext.IsMachineLevel)
            {
                try
                {
                    ExceptionHelper.UnrestrictedSocketPermission.Demand();
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException(System.SR.GetString("net_config_element_permission", new object[] { "socket" }), exception);
                }
            }
        }

        [ConfigurationProperty("alwaysUseCompletionPortsForAccept", DefaultValue=false)]
        public bool AlwaysUseCompletionPortsForAccept
        {
            get => 
                ((bool) base[this.alwaysUseCompletionPortsForAccept]);
            set
            {
                base[this.alwaysUseCompletionPortsForAccept] = value;
            }
        }

        [ConfigurationProperty("alwaysUseCompletionPortsForConnect", DefaultValue=false)]
        public bool AlwaysUseCompletionPortsForConnect
        {
            get => 
                ((bool) base[this.alwaysUseCompletionPortsForConnect]);
            set
            {
                base[this.alwaysUseCompletionPortsForConnect] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

