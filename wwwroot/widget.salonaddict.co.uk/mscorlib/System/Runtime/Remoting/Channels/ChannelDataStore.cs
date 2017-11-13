namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ChannelDataStore : IChannelDataStore
    {
        private string[] _channelURIs;
        private DictionaryEntry[] _extraData;

        public ChannelDataStore(string[] channelURIs)
        {
            this._channelURIs = channelURIs;
            this._extraData = null;
        }

        private ChannelDataStore(string[] channelUrls, DictionaryEntry[] extraData)
        {
            this._channelURIs = channelUrls;
            this._extraData = extraData;
        }

        internal ChannelDataStore InternalShallowCopy() => 
            new ChannelDataStore(this._channelURIs, this._extraData);

        public string[] ChannelUris
        {
            get => 
                this._channelURIs;
            set
            {
                this._channelURIs = value;
            }
        }

        public object this[object key]
        {
            get
            {
                foreach (DictionaryEntry entry in this._extraData)
                {
                    if (entry.Key.Equals(key))
                    {
                        return entry.Value;
                    }
                }
                return null;
            }
            set
            {
                if (this._extraData == null)
                {
                    this._extraData = new DictionaryEntry[] { new DictionaryEntry(key, value) };
                }
                else
                {
                    int length = this._extraData.Length;
                    DictionaryEntry[] entryArray = new DictionaryEntry[length + 1];
                    int index = 0;
                    while (index < length)
                    {
                        entryArray[index] = this._extraData[index];
                        index++;
                    }
                    entryArray[index] = new DictionaryEntry(key, value);
                    this._extraData = entryArray;
                }
            }
        }
    }
}

