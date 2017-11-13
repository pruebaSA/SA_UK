namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal class TransportReplyChannelAcceptor : ReplyChannelAcceptor
    {
        private TransportChannelListener listener;
        private TransportManagerContainer transportManagerContainer;

        public TransportReplyChannelAcceptor(TransportChannelListener listener) : base(listener)
        {
            this.listener = listener;
        }

        private IAsyncResult DummyBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new CompletedAsyncResult(callback, state);

        private void DummyEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            if ((this.transportManagerContainer != null) && !this.TransferTransportManagers())
            {
                this.transportManagerContainer.Close();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ChainedBeginHandler handler = new ChainedBeginHandler(this.DummyBeginClose);
            ChainedEndHandler handler2 = new ChainedEndHandler(this.DummyEndClose);
            if ((this.transportManagerContainer != null) && !this.TransferTransportManagers())
            {
                handler = new ChainedBeginHandler(this.transportManagerContainer.BeginClose);
                handler2 = new ChainedEndHandler(this.transportManagerContainer.EndClose);
            }
            return new ChainedAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), handler, handler2);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            base.OnClose(new TimeoutHelper(timeout).RemainingTime());
            if ((this.transportManagerContainer != null) && !this.TransferTransportManagers())
            {
                this.transportManagerContainer.Close();
            }
        }

        protected override ReplyChannel OnCreateChannel() => 
            new TransportReplyChannel(base.ChannelManager, null);

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            this.transportManagerContainer = this.listener.GetTransportManagers();
            this.listener = null;
        }

        private bool TransferTransportManagers()
        {
            TransportReplyChannel currentChannel = (TransportReplyChannel) base.GetCurrentChannel();
            return currentChannel?.TransferTransportManagers(this.transportManagerContainer);
        }

        protected class TransportReplyChannel : ReplyChannel
        {
            private TransportManagerContainer transportManagerContainer;

            public TransportReplyChannel(ChannelManagerBase channelManager, EndpointAddress localAddress) : base(channelManager, localAddress)
            {
            }

            private IAsyncResult DummyBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
                new CompletedAsyncResult(callback, state);

            private void DummyEndClose(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }

            protected override void OnAbort()
            {
                if (this.transportManagerContainer != null)
                {
                    this.transportManagerContainer.Close();
                }
                base.OnAbort();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                ChainedBeginHandler handler = new ChainedBeginHandler(this.DummyBeginClose);
                ChainedEndHandler handler2 = new ChainedEndHandler(this.DummyEndClose);
                if (this.transportManagerContainer != null)
                {
                    handler = new ChainedBeginHandler(this.transportManagerContainer.BeginClose);
                    handler2 = new ChainedEndHandler(this.transportManagerContainer.EndClose);
                }
                return new ChainedAsyncResult(timeout, callback, state, handler, handler2, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose));
            }

            protected override void OnClose(TimeSpan timeout)
            {
                if (this.transportManagerContainer != null)
                {
                    this.transportManagerContainer.Close();
                }
                base.OnClose(timeout);
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                ChainedAsyncResult.End(result);
            }

            public bool TransferTransportManagers(TransportManagerContainer transportManagerContainer)
            {
                lock (base.ThisLock)
                {
                    if (base.State != CommunicationState.Opened)
                    {
                        return false;
                    }
                    this.transportManagerContainer = transportManagerContainer;
                    return true;
                }
            }
        }
    }
}

