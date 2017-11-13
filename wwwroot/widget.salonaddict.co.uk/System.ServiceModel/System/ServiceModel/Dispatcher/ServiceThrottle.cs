namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    public sealed class ServiceThrottle
    {
        private FlowThrottle calls;
        internal const int DefaultMaxConcurrentCalls = 0x10;
        internal const int DefaultMaxConcurrentSessions = 10;
        private QuotaThrottle dynamic;
        private ServiceHostBase host;
        private FlowThrottle instanceContexts;
        private bool isActive;
        private const string MaxConcurrentCallsConfigName = "maxConcurrentCalls";
        private const string MaxConcurrentCallsPropertyName = "MaxConcurrentCalls";
        private const string MaxConcurrentInstancesConfigName = "maxConcurrentInstances";
        private const string MaxConcurrentInstancesPropertyName = "MaxConcurrentInstances";
        private const string MaxConcurrentSessionsConfigName = "maxConcurrentSessions";
        private const string MaxConcurrentSessionsPropertyName = "MaxConcurrentSessions";
        private FlowThrottle sessions;
        private object thisLock = new object();

        internal ServiceThrottle(ServiceHostBase host)
        {
            if (host == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("host");
            }
            this.host = host;
            this.MaxConcurrentCalls = 0x10;
            this.MaxConcurrentSessions = 10;
            this.isActive = true;
        }

        internal bool AcquireCall(ChannelHandler channel)
        {
            lock (this.ThisLock)
            {
                return this.PrivateAcquireCall(channel);
            }
        }

        internal bool AcquireInstanceContextAndDynamic(ChannelHandler channel, bool acquireInstanceContextThrottle)
        {
            lock (this.ThisLock)
            {
                if (!acquireInstanceContextThrottle)
                {
                    return this.PrivateAcquireDynamic(channel);
                }
                return (this.PrivateAcquireInstanceContext(channel) && this.PrivateAcquireDynamic(channel));
            }
        }

        internal bool AcquireSession(ISessionThrottleNotification source)
        {
            lock (this.ThisLock)
            {
                return this.PrivateAcquireSession(source);
            }
        }

        internal bool AcquireSession(ListenerHandler listener)
        {
            lock (this.ThisLock)
            {
                return this.PrivateAcquireSessionListenerHandler(listener);
            }
        }

        internal void DeactivateCall()
        {
            if (this.isActive && (this.calls != null))
            {
                this.calls.Release();
            }
        }

        internal void DeactivateChannel()
        {
            if (this.isActive && (this.sessions != null))
            {
                this.sessions.Release();
            }
        }

        internal void DeactivateInstanceContext()
        {
            if (this.isActive && (this.instanceContexts != null))
            {
                this.instanceContexts.Release();
            }
        }

        private void GotCall(object state)
        {
            ChannelHandler handler = (ChannelHandler) state;
            lock (this.ThisLock)
            {
                handler.ThrottleAcquiredForCall();
            }
        }

        private void GotDynamic(object state)
        {
            ((ChannelHandler) state).ThrottleAcquired();
        }

        private void GotInstanceContext(object state)
        {
            ChannelHandler channel = (ChannelHandler) state;
            lock (this.ThisLock)
            {
                if (this.PrivateAcquireDynamic(channel))
                {
                    channel.ThrottleAcquired();
                }
            }
        }

        private void GotSession(object state)
        {
            ((ISessionThrottleNotification) state).ThrottleAcquired();
        }

        internal int IncrementManualFlowControlLimit(int incrementBy) => 
            this.Dynamic.IncrementLimit(incrementBy);

        private bool PrivateAcquireCall(ChannelHandler channel)
        {
            if (this.calls != null)
            {
                return this.calls.Acquire(channel);
            }
            return true;
        }

        private bool PrivateAcquireDynamic(ChannelHandler channel)
        {
            if (this.dynamic != null)
            {
                return this.dynamic.Acquire(channel);
            }
            return true;
        }

        private bool PrivateAcquireInstanceContext(ChannelHandler channel)
        {
            if ((this.instanceContexts != null) && (channel.InstanceContext == null))
            {
                channel.InstanceContextServiceThrottle = this;
                return this.instanceContexts.Acquire(channel);
            }
            return true;
        }

        private bool PrivateAcquireSession(ISessionThrottleNotification source)
        {
            if (this.sessions != null)
            {
                return this.sessions.Acquire(source);
            }
            return true;
        }

        private bool PrivateAcquireSessionListenerHandler(ListenerHandler listener)
        {
            if (((this.sessions != null) && (listener.Channel != null)) && (listener.Channel.Throttle == null))
            {
                listener.Channel.Throttle = this;
                return this.sessions.Acquire(listener);
            }
            return true;
        }

        private void ThrowIfClosedOrOpened(string memberName)
        {
            if (this.host.State == CommunicationState.Opened)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxImmutableThrottle1", new object[] { memberName })));
            }
            this.host.ThrowIfClosedOrOpened();
        }

        private void UpdateIsActive()
        {
            this.isActive = (((this.dynamic != null) || ((this.calls != null) && (this.calls.Capacity != 0x7fffffff))) || ((this.sessions != null) && (this.sessions.Capacity != 0x7fffffff))) || ((this.instanceContexts != null) && (this.instanceContexts.Capacity != 0x7fffffff));
        }

        private FlowThrottle Calls
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (this.calls == null)
                    {
                        this.calls = new FlowThrottle(new WaitCallback(this.GotCall), 0x10, "MaxConcurrentCalls", "maxConcurrentCalls");
                    }
                    return this.calls;
                }
            }
        }

        private QuotaThrottle Dynamic
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (this.dynamic == null)
                    {
                        this.dynamic = new QuotaThrottle(new WaitCallback(this.GotDynamic), new object());
                        this.dynamic.Owner = "ServiceHost";
                    }
                    this.UpdateIsActive();
                    return this.dynamic;
                }
            }
        }

        private FlowThrottle InstanceContexts
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (this.instanceContexts == null)
                    {
                        this.instanceContexts = new FlowThrottle(new WaitCallback(this.GotInstanceContext), 0x7fffffff, "MaxConcurrentInstances", "maxConcurrentInstances");
                    }
                    return this.instanceContexts;
                }
            }
        }

        internal bool IsActive =>
            this.isActive;

        internal int ManualFlowControlLimit
        {
            get => 
                this.Dynamic.Limit;
            set
            {
                this.Dynamic.SetLimit(value);
            }
        }

        public int MaxConcurrentCalls
        {
            get => 
                this.Calls.Capacity;
            set
            {
                this.ThrowIfClosedOrOpened("MaxConcurrentCalls");
                this.Calls.Capacity = value;
                this.UpdateIsActive();
            }
        }

        public int MaxConcurrentInstances
        {
            get => 
                this.InstanceContexts.Capacity;
            set
            {
                this.ThrowIfClosedOrOpened("MaxConcurrentInstances");
                this.InstanceContexts.Capacity = value;
                this.UpdateIsActive();
            }
        }

        public int MaxConcurrentSessions
        {
            get => 
                this.Sessions.Capacity;
            set
            {
                this.ThrowIfClosedOrOpened("MaxConcurrentSessions");
                this.Sessions.Capacity = value;
                this.UpdateIsActive();
            }
        }

        private FlowThrottle Sessions
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (this.sessions == null)
                    {
                        this.sessions = new FlowThrottle(new WaitCallback(this.GotSession), 10, "MaxConcurrentSessions", "maxConcurrentSessions");
                    }
                    return this.sessions;
                }
            }
        }

        internal object ThisLock =>
            this.thisLock;
    }
}

