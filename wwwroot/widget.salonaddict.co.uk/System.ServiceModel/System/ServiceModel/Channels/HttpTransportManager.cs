namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    internal abstract class HttpTransportManager : TransportManager, ITransportManagerRegistration
    {
        private Dictionary<string, UriPrefixTable<HttpChannelListener>> addressTables;
        private System.ServiceModel.HostNameComparisonMode hostNameComparisonMode;
        private Uri listenUri;
        private string realm;

        internal HttpTransportManager()
        {
            this.addressTables = new Dictionary<string, UriPrefixTable<HttpChannelListener>>();
        }

        internal HttpTransportManager(Uri listenUri, System.ServiceModel.HostNameComparisonMode hostNameComparisonMode, string realm) : this()
        {
            this.hostNameComparisonMode = hostNameComparisonMode;
            this.listenUri = listenUri;
            this.realm = realm;
        }

        protected void Fault(Exception exception)
        {
            lock (base.ThisLock)
            {
                foreach (KeyValuePair<string, UriPrefixTable<HttpChannelListener>> pair in this.addressTables)
                {
                    base.Fault<HttpChannelListener>(pair.Value, exception);
                }
            }
        }

        internal virtual bool IsCompatible(HttpChannelListener listener) => 
            ((this.hostNameComparisonMode == listener.HostNameComparisonMode) && (this.realm == listener.Realm));

        internal override void OnClose()
        {
            this.TransportManagerTable.UnregisterUri(this.ListenUri, this.HostNameComparisonMode);
        }

        internal override void Register(TransportChannelListener channelListener)
        {
            UriPrefixTable<HttpChannelListener> table;
            lock (this.addressTables)
            {
                string method = ((HttpChannelListener) channelListener).Method;
                if (!this.addressTables.TryGetValue(method, out table))
                {
                    table = new UriPrefixTable<HttpChannelListener>();
                    this.addressTables[method] = table;
                }
            }
            table.RegisterUri(channelListener.Uri, channelListener.InheritBaseAddressSettings ? this.hostNameComparisonMode : channelListener.HostNameComparisonModeInternal, (HttpChannelListener) channelListener);
        }

        IList<TransportManager> ITransportManagerRegistration.Select(TransportChannelListener channelListener)
        {
            IList<TransportManager> list = null;
            if (this.IsCompatible((HttpChannelListener) channelListener))
            {
                list = new List<TransportManager> {
                    this
                };
            }
            return list;
        }

        protected bool TryLookupUri(Uri requestUri, string requestMethod, System.ServiceModel.HostNameComparisonMode hostNameComparisonMode, out HttpChannelListener listener)
        {
            listener = null;
            if (requestMethod == null)
            {
                requestMethod = string.Empty;
            }
            lock (this.addressTables)
            {
                UriPrefixTable<HttpChannelListener> table;
                HttpChannelListener item = null;
                if (((requestMethod.Length > 0) && this.addressTables.TryGetValue(requestMethod, out table)) && (table.TryLookupUri(requestUri, hostNameComparisonMode, out item) && (string.Compare(requestUri.AbsolutePath, item.Uri.AbsolutePath, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    item = null;
                }
                if (this.addressTables.TryGetValue(string.Empty, out table) && table.TryLookupUri(requestUri, hostNameComparisonMode, out listener))
                {
                    if ((item != null) && (item.Uri.AbsoluteUri.Length >= listener.Uri.AbsoluteUri.Length))
                    {
                        listener = item;
                    }
                }
                else
                {
                    listener = item;
                }
            }
            return (listener != null);
        }

        internal override void Unregister(TransportChannelListener channelListener)
        {
            UriPrefixTable<HttpChannelListener> table;
            lock (this.addressTables)
            {
                string method = ((HttpChannelListener) channelListener).Method;
                if (!this.addressTables.TryGetValue(method, out table))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ListenerFactoryNotRegistered", new object[] { channelListener.Uri })));
                }
            }
            System.ServiceModel.HostNameComparisonMode registeredComparisonMode = channelListener.InheritBaseAddressSettings ? this.hostNameComparisonMode : channelListener.HostNameComparisonModeInternal;
            TransportManager.EnsureRegistered<HttpChannelListener>(table, (HttpChannelListener) channelListener, registeredComparisonMode);
            table.UnregisterUri(channelListener.Uri, registeredComparisonMode);
        }

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                this.hostNameComparisonMode;
            protected set
            {
                lock (base.ThisLock)
                {
                    base.ThrowIfOpen();
                    this.hostNameComparisonMode = value;
                }
            }
        }

        public Uri ListenUri
        {
            get => 
                this.listenUri;
            protected set
            {
                this.listenUri = value;
            }
        }

        internal string Realm =>
            this.realm;

        internal override string Scheme =>
            Uri.UriSchemeHttp;

        internal virtual UriPrefixTable<ITransportManagerRegistration> TransportManagerTable =>
            HttpChannelListener.StaticTransportManagerTable;
    }
}

