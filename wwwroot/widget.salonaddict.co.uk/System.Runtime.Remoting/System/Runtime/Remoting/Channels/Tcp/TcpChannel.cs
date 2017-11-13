﻿namespace System.Runtime.Remoting.Channels.Tcp
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;

    public class TcpChannel : IChannelReceiver, IChannelSender, IChannel, ISecurableChannel
    {
        private string _channelName;
        private int _channelPriority;
        private TcpClientChannel _clientChannel;
        private TcpServerChannel _serverChannel;

        public TcpChannel()
        {
            this._channelPriority = 1;
            this._channelName = "tcp";
            this._clientChannel = new TcpClientChannel();
        }

        public TcpChannel(int port) : this()
        {
            this._serverChannel = new TcpServerChannel(port);
        }

        public TcpChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
        {
            this._channelPriority = 1;
            this._channelName = "tcp";
            Hashtable hashtable = new Hashtable();
            Hashtable hashtable2 = new Hashtable();
            bool flag = false;
            if (properties != null)
            {
                foreach (DictionaryEntry entry in properties)
                {
                    switch (((string) entry.Key))
                    {
                        case "name":
                            this._channelName = (string) entry.Value;
                            break;

                        case "priority":
                            this._channelPriority = Convert.ToInt32((string) entry.Value, CultureInfo.InvariantCulture);
                            break;

                        case "port":
                            hashtable2["port"] = entry.Value;
                            flag = true;
                            break;

                        default:
                            hashtable[entry.Key] = entry.Value;
                            hashtable2[entry.Key] = entry.Value;
                            break;
                    }
                }
            }
            this._clientChannel = new TcpClientChannel(hashtable, clientSinkProvider);
            if (flag)
            {
                this._serverChannel = new TcpServerChannel(hashtable2, serverSinkProvider);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI) => 
            this._clientChannel.CreateMessageSink(url, remoteChannelData, out objectURI);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public string[] GetUrlsForUri(string objectURI)
        {
            if (this._serverChannel != null)
            {
                return this._serverChannel.GetUrlsForUri(objectURI);
            }
            return null;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public string Parse(string url, out string objectURI) => 
            TcpChannelHelper.ParseURL(url, out objectURI);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public void StartListening(object data)
        {
            if (this._serverChannel != null)
            {
                this._serverChannel.StartListening(data);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public void StopListening(object data)
        {
            if (this._serverChannel != null)
            {
                this._serverChannel.StopListening(data);
            }
        }

        public object ChannelData
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
            get
            {
                if (this._serverChannel != null)
                {
                    return this._serverChannel.ChannelData;
                }
                return null;
            }
        }

        public string ChannelName =>
            this._channelName;

        public int ChannelPriority =>
            this._channelPriority;

        public bool IsSecured
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
            get
            {
                if (this._clientChannel != null)
                {
                    return this._clientChannel.IsSecured;
                }
                return ((this._serverChannel != null) && this._serverChannel.IsSecured);
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
            set
            {
                if (ChannelServices.RegisteredChannels.Contains(this))
                {
                    throw new InvalidOperationException(CoreChannel.GetResourceString("Remoting_InvalidOperation_IsSecuredCannotBeChangedOnRegisteredChannels"));
                }
                if (this._clientChannel != null)
                {
                    this._clientChannel.IsSecured = value;
                }
                if (this._serverChannel != null)
                {
                    this._serverChannel.IsSecured = value;
                }
            }
        }
    }
}

