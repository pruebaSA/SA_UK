namespace System.ServiceModel.Description
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Xml;

    public sealed class WebScriptEnablingBehavior : WebHttpBehavior
    {
        private static readonly DataContractJsonSerializer jsonFaultSerializer = new DataContractJsonSerializer(typeof(JsonFaultDetail));
        private const int MaxMetadataEndpointBufferSize = 0x800;
        private WebMessageFormat requestMessageFormat = webScriptDefaultMessageFormat;
        private WebMessageFormat responseMessageFormat = webScriptDefaultMessageFormat;
        private static readonly WebMessageBodyStyle webScriptBodyStyle = WebMessageBodyStyle.WrappedRequest;
        private static readonly WebMessageFormat webScriptDefaultMessageFormat = WebMessageFormat.Json;

        protected override void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new JsonClientMessageInspector());
        }

        private void AddMetadataEndpoint(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher, bool debugMode)
        {
            Uri uri = endpoint.Address.Uri;
            if (uri != null)
            {
                HttpTransportBindingElement element;
                ServiceHostBase host = endpointDispatcher.ChannelDispatcher.Host;
                UriBuilder builder = new UriBuilder(uri);
                builder.Path = builder.Path + (builder.Path.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? WebScriptClientGenerator.GetMetadataEndpointSuffix(debugMode) : ("/" + WebScriptClientGenerator.GetMetadataEndpointSuffix(debugMode)));
                EndpointAddress address = new EndpointAddress(builder.Uri, new AddressHeader[0]);
                foreach (ServiceEndpoint endpoint2 in host.Description.Endpoints)
                {
                    if (EndpointAddress.UriEquals(endpoint2.Address.Uri, address.Uri, true, false))
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.JsonNoEndpointAtMetadataAddress, new object[] { base.GetType().ToString(), endpoint2.Address, endpoint2.Name, host.Description.Name })));
                    }
                }
                HttpTransportBindingElement element2 = endpoint.Binding.CreateBindingElements().Find<HttpTransportBindingElement>();
                if (element2 != null)
                {
                    element = (HttpTransportBindingElement) element2.Clone();
                }
                else if (uri.Scheme == "https")
                {
                    element = new HttpsTransportBindingElement();
                }
                else
                {
                    element = new HttpTransportBindingElement();
                }
                element.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                element.TransferMode = TransferMode.Buffered;
                element.MaxBufferSize = 0x800;
                element.MaxReceivedMessageSize = 0x800L;
                Binding binding = new CustomBinding(new BindingElement[] { new WebScriptMetadataMessageEncodingBindingElement(), element });
                BindingParameterCollection parameters = new BindingParameterCollection();
                VirtualPathExtension item = host.Extensions.Find<VirtualPathExtension>();
                if (item != null)
                {
                    parameters.Add(item);
                }
                ContractDescription contract = ContractDescription.GetContract(typeof(ServiceMetadataExtension.IHttpGetMetadata));
                OperationDescription description2 = contract.Operations[0];
                EndpointDispatcher dispatcher = new EndpointDispatcher(address, contract.Name, contract.Namespace);
                DispatchOperation operation = new DispatchOperation(dispatcher.DispatchRuntime, description2.Name, description2.Messages[0].Action, description2.Messages[1].Action) {
                    Formatter = new WebScriptMetadataFormatter(),
                    Invoker = new SyncMethodInvoker(description2.SyncMethod)
                };
                dispatcher.DispatchRuntime.Operations.Add(operation);
                dispatcher.DispatchRuntime.SingletonInstanceContext = new InstanceContext(host, new WebScriptClientGenerator(endpoint, debugMode));
                dispatcher.DispatchRuntime.InstanceContextProvider = new WebScriptMetadataInstanceContextProvider(dispatcher.DispatchRuntime.SingletonInstanceContext);
                IChannelListener<IReplyChannel> listener = null;
                if (binding.CanBuildChannelListener<IReplyChannel>(parameters))
                {
                    listener = binding.BuildChannelListener<IReplyChannel>(address.Uri, parameters);
                }
                ChannelDispatcher dispatcher2 = new ChannelDispatcher(listener) {
                    MessageVersion = MessageVersion.None,
                    Endpoints = { dispatcher }
                };
                host.ChannelDispatchers.Add(dispatcher2);
            }
        }

        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpointDispatcher.ChannelDispatcher == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("endpointDispatcher", SR2.GetString(SR2.ChannelDispatcherMustBePresent, new object[0]));
            }
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new JsonErrorHandler(endpoint, endpointDispatcher.ChannelDispatcher.IncludeExceptionDetailInFaults));
        }

        public override void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            base.ApplyClientBehavior(endpoint, clientRuntime);
            clientRuntime.MessageInspectors.Add(new JsonClientMessageInspector());
        }

        public override void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            base.ApplyDispatchBehavior(endpoint, endpointDispatcher);
            try
            {
                this.AddMetadataEndpoint(endpoint, endpointDispatcher, false);
                this.AddMetadataEndpoint(endpoint, endpointDispatcher, true);
            }
            catch (XmlException exception)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.InvalidXmlCharactersInNameUsedWithPOSTMethod, new object[] { string.Empty, string.Empty, string.Empty }), exception));
            }
        }

        internal override DataContractJsonSerializerOperationFormatter CreateDataContractJsonSerializerOperationFormatter(OperationDescription od, DataContractSerializerOperationBehavior dcsob, bool isWrapped) => 
            new DataContractJsonSerializerOperationFormatter(od, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject, dcsob.DataContractSurrogate, isWrapped, true);

        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription) => 
            new JsonQueryStringConverter(operationDescription);

        internal override string GetWmiTypeName() => 
            "WebScriptEnablingBehavior";

        internal override bool UseBareReplyFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, WebMessageFormat responseFormat, out Type parameterType)
        {
            if (responseFormat == WebMessageFormat.Json)
            {
                parameterType = null;
                return false;
            }
            return base.UseBareReplyFormatter(style, operationDescription, responseFormat, out parameterType);
        }

        public override void Validate(ServiceEndpoint endpoint)
        {
            base.Validate(endpoint);
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                if (description.Behaviors.Find<XmlSerializerOperationBehavior>() != null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebScriptNotSupportedForXmlSerializerFormat, new object[] { typeof(XmlSerializerFormatAttribute).Name, base.GetType().ToString() })));
                }
                string webMethod = WebHttpBehavior.GetWebMethod(description);
                if ((webMethod != "GET") && (webMethod != "POST"))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebScriptInvalidHttpRequestMethod, new object[] { description.Name, endpoint.Contract.Name, webMethod, base.GetType().ToString() })));
                }
                WebGetAttribute attribute = description.Behaviors.Find<WebGetAttribute>();
                if ((attribute != null) && (attribute.UriTemplate != null))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebScriptNotSupportedForXmlSerializerFormat, new object[] { typeof(UriTemplate).Name, base.GetType().ToString() })));
                }
                WebInvokeAttribute attribute2 = description.Behaviors.Find<WebInvokeAttribute>();
                if ((attribute2 != null) && (attribute2.UriTemplate != null))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebScriptNotSupportedForXmlSerializerFormat, new object[] { typeof(UriTemplate).Name, base.GetType().ToString() })));
                }
                WebMessageBodyStyle bodyStyle = base.GetBodyStyle(description);
                if (bodyStyle != webScriptBodyStyle)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.BodyStyleNotSupportedByWebScript, new object[] { bodyStyle, base.GetType().Name, webScriptBodyStyle })));
                }
                foreach (MessageDescription description2 in description.Messages)
                {
                    if ((!description2.IsTypedMessage && (description2.Direction == MessageDirection.Output)) && (description2.Body.Parts.Count > 0))
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebScriptOutRefOperationsNotSupported, new object[] { description.Name, endpoint.Contract.Name })));
                    }
                }
            }
        }

        public override WebMessageBodyStyle DefaultBodyStyle
        {
            get => 
                webScriptBodyStyle;
            set
            {
                if (value != webScriptBodyStyle)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.BodyStyleNotSupportedByWebScript, new object[] { value, base.GetType().Name, webScriptBodyStyle })));
                }
            }
        }

        public override WebMessageFormat DefaultOutgoingRequestFormat
        {
            get => 
                this.requestMessageFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.requestMessageFormat = value;
            }
        }

        public override WebMessageFormat DefaultOutgoingResponseFormat
        {
            get => 
                this.responseMessageFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.responseMessageFormat = value;
            }
        }

        private class JsonClientMessageInspector : WebFaultClientMessageInspector
        {
            public override void AfterReceiveReply(ref Message reply, object correlationState)
            {
                bool flag = true;
                if (reply != null)
                {
                    object obj2 = reply.Properties[HttpResponseMessageProperty.Name];
                    if ((obj2 != null) && (((HttpResponseMessageProperty) obj2).Headers["jsonerror"] == "true"))
                    {
                        flag = false;
                        XmlDictionaryReader readerAtBodyContents = reply.GetReaderAtBodyContents();
                        WebScriptEnablingBehavior.JsonFaultDetail detail = WebScriptEnablingBehavior.jsonFaultSerializer.ReadObject(readerAtBodyContents) as WebScriptEnablingBehavior.JsonFaultDetail;
                        FaultCode subCode = new FaultCode("InternalServiceFault", "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher");
                        subCode = FaultCode.CreateReceiverFaultCode(subCode);
                        if (detail == null)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FaultException(MessageFault.CreateFault(subCode, System.ServiceModel.SR.GetString("SFxInternalServerError"))));
                        }
                        if (detail.ExceptionDetail != null)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FaultException<ExceptionDetail>(detail.ExceptionDetail, detail.Message, subCode));
                        }
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FaultException(MessageFault.CreateFault(subCode, detail.Message)));
                    }
                }
                if (flag)
                {
                    base.AfterReceiveReply(ref reply, correlationState);
                }
            }
        }

        private class JsonErrorHandler : IErrorHandler
        {
            private bool includeExceptionDetailInFaults;
            private string outgoingContentType;

            public JsonErrorHandler(ServiceEndpoint endpoint, bool includeExceptionDetailInFaults)
            {
                WebMessageEncodingBindingElement encodingElement = endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();
                this.outgoingContentType = JsonMessageEncoderFactory.GetContentType(encodingElement);
                this.includeExceptionDetailInFaults = includeExceptionDetailInFaults;
            }

            public bool HandleError(Exception error) => 
                false;

            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                HttpResponseMessageProperty property;
                object obj3;
                if (fault == null)
                {
                    FaultCode subCode = new FaultCode("InternalServiceFault", "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher");
                    subCode = FaultCode.CreateReceiverFaultCode(subCode);
                    string action = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher/fault";
                    MessageFault fault2 = MessageFault.CreateFault(subCode, new FaultReason(error.Message, CultureInfo.CurrentCulture), new ExceptionDetail(error));
                    fault = Message.CreateMessage(version, action, (BodyWriter) new JsonFaultBodyWriter(fault2, this.includeExceptionDetailInFaults));
                    property = new HttpResponseMessageProperty();
                    fault.Properties.Add(HttpResponseMessageProperty.Name, property);
                }
                else
                {
                    MessageFault fault3 = MessageFault.CreateFault(fault, 0x10000);
                    Message message = Message.CreateMessage(version, fault.Headers.Action, (BodyWriter) new JsonFaultBodyWriter(fault3, this.includeExceptionDetailInFaults));
                    message.Headers.To = fault.Headers.To;
                    message.Properties.CopyProperties(fault.Properties);
                    object obj2 = null;
                    if (message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj2))
                    {
                        property = (HttpResponseMessageProperty) obj2;
                    }
                    else
                    {
                        property = new HttpResponseMessageProperty();
                        message.Properties.Add(HttpResponseMessageProperty.Name, property);
                    }
                    fault.Close();
                    fault = message;
                }
                property.Headers.Add(HttpResponseHeader.ContentType, this.outgoingContentType);
                property.Headers.Add("jsonerror", "true");
                property.StatusCode = HttpStatusCode.InternalServerError;
                if (fault.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj3))
                {
                    WebBodyFormatMessageProperty property2 = obj3 as WebBodyFormatMessageProperty;
                    if ((property2 == null) || (property2.Format != WebContentFormat.Json))
                    {
                        fault.Properties["WebBodyFormatMessageProperty"] = WebBodyFormatMessageProperty.JsonProperty;
                    }
                }
                else
                {
                    fault.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.JsonProperty);
                }
            }

            private class JsonFaultBodyWriter : BodyWriter
            {
                private WebScriptEnablingBehavior.JsonFaultDetail faultDetail;

                public JsonFaultBodyWriter(MessageFault fault, bool includeExceptionDetailInFaults) : base(false)
                {
                    this.faultDetail = new WebScriptEnablingBehavior.JsonFaultDetail();
                    if (includeExceptionDetailInFaults)
                    {
                        this.faultDetail.Message = fault.Reason.ToString();
                        if (fault.HasDetail)
                        {
                            try
                            {
                                ExceptionDetail detail = fault.GetDetail<ExceptionDetail>();
                                this.faultDetail.StackTrace = detail.StackTrace;
                                this.faultDetail.ExceptionType = detail.Type;
                                this.faultDetail.ExceptionDetail = detail;
                            }
                            catch (SerializationException exception)
                            {
                                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                            }
                            catch (SecurityException exception2)
                            {
                                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                            }
                        }
                    }
                    else
                    {
                        this.faultDetail.Message = System.ServiceModel.SR.GetString("SFxInternalServerError");
                    }
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    WebScriptEnablingBehavior.jsonFaultSerializer.WriteObject(writer, this.faultDetail);
                }
            }
        }

        [DataContract]
        internal class JsonFaultDetail
        {
            private System.ServiceModel.ExceptionDetail exceptionDetail;
            private string exceptionType;
            private string message;
            private string stackTrace;

            [DataMember(Name="ExceptionDetail")]
            public System.ServiceModel.ExceptionDetail ExceptionDetail
            {
                get => 
                    this.exceptionDetail;
                set
                {
                    this.exceptionDetail = value;
                }
            }

            [DataMember(Name="ExceptionType")]
            public string ExceptionType
            {
                get => 
                    this.exceptionType;
                set
                {
                    this.exceptionType = value;
                }
            }

            [DataMember(Name="Message")]
            public string Message
            {
                get => 
                    this.message;
                set
                {
                    this.message = value;
                }
            }

            [DataMember(Name="StackTrace")]
            public string StackTrace
            {
                get => 
                    this.stackTrace;
                set
                {
                    this.stackTrace = value;
                }
            }
        }
    }
}

