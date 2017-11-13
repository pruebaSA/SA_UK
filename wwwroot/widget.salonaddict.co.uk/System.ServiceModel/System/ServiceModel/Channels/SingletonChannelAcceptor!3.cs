namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    internal abstract class SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType> : InputQueueChannelAcceptor<ChannelInterfaceType> where ChannelInterfaceType: class, IChannel where TChannel: InputQueueChannel<QueueItemType> where QueueItemType: class, IDisposable
    {
        private TChannel currentChannel;
        private object currentChannelLock;
        private static WaitCallback onInvokeDequeuedCallback;

        public SingletonChannelAcceptor(ChannelManagerBase channelManager) : base(channelManager)
        {
            this.currentChannelLock = new object();
        }

        public override ChannelInterfaceType AcceptChannel(TimeSpan timeout)
        {
            this.EnsureChannelAvailable();
            return base.AcceptChannel(timeout);
        }

        public override IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.EnsureChannelAvailable();
            return base.BeginAcceptChannel(timeout, callback, state);
        }

        public void DispatchItems()
        {
            TChannel local = this.EnsureChannelAvailable();
            if (local != null)
            {
                local.Dispatch();
            }
        }

        public void Enqueue(QueueItemType item)
        {
            this.Enqueue(item, null);
        }

        public void Enqueue(QueueItemType item, ItemDequeuedCallback dequeuedCallback)
        {
            this.Enqueue(item, dequeuedCallback, true);
        }

        public void Enqueue(Exception exception, ItemDequeuedCallback dequeuedCallback)
        {
            this.Enqueue(exception, dequeuedCallback, true);
        }

        public void Enqueue(QueueItemType item, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                this.OnTraceMessageReceived(item);
            }
            if (local != null)
            {
                local.EnqueueAndDispatch(item, dequeuedCallback, canDispatchOnThisThread);
            }
            else
            {
                SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, canDispatchOnThisThread);
                item.Dispose();
            }
        }

        public void Enqueue(Exception exception, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (local != null)
            {
                local.EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
            }
            else
            {
                SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, canDispatchOnThisThread);
            }
        }

        public void EnqueueAndDispatch(QueueItemType item, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                this.OnTraceMessageReceived(item);
            }
            if (local != null)
            {
                local.EnqueueAndDispatch(item, dequeuedCallback, canDispatchOnThisThread);
            }
            else
            {
                SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, canDispatchOnThisThread);
                item.Dispose();
            }
        }

        public override void EnqueueAndDispatch(Exception exception, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (local != null)
            {
                local.EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
            }
            else
            {
                SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, canDispatchOnThisThread);
            }
        }

        public bool EnqueueWithoutDispatch(QueueItemType item, ItemDequeuedCallback dequeuedCallback)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                this.OnTraceMessageReceived(item);
            }
            if (local != null)
            {
                return local.EnqueueWithoutDispatch(item, dequeuedCallback);
            }
            SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, false);
            item.Dispose();
            return false;
        }

        public override bool EnqueueWithoutDispatch(Exception exception, ItemDequeuedCallback dequeuedCallback)
        {
            TChannel local = this.EnsureChannelAvailable();
            if (local != null)
            {
                return local.EnqueueWithoutDispatch(exception, dequeuedCallback);
            }
            SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.InvokeDequeuedCallback(dequeuedCallback, false);
            return false;
        }

        private TChannel EnsureChannelAvailable()
        {
            bool flag = false;
            TChannel currentChannel = this.currentChannel;
            if (currentChannel == null)
            {
                lock (this.currentChannelLock)
                {
                    if (base.IsDisposed)
                    {
                        return default(TChannel);
                    }
                    currentChannel = this.currentChannel;
                    if (currentChannel == null)
                    {
                        currentChannel = this.OnCreateChannel();
                        currentChannel.Closed += new EventHandler(this.OnChannelClosed);
                        this.currentChannel = currentChannel;
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                base.EnqueueAndDispatch((ChannelInterfaceType) currentChannel);
            }
            return currentChannel;
        }

        protected TChannel GetCurrentChannel() => 
            this.currentChannel;

        private static void InvokeDequeuedCallback(ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            if (dequeuedCallback != null)
            {
                if (canDispatchOnThisThread)
                {
                    dequeuedCallback();
                }
                else
                {
                    if (SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.onInvokeDequeuedCallback == null)
                    {
                        SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.onInvokeDequeuedCallback = new WaitCallback(SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.OnInvokeDequeuedCallback);
                    }
                    IOThreadScheduler.ScheduleCallback(SingletonChannelAcceptor<ChannelInterfaceType, TChannel, QueueItemType>.onInvokeDequeuedCallback, dequeuedCallback);
                }
            }
        }

        protected void OnChannelClosed(object sender, EventArgs args)
        {
            IChannel channel = (IChannel) sender;
            lock (this.currentChannelLock)
            {
                if (channel == this.currentChannel)
                {
                    this.currentChannel = default(TChannel);
                }
            }
        }

        protected abstract TChannel OnCreateChannel();
        private static void OnInvokeDequeuedCallback(object state)
        {
            ItemDequeuedCallback callback = (ItemDequeuedCallback) state;
            callback();
        }

        protected abstract void OnTraceMessageReceived(QueueItemType item);
    }
}

