﻿namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;

    internal class DispatchChannelSink : IServerChannelSink, IChannelSinkBase
    {
        internal DispatchChannelSink()
        {
        }

        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
        {
            throw new NotSupportedException();
        }

        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
        {
            throw new NotSupportedException();
        }

        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            if (requestMsg == null)
            {
                throw new ArgumentNullException("requestMsg", Environment.GetResourceString("Remoting_Channel_DispatchSinkMessageMissing"));
            }
            if (requestStream != null)
            {
                throw new RemotingException(Environment.GetResourceString("Remoting_Channel_DispatchSinkWantsNullRequestStream"));
            }
            responseHeaders = null;
            responseStream = null;
            return ChannelServices.DispatchMessage(sinkStack, requestMsg, out responseMsg);
        }

        public IServerChannelSink NextChannelSink =>
            null;

        public IDictionary Properties =>
            null;
    }
}

