﻿namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels.Http;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    using System.Security.Permissions;

    public class SoapServerFormatterSink : IServerChannelSink, IChannelSinkBase
    {
        private System.Runtime.Serialization.Formatters.TypeFilterLevel _formatterSecurityLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
        private bool _includeVersioning = true;
        private IServerChannelSink _nextSink;
        private Protocol _protocol;
        private IChannelReceiver _receiver;
        private bool _strictBinding;

        public SoapServerFormatterSink(Protocol protocol, IServerChannelSink nextSink, IChannelReceiver receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException("receiver");
            }
            this._nextSink = nextSink;
            this._protocol = protocol;
            this._receiver = receiver;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
        {
            this.SerializeResponse(sinkStack, msg, true, ref headers, out stream);
            sinkStack.AsyncProcessResponse(msg, headers, stream);
        }

        private Header[] GetChannelHeaders(ITransportHeaders requestHeaders, out string soapActionToVerify)
        {
            string str3;
            string str4;
            soapActionToVerify = null;
            string uRI = (string) requestHeaders["__RequestUri"];
            string uri = (string) requestHeaders["SOAPAction"];
            if (uri == null)
            {
                throw new RemotingException(CoreChannel.GetResourceString("Remoting_SoapActionMissing"));
            }
            uri = HttpEncodingHelper.DecodeUri(uri);
            soapActionToVerify = uri;
            if (!SoapServices.GetTypeAndMethodNameFromSoapAction(uri, out str3, out str4))
            {
                Type serverTypeForUri = RemotingServices.GetServerTypeForUri(uRI);
                if (serverTypeForUri == null)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, CoreChannel.GetResourceString("Remoting_TypeNotFoundFromUri"), new object[] { uRI }));
                }
                str3 = "clr:" + serverTypeForUri.FullName + ", " + serverTypeForUri.Assembly.GetName().Name;
            }
            else
            {
                str3 = "clr:" + str3;
            }
            int num = 2;
            Header[] headerArray = new Header[num];
            headerArray[0] = new Header("__Uri", uRI);
            headerArray[1] = new Header("__TypeName", str3);
            return headerArray;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
        {
            throw new NotSupportedException();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            ServerProcessing complete;
            if (requestMsg != null)
            {
                return this._nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
            }
            if (requestHeaders == null)
            {
                throw new ArgumentNullException("requestHeaders");
            }
            BaseTransportHeaders headers = requestHeaders as BaseTransportHeaders;
            responseHeaders = null;
            responseStream = null;
            string str = null;
            string str2 = null;
            bool flag = true;
            string contentType = null;
            if (headers != null)
            {
                contentType = headers.ContentType;
            }
            else
            {
                contentType = requestHeaders["Content-Type"] as string;
            }
            if (contentType != null)
            {
                string str4;
                HttpChannelHelper.ParseContentType(contentType, out str2, out str4);
            }
            if ((str2 != null) && (string.Compare(str2, "text/xml", StringComparison.Ordinal) != 0))
            {
                flag = false;
            }
            if (this._protocol == Protocol.Http)
            {
                str = (string) requestHeaders["__RequestVerb"];
                if (!str.Equals("POST") && !str.Equals("M-POST"))
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                if (this._nextSink != null)
                {
                    return this._nextSink.ProcessMessage(sinkStack, null, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
                }
                if (this._protocol != Protocol.Http)
                {
                    throw new RemotingException(CoreChannel.GetResourceString("Remoting_Channels_InvalidRequestFormat"));
                }
                responseHeaders = new TransportHeaders();
                responseHeaders["__HttpStatusCode"] = "400";
                responseHeaders["__HttpReasonPhrase"] = "Bad Request";
                responseStream = null;
                responseMsg = null;
                return ServerProcessing.Complete;
            }
            bool bClientIsClr = true;
            try
            {
                string str7;
                string uRI = null;
                if (headers != null)
                {
                    uRI = headers.RequestUri;
                }
                else
                {
                    uRI = (string) requestHeaders["__RequestUri"];
                }
                if (RemotingServices.GetServerTypeForUri(uRI) == null)
                {
                    throw new RemotingException(CoreChannel.GetResourceString("Remoting_ChnlSink_UriNotPublished"));
                }
                if (this._protocol == Protocol.Http)
                {
                    string str6 = (string) requestHeaders["User-Agent"];
                    if (str6 != null)
                    {
                        if (str6.IndexOf("MS .NET Remoting") == -1)
                        {
                            bClientIsClr = false;
                        }
                    }
                    else
                    {
                        bClientIsClr = false;
                    }
                }
                bool data = true;
                object obj2 = requestHeaders["__CustomErrorsEnabled"];
                if ((obj2 != null) && (obj2 is bool))
                {
                    data = (bool) obj2;
                }
                CallContext.SetData("__CustomErrorsEnabled", data);
                Header[] channelHeaders = this.GetChannelHeaders(requestHeaders, out str7);
                PermissionSet set = null;
                if (this.TypeFilterLevel != System.Runtime.Serialization.Formatters.TypeFilterLevel.Full)
                {
                    set = new PermissionSet(PermissionState.None);
                    set.SetPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
                }
                try
                {
                    if (set != null)
                    {
                        set.PermitOnly();
                    }
                    requestMsg = CoreChannel.DeserializeSoapRequestMessage(requestStream, channelHeaders, this._strictBinding, this.TypeFilterLevel);
                }
                finally
                {
                    if (set != null)
                    {
                        CodeAccessPermission.RevertPermitOnly();
                    }
                }
                requestStream.Close();
                if (requestMsg == null)
                {
                    throw new RemotingException(CoreChannel.GetResourceString("Remoting_DeserializeMessage"));
                }
                if ((str7 != null) && !SoapServices.IsSoapActionValidForMethodBase(str7, ((IMethodMessage) requestMsg).MethodBase))
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, CoreChannel.GetResourceString("Remoting_Soap_InvalidSoapAction"), new object[] { str7 }));
                }
                sinkStack.Push(this, null);
                complete = this._nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, null, out responseMsg, out responseHeaders, out responseStream);
                if (responseStream != null)
                {
                    throw new RemotingException(CoreChannel.GetResourceString("Remoting_ChnlSink_WantNullResponseStream"));
                }
                switch (complete)
                {
                    case ServerProcessing.Complete:
                        if (responseMsg == null)
                        {
                            throw new RemotingException(CoreChannel.GetResourceString("Remoting_DispatchMessage"));
                        }
                        break;

                    case ServerProcessing.OneWay:
                        sinkStack.Pop(this);
                        return complete;

                    case ServerProcessing.Async:
                        sinkStack.Store(this, null);
                        return complete;

                    default:
                        return complete;
                }
                sinkStack.Pop(this);
                this.SerializeResponse(sinkStack, responseMsg, bClientIsClr, ref responseHeaders, out responseStream);
                return complete;
            }
            catch (Exception exception)
            {
                complete = ServerProcessing.Complete;
                responseMsg = new ReturnMessage(exception, (requestMsg == null) ? ((IMethodCallMessage) new System.Runtime.Remoting.Channels.Http.ErrorMessage()) : ((IMethodCallMessage) requestMsg));
                CallContext.SetData("__ClientIsClr", bClientIsClr);
                responseStream = (MemoryStream) CoreChannel.SerializeSoapMessage(responseMsg, this._includeVersioning);
                CallContext.FreeNamedDataSlot("__ClientIsClr");
                responseStream.Position = 0L;
                responseHeaders = new TransportHeaders();
                if (this._protocol == Protocol.Http)
                {
                    responseHeaders["__HttpStatusCode"] = "500";
                    responseHeaders["__HttpReasonPhrase"] = "Internal Server Error";
                    responseHeaders["Content-Type"] = "text/xml; charset=\"utf-8\"";
                }
            }
            catch
            {
                complete = ServerProcessing.Complete;
                responseMsg = new ReturnMessage(new Exception(CoreChannel.GetResourceString("Remoting_nonClsCompliantException")), (requestMsg == null) ? ((IMethodCallMessage) new System.Runtime.Remoting.Channels.Http.ErrorMessage()) : ((IMethodCallMessage) requestMsg));
                CallContext.SetData("__ClientIsClr", bClientIsClr);
                responseStream = (MemoryStream) CoreChannel.SerializeSoapMessage(responseMsg, this._includeVersioning);
                CallContext.FreeNamedDataSlot("__ClientIsClr");
                responseStream.Position = 0L;
                responseHeaders = new TransportHeaders();
                if (this._protocol == Protocol.Http)
                {
                    responseHeaders["__HttpStatusCode"] = "500";
                    responseHeaders["__HttpReasonPhrase"] = "Internal Server Error";
                    responseHeaders["Content-Type"] = "text/xml; charset=\"utf-8\"";
                }
            }
            finally
            {
                CallContext.FreeNamedDataSlot("__CustomErrorsEnabled");
            }
            return complete;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure, Infrastructure=true)]
        private void SerializeResponse(IServerResponseChannelSinkStack sinkStack, IMessage msg, bool bClientIsClr, ref ITransportHeaders headers, out Stream stream)
        {
            BaseTransportHeaders headers2 = new BaseTransportHeaders();
            if (headers != null)
            {
                foreach (DictionaryEntry entry in headers)
                {
                    headers2[entry.Key] = entry.Value;
                }
            }
            headers = headers2;
            headers2.ContentType = "text/xml; charset=\"utf-8\"";
            if (this._protocol == Protocol.Http)
            {
                IMethodReturnMessage message = msg as IMethodReturnMessage;
                if ((message != null) && (message.Exception != null))
                {
                    headers["__HttpStatusCode"] = "500";
                    headers["__HttpReasonPhrase"] = "Internal Server Error";
                }
            }
            bool flag = false;
            stream = sinkStack.GetResponseStream(msg, headers);
            if (stream == null)
            {
                stream = new ChunkedMemoryStream(CoreChannel.BufferPool);
                flag = true;
            }
            bool bBashedUrl = CoreChannel.SetupUrlBashingForIisSslIfNecessary();
            CallContext.SetData("__ClientIsClr", bClientIsClr);
            try
            {
                CoreChannel.SerializeSoapMessage(msg, stream, this._includeVersioning);
            }
            finally
            {
                CallContext.FreeNamedDataSlot("__ClientIsClr");
                CoreChannel.CleanupUrlBashingForIisSslIfNecessary(bBashedUrl);
            }
            if (flag)
            {
                stream.Position = 0L;
            }
        }

        internal bool IncludeVersioning
        {
            set
            {
                this._includeVersioning = value;
            }
        }

        public IServerChannelSink NextChannelSink =>
            this._nextSink;

        public IDictionary Properties =>
            null;

        internal bool StrictBinding
        {
            set
            {
                this._strictBinding = value;
            }
        }

        [ComVisible(false)]
        public System.Runtime.Serialization.Formatters.TypeFilterLevel TypeFilterLevel
        {
            get => 
                this._formatterSecurityLevel;
            set
            {
                this._formatterSecurityLevel = value;
            }
        }

        [Serializable]
        public enum Protocol
        {
            Http,
            Other
        }
    }
}

