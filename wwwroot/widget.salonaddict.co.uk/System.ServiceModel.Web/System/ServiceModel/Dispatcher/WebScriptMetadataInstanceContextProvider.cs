namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class WebScriptMetadataInstanceContextProvider : IInstanceContextProvider
    {
        private InstanceContext instanceContext;

        public WebScriptMetadataInstanceContextProvider(InstanceContext instanceContext)
        {
            this.instanceContext = instanceContext;
        }

        public InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel) => 
            this.instanceContext;

        public void InitializeInstanceContext(InstanceContext instanceContext, Message message, IContextChannel channel)
        {
        }

        public bool IsIdle(InstanceContext instanceContext) => 
            false;

        public void NotifyIdle(InstanceContextIdleCallback callback, InstanceContext instanceContext)
        {
        }
    }
}

