namespace System.ServiceModel.Dispatcher
{
    using System;

    internal class ListenerChannel
    {
        private IChannelBinder binder;
        private ServiceThrottle throttle;

        public ListenerChannel(IChannelBinder binder)
        {
            this.binder = binder;
        }

        public IChannelBinder Binder =>
            this.binder;

        public ServiceThrottle Throttle
        {
            get => 
                this.throttle;
            set
            {
                this.throttle = value;
            }
        }
    }
}

