namespace System.ServiceModel.Channels
{
    using System;

    public abstract class BindingElement
    {
        protected BindingElement()
        {
        }

        protected BindingElement(BindingElement elementToBeCloned)
        {
        }

        public virtual IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) => 
            context?.BuildInnerChannelFactory<TChannel>();

        public virtual IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            context?.BuildInnerChannelListener<TChannel>();

        public virtual bool CanBuildChannelFactory<TChannel>(BindingContext context) => 
            context?.CanBuildInnerChannelFactory<TChannel>();

        public virtual bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            context?.CanBuildInnerChannelListener<TChannel>();

        public abstract BindingElement Clone();
        internal T GetIndividualProperty<T>() where T: class => 
            this.GetProperty<T>(new BindingContext(new CustomBinding(), new BindingParameterCollection()));

        public abstract T GetProperty<T>(BindingContext context) where T: class;
        internal virtual bool IsMatch(BindingElement b) => 
            false;
    }
}

