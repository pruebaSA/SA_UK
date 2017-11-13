namespace System.Runtime.Remoting.Channels
{
    using System;

    internal class RegisteredChannelList
    {
        private RegisteredChannel[] _channels;

        internal RegisteredChannelList()
        {
            this._channels = new RegisteredChannel[0];
        }

        internal RegisteredChannelList(RegisteredChannel[] channels)
        {
            this._channels = channels;
        }

        internal int FindChannelIndex(IChannel channel)
        {
            object obj2 = channel;
            for (int i = 0; i < this._channels.Length; i++)
            {
                if (obj2 == this.GetChannel(i))
                {
                    return i;
                }
            }
            return -1;
        }

        internal int FindChannelIndex(string name)
        {
            for (int i = 0; i < this._channels.Length; i++)
            {
                if (string.Compare(name, this.GetChannel(i).ChannelName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal IChannel GetChannel(int index) => 
            this._channels[index].Channel;

        internal bool IsReceiver(int index) => 
            this._channels[index].IsReceiver();

        internal bool IsSender(int index) => 
            this._channels[index].IsSender();

        internal int Count =>
            this._channels?.Length;

        internal int ReceiverCount
        {
            get
            {
                if (this._channels == null)
                {
                    return 0;
                }
                int num = 0;
                for (int i = 0; i < this._channels.Length; i++)
                {
                    if (this.IsReceiver(i))
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        internal RegisteredChannel[] RegisteredChannels =>
            this._channels;
    }
}

