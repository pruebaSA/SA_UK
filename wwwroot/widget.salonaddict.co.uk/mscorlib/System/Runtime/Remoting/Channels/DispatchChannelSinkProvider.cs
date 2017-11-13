namespace System.Runtime.Remoting.Channels
{
    using System;

    internal class DispatchChannelSinkProvider : IServerChannelSinkProvider
    {
        internal DispatchChannelSinkProvider()
        {
        }

        public IServerChannelSink CreateSink(IChannelReceiver channel) => 
            new DispatchChannelSink();

        public void GetChannelData(IChannelDataStore channelData)
        {
        }

        public IServerChannelSinkProvider Next
        {
            get => 
                null;
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

