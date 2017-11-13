namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class HttpHostedTransportConfiguration : HostedTransportConfigurationBase
    {
        private static bool canDebugPrint = true;
        private Collection<HostedHttpTransportManager> transportManagerDirectory;

        internal HttpHostedTransportConfiguration() : this(Uri.UriSchemeHttp)
        {
        }

        protected internal HttpHostedTransportConfiguration(string scheme) : base(scheme)
        {
            this.CreateTransportManagers();
        }

        private HostedHttpTransportManager CreateTransportManager(BaseUriWithWildcard listenAddress)
        {
            UriPrefixTable<ITransportManagerRegistration> staticTransportManagerTable = null;
            if (object.ReferenceEquals(base.Scheme, Uri.UriSchemeHttp))
            {
                staticTransportManagerTable = HttpChannelListener.StaticTransportManagerTable;
            }
            else
            {
                staticTransportManagerTable = SharedHttpsTransportManager.StaticTransportManagerTable;
            }
            HostedHttpTransportManager item = null;
            lock (staticTransportManagerTable)
            {
                ITransportManagerRegistration registration;
                if (!staticTransportManagerTable.TryLookupUri(listenAddress.BaseAddress, listenAddress.HostNameComparisonMode, out registration))
                {
                    item = new HostedHttpTransportManager(listenAddress);
                    staticTransportManagerTable.RegisterUri(listenAddress.BaseAddress, listenAddress.HostNameComparisonMode, item);
                }
            }
            return item;
        }

        private void CreateTransportManagers()
        {
            Collection<HostedHttpTransportManager> collection = new Collection<HostedHttpTransportManager>();
            foreach (string str in HostedTransportConfigurationManager.MetabaseSettings.GetBindings(base.Scheme))
            {
                BaseUriWithWildcard listenAddress = null;
                if (object.ReferenceEquals(base.Scheme, Uri.UriSchemeHttp))
                {
                    listenAddress = BaseUriWithWildcard.CreateHttpUri(str);
                }
                else
                {
                    listenAddress = BaseUriWithWildcard.CreateHttpsUri(str);
                }
                bool flag = false;
                if (ServiceHostingEnvironment.MultipleSiteBindingsEnabled)
                {
                    listenAddress = new BaseUriWithWildcard(listenAddress.BaseAddress, HostNameComparisonMode.WeakWildcard);
                    flag = true;
                }
                HostedHttpTransportManager item = this.CreateTransportManager(listenAddress);
                if (item != null)
                {
                    collection.Add(item);
                    base.ListenAddresses.Add(listenAddress);
                }
                if (flag)
                {
                    break;
                }
            }
            this.transportManagerDirectory = collection;
        }

        internal HostedHttpTransportManager GetHttpTransportManager(Uri uri)
        {
            HostedHttpTransportManager manager = null;
            HostedHttpTransportManager manager2 = null;
            if (ServiceHostingEnvironment.MultipleSiteBindingsEnabled)
            {
                return this.transportManagerDirectory[0];
            }
            foreach (HostedHttpTransportManager manager3 in this.transportManagerDirectory)
            {
                if ((string.Compare(manager3.Scheme, uri.Scheme, StringComparison.OrdinalIgnoreCase) == 0) && (manager3.ListenUri.Port == uri.Port))
                {
                    if (manager3.HostNameComparisonMode == HostNameComparisonMode.StrongWildcard)
                    {
                        return manager3;
                    }
                    if (manager3.HostNameComparisonMode == HostNameComparisonMode.WeakWildcard)
                    {
                        manager2 = manager3;
                    }
                    if ((manager3.HostNameComparisonMode == HostNameComparisonMode.Exact) && (string.Compare(manager3.ListenUri.Host, uri.Host, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        manager = manager3;
                    }
                }
            }
            if (manager == null)
            {
                manager = manager2;
            }
            return manager;
        }

        [Conditional("DEBUG")]
        private static void TryDebugPrint(string message)
        {
            if (canDebugPrint)
            {
            }
        }
    }
}

