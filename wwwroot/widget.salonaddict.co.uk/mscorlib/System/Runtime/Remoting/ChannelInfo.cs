namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.Remoting.Channels;

    [Serializable]
    internal sealed class ChannelInfo : IChannelInfo
    {
        private object[] channelData;

        internal ChannelInfo()
        {
            this.ChannelData = ChannelServices.CurrentChannelData;
        }

        public object[] ChannelData
        {
            get => 
                this.channelData;
            set
            {
                this.channelData = value;
            }
        }
    }
}

