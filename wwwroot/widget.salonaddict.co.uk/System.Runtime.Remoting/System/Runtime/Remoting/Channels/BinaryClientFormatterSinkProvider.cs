namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security.Permissions;

    public class BinaryClientFormatterSinkProvider : IClientFormatterSinkProvider, IClientChannelSinkProvider
    {
        private bool _includeVersioning;
        private IClientChannelSinkProvider _next;
        private bool _strictBinding;

        public BinaryClientFormatterSinkProvider()
        {
            this._includeVersioning = true;
        }

        public BinaryClientFormatterSinkProvider(IDictionary properties, ICollection providerData)
        {
            this._includeVersioning = true;
            if (properties != null)
            {
                foreach (DictionaryEntry entry in properties)
                {
                    string str2 = entry.Key.ToString();
                    if (str2 != null)
                    {
                        if (str2 == "includeVersions")
                        {
                            this._includeVersioning = Convert.ToBoolean(entry.Value, CultureInfo.InvariantCulture);
                        }
                        else if (str2 == "strictBinding")
                        {
                            goto Label_006F;
                        }
                    }
                    continue;
                Label_006F:
                    this._strictBinding = Convert.ToBoolean(entry.Value, CultureInfo.InvariantCulture);
                }
            }
            CoreChannel.VerifyNoProviderData(base.GetType().Name, providerData);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            IClientChannelSink nextSink = null;
            if (this._next != null)
            {
                nextSink = this._next.CreateSink(channel, url, remoteChannelData);
                if (nextSink == null)
                {
                    return null;
                }
            }
            SinkChannelProtocol protocol = CoreChannel.DetermineChannelProtocol(channel);
            return new BinaryClientFormatterSink(nextSink) { 
                IncludeVersioning = this._includeVersioning,
                StrictBinding = this._strictBinding,
                ChannelProtocol = protocol
            };
        }

        public IClientChannelSinkProvider Next
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
            get => 
                this._next;
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
            set
            {
                this._next = value;
            }
        }
    }
}

