namespace System.Runtime.Remoting.MetadataServices
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Remoting.Channels;
    using System.Security.Permissions;

    public class SdlChannelSinkProvider : IServerChannelSinkProvider
    {
        private bool _bMetadataEnabled;
        private bool _bRemoteApplicationMetadataEnabled;
        private IServerChannelSinkProvider _next;

        public SdlChannelSinkProvider()
        {
            this._bMetadataEnabled = true;
        }

        public SdlChannelSinkProvider(IDictionary properties, ICollection providerData)
        {
            this._bMetadataEnabled = true;
            if (properties != null)
            {
                foreach (DictionaryEntry entry in properties)
                {
                    switch (((string) entry.Key))
                    {
                        case "remoteApplicationMetadataEnabled":
                            this._bRemoteApplicationMetadataEnabled = Convert.ToBoolean(entry.Value, CultureInfo.InvariantCulture);
                            break;

                        case "metadataEnabled":
                            this._bMetadataEnabled = Convert.ToBoolean(entry.Value, CultureInfo.InvariantCulture);
                            break;

                        default:
                            CoreChannel.ReportUnknownProviderConfigProperty(base.GetType().Name, (string) entry.Key);
                            break;
                    }
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;
            if (this._next != null)
            {
                nextSink = this._next.CreateSink(channel);
            }
            return new SdlChannelSink(channel, nextSink) { 
                RemoteApplicationMetadataEnabled = this._bRemoteApplicationMetadataEnabled,
                MetadataEnabled = this._bMetadataEnabled
            };
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public void GetChannelData(IChannelDataStore localChannelData)
        {
        }

        public IServerChannelSinkProvider Next
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

