namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;

    internal class ServerSingletonConnectionReader : SingletonConnectionReader
    {
        private ChannelBinding channelBindingToken;
        private ConnectionDemuxer connectionDemuxer;
        private string contentType;
        private ServerSingletonDecoder decoder;
        private IConnection rawConnection;

        public ServerSingletonConnectionReader(ServerSingletonPreambleConnectionReader preambleReader, IConnection upgradedConnection, ConnectionDemuxer connectionDemuxer) : base(upgradedConnection, preambleReader.BufferOffset, preambleReader.BufferSize, preambleReader.Security, preambleReader.TransportSettings, preambleReader.Via)
        {
            this.decoder = preambleReader.Decoder;
            this.contentType = this.decoder.ContentType;
            this.connectionDemuxer = connectionDemuxer;
            this.rawConnection = preambleReader.RawConnection;
            this.channelBindingToken = preambleReader.ChannelBinding;
        }

        protected override bool DecodeBytes(byte[] buffer, ref int offset, ref int size, ref bool isAtEof)
        {
            while (size > 0)
            {
                int num = this.decoder.Decode(buffer, offset, size);
                if (num > 0)
                {
                    offset += num;
                    size -= num;
                }
                ServerSingletonDecoder.State currentState = this.decoder.CurrentState;
                if (currentState != ServerSingletonDecoder.State.EnvelopeStart)
                {
                    if (currentState == ServerSingletonDecoder.State.End)
                    {
                        goto Label_003D;
                    }
                    continue;
                }
                return true;
            Label_003D:
                isAtEof = true;
                return false;
            }
            return false;
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.Connection.Write(SingletonEncoder.EndBytes, 0, SingletonEncoder.EndBytes.Length, true, helper.RemainingTime());
            this.connectionDemuxer.ReuseConnection(this.rawConnection, helper.RemainingTime());
            if (this.channelBindingToken != null)
            {
                this.channelBindingToken.Dispose();
            }
        }

        protected override void PrepareMessage(Message message)
        {
            base.PrepareMessage(message);
            IPEndPoint remoteIPEndPoint = this.rawConnection.RemoteIPEndPoint;
            if (remoteIPEndPoint != null)
            {
                RemoteEndpointMessageProperty property = new RemoteEndpointMessageProperty(remoteIPEndPoint);
                message.Properties.Add(RemoteEndpointMessageProperty.Name, property);
            }
            ChannelBindingUtility.TryAddToMessage(this.channelBindingToken, message, false);
        }

        protected override string ContentType =>
            this.contentType;

        protected override long StreamPosition =>
            this.decoder.StreamPosition;
    }
}

