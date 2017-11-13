namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.Threading;

    internal sealed class InternalDuplexChannelFactory : LayeredChannelFactory<IDuplexChannel>
    {
        private static long channelCount;
        private InputChannelDemuxer channelDemuxer;
        private IChannelFactory<IOutputChannel> innerChannelFactory;
        private IChannelListener<IInputChannel> innerChannelListener;
        private LocalAddressProvider localAddressProvider;
        private bool providesCorrelation;

        internal InternalDuplexChannelFactory(InternalDuplexBindingElement bindingElement, BindingContext context, InputChannelDemuxer channelDemuxer, IChannelFactory<IOutputChannel> innerChannelFactory, LocalAddressProvider localAddressProvider) : base(context.Binding, innerChannelFactory)
        {
            this.channelDemuxer = channelDemuxer;
            this.innerChannelFactory = innerChannelFactory;
            ChannelDemuxerFilter filter = new ChannelDemuxerFilter(new MatchNoneMessageFilter(), -2147483648);
            this.innerChannelListener = this.channelDemuxer.BuildChannelListener<IInputChannel>(filter);
            this.localAddressProvider = localAddressProvider;
            this.providesCorrelation = bindingElement.ProvidesCorrelation;
        }

        public IDuplexChannel CreateChannel(EndpointAddress address, Uri via, MessageFilter filter, int priority) => 
            this.CreateChannel(address, via, new EndpointAddress(this.innerChannelListener.Uri, new AddressHeader[0]), filter, priority);

        public IDuplexChannel CreateChannel(EndpointAddress remoteAddress, Uri via, EndpointAddress localAddress, MessageFilter filter, int priority)
        {
            ChannelDemuxerFilter filter2 = new ChannelDemuxerFilter(new AndMessageFilter(new EndpointAddressMessageFilter(localAddress, true), filter), priority);
            IDuplexChannel channel = null;
            IOutputChannel innerOutputChannel = null;
            IChannelListener<IInputChannel> innerInputListener = null;
            IInputChannel innerInputChannel = null;
            try
            {
                innerOutputChannel = this.innerChannelFactory.CreateChannel(remoteAddress, via);
                innerInputListener = this.channelDemuxer.BuildChannelListener<IInputChannel>(filter2);
                innerInputListener.Open();
                innerInputChannel = innerInputListener.AcceptChannel();
                channel = new ClientCompositeDuplexChannel(this, innerInputChannel, innerInputListener, localAddress, innerOutputChannel);
            }
            finally
            {
                if (channel == null)
                {
                    if (innerOutputChannel != null)
                    {
                        innerOutputChannel.Close();
                    }
                    if (innerInputListener != null)
                    {
                        innerInputListener.Close();
                    }
                    if (innerInputChannel != null)
                    {
                        innerInputChannel.Close();
                    }
                }
            }
            return channel;
        }

        private void CreateUniqueLocalAddress(out EndpointAddress address, out int priority)
        {
            long num = Interlocked.Increment(ref channelCount);
            if (num > 1L)
            {
                AddressHeader header = AddressHeader.CreateAddressHeader(XD.UtilityDictionary.UniqueEndpointHeaderName, XD.UtilityDictionary.UniqueEndpointHeaderNamespace, num);
                address = new EndpointAddress(this.innerChannelListener.Uri, new AddressHeader[] { header });
                priority = 1;
            }
            else
            {
                address = new EndpointAddress(this.innerChannelListener.Uri, new AddressHeader[0]);
                priority = 0;
            }
        }

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(IChannelListener))
            {
                return (T) this.innerChannelListener;
            }
            if ((typeof(T) == typeof(ISecurityCapabilities)) && !this.providesCorrelation)
            {
                return InternalDuplexBindingElement.GetSecurityCapabilities<T>(base.GetProperty<ISecurityCapabilities>());
            }
            T property = base.GetProperty<T>();
            if (property != null)
            {
                return property;
            }
            IChannelListener innerChannelListener = this.innerChannelListener;
            if (innerChannelListener != null)
            {
                return innerChannelListener.GetProperty<T>();
            }
            return default(T);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.innerChannelListener.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new ChainedCloseAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), new ICommunicationObject[] { this.innerChannelListener });

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new ChainedOpenAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginOpen), new ChainedEndHandler(this.OnEndOpen), new ICommunicationObject[] { this.innerChannelListener });

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnClose(helper.RemainingTime());
            this.innerChannelListener.Close(helper.RemainingTime());
        }

        protected override IDuplexChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            EndpointAddress localAddress;
            int priority;
            MessageFilter filter;
            if (this.localAddressProvider != null)
            {
                localAddress = this.localAddressProvider.LocalAddress;
                filter = this.localAddressProvider.Filter;
                priority = this.localAddressProvider.Priority;
            }
            else
            {
                this.CreateUniqueLocalAddress(out localAddress, out priority);
                filter = new MatchAllMessageFilter();
            }
            return this.CreateChannel(address, via, localAddress, filter, priority);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnOpen(helper.RemainingTime());
            this.innerChannelListener.Open(helper.RemainingTime());
        }

        private class ClientCompositeDuplexChannel : LayeredDuplexChannel
        {
            private IChannelListener<IInputChannel> innerInputListener;

            public ClientCompositeDuplexChannel(ChannelManagerBase channelManager, IInputChannel innerInputChannel, IChannelListener<IInputChannel> innerInputListener, EndpointAddress localAddress, IOutputChannel innerOutputChannel) : base(channelManager, innerInputChannel, localAddress, innerOutputChannel)
            {
                this.innerInputListener = innerInputListener;
            }

            protected override void OnAbort()
            {
                base.OnAbort();
                this.innerInputListener.Abort();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
                new ChainedAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), new ChainedBeginHandler(this.innerInputListener.BeginClose), new ChainedEndHandler(this.innerInputListener.EndClose));

            protected override void OnClose(TimeSpan timeout)
            {
                TimeoutHelper helper = new TimeoutHelper(timeout);
                base.OnClose(helper.RemainingTime());
                this.innerInputListener.Close(helper.RemainingTime());
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                ChainedAsyncResult.End(result);
            }
        }
    }
}

