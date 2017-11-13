namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class PerCallInstanceContextProvider : InstanceContextProviderBase
    {
        internal PerCallInstanceContextProvider(DispatchRuntime dispatchRuntime) : base(dispatchRuntime)
        {
        }

        public override InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel) => 
            null;

        public override void InitializeInstanceContext(InstanceContext instanceContext, Message message, IContextChannel channel)
        {
        }

        public override bool IsIdle(InstanceContext instanceContext) => 
            true;

        public override void NotifyIdle(InstanceContextIdleCallback callback, InstanceContext instanceContext)
        {
        }
    }
}

