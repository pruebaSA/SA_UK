namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Administration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;

    public class WebHttpBehavior : IEndpointBehavior, IWmiInstanceProvider
    {
        private WebMessageBodyStyle defaultBodyStyle = WebMessageBodyStyle.Bare;
        private WebMessageFormat defaultOutgoingReplyFormat = WebMessageFormat.Xml;
        private WebMessageFormat defaultOutgoingRequestFormat = WebMessageFormat.Xml;
        private static readonly string defaultStreamContentType = "application/octet-stream";
        internal const string GET = "GET";
        internal const string POST = "POST";
        private XmlSerializerOperationBehavior.Reflector reflector;
        internal const string WildcardAction = "*";
        internal const string WildcardMethod = "*";
        private UnwrappedTypesXmlSerializerManager xmlSerializerManager = new UnwrappedTypesXmlSerializerManager();

        public virtual void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        protected virtual void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new WebFaultClientMessageInspector());
        }

        protected virtual void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(new WebErrorHandler(endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults));
        }

        public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
            }
            if (clientRuntime == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientRuntime");
            }
            this.reflector = new XmlSerializerOperationBehavior.Reflector(endpoint.Contract.Namespace, null);
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                if (clientRuntime.Operations.Contains(description.Name))
                {
                    ClientOperation operation = clientRuntime.Operations[description.Name];
                    IClientMessageFormatter requestClientFormatter = this.GetRequestClientFormatter(description, endpoint);
                    IClientMessageFormatter replyClientFormatter = this.GetReplyClientFormatter(description, endpoint);
                    operation.Formatter = new CompositeClientFormatter(requestClientFormatter, replyClientFormatter);
                    operation.SerializeRequest = true;
                    operation.DeserializeReply = (description.Messages.Count > 1) && !IsUntypedMessage(description.Messages[1]);
                }
            }
            this.AddClientErrorInspector(endpoint, clientRuntime);
        }

        public virtual void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
            }
            if (endpointDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointDispatcher");
            }
            this.reflector = new XmlSerializerOperationBehavior.Reflector(endpoint.Contract.Namespace, null);
            endpointDispatcher.AddressFilter = new PrefixEndpointAddressMessageFilter(endpoint.Address);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();
            endpointDispatcher.DispatchRuntime.OperationSelector = this.GetOperationSelector(endpoint);
            string name = null;
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                if ((description.Messages[0].Direction == MessageDirection.Input) && (description.Messages[0].Action == "*"))
                {
                    name = description.Name;
                    break;
                }
            }
            if (name != null)
            {
                endpointDispatcher.DispatchRuntime.Operations.Add(endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation);
            }
            endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation = new DispatchOperation(endpointDispatcher.DispatchRuntime, "*", "*", "*");
            endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.DeserializeRequest = false;
            endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.SerializeReply = false;
            endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Invoker = new HttpUnhandledOperationInvoker();
            foreach (OperationDescription description2 in endpoint.Contract.Operations)
            {
                DispatchOperation unhandledDispatchOperation = null;
                if (endpointDispatcher.DispatchRuntime.Operations.Contains(description2.Name))
                {
                    unhandledDispatchOperation = endpointDispatcher.DispatchRuntime.Operations[description2.Name];
                }
                else if (endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Name == description2.Name)
                {
                    unhandledDispatchOperation = endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation;
                }
                if (unhandledDispatchOperation != null)
                {
                    IDispatchMessageFormatter requestDispatchFormatter = this.GetRequestDispatchFormatter(description2, endpoint);
                    IDispatchMessageFormatter replyDispatchFormatter = this.GetReplyDispatchFormatter(description2, endpoint);
                    unhandledDispatchOperation.Formatter = new CompositeDispatchFormatter(requestDispatchFormatter, replyDispatchFormatter);
                    unhandledDispatchOperation.DeserializeRequest = requestDispatchFormatter != null;
                    unhandledDispatchOperation.SerializeReply = (description2.Messages.Count > 1) && (replyDispatchFormatter != null);
                }
            }
            this.AddServerErrorHandlers(endpoint, endpointDispatcher);
        }

        private static void CloneMessageDescriptionsBeforeActing(OperationDescription operationDescription, Effect effect)
        {
            MessageDescription description = operationDescription.Messages[0];
            bool flag = operationDescription.Messages.Count > 1;
            MessageDescription description2 = flag ? operationDescription.Messages[1] : null;
            operationDescription.Messages[0] = description.Clone();
            if (flag)
            {
                operationDescription.Messages[1] = description2.Clone();
            }
            effect();
            operationDescription.Messages[0] = description;
            if (flag)
            {
                operationDescription.Messages[1] = description2;
            }
        }

        private static Collection<MessagePartDescription> CloneParts(MessageDescription md)
        {
            MessagePartDescriptionCollection parts = md.Body.Parts;
            Collection<MessagePartDescription> collection = new Collection<MessagePartDescription>();
            for (int i = 0; i < parts.Count; i++)
            {
                MessagePartDescription item = parts[i].Clone();
                collection.Add(item);
            }
            return collection;
        }

        internal virtual DataContractJsonSerializerOperationFormatter CreateDataContractJsonSerializerOperationFormatter(OperationDescription od, DataContractSerializerOperationBehavior dcsob, bool isWrapped) => 
            new DataContractJsonSerializerOperationFormatter(od, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject, dcsob.DataContractSurrogate, isWrapped, false);

        private static void EnsureNotUntypedMessageNorMessageContract(OperationDescription operationDescription)
        {
            bool flag = false;
            if ((GetWebMethod(operationDescription) == "GET") && (GetWebUriTemplate(operationDescription) == null))
            {
                flag = true;
            }
            if (IsTypedMessage(operationDescription.Messages[0]))
            {
                if (flag)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveMCParameter, new object[] { operationDescription.Name, operationDescription.DeclaringContract.Name, operationDescription.Messages[0].MessageType.Name })));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTParamsDoNotComposeWithMessageContract, new object[] { operationDescription.Name, operationDescription.DeclaringContract.Name })));
            }
            if (IsUntypedMessage(operationDescription.Messages[0]))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTParamsDoNotComposeWithMessage, new object[] { operationDescription.Name, operationDescription.DeclaringContract.Name })));
            }
        }

        private static void EnsureOk(WebGetAttribute wga, WebInvokeAttribute wia, OperationDescription od)
        {
            if ((wga != null) && (wia != null))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.MultipleWebAttributes, new object[] { od.Name, od.DeclaringContract.Name })));
            }
        }

        internal WebMessageBodyStyle GetBodyStyle(OperationDescription od)
        {
            WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
            WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.GetBodyStyleOrDefault(this.DefaultBodyStyle);
            }
            if (wia != null)
            {
                return wia.GetBodyStyleOrDefault(this.DefaultBodyStyle);
            }
            return this.DefaultBodyStyle;
        }

        internal IClientMessageFormatter GetDefaultClientFormatter(OperationDescription od, bool useJson, bool isWrapped)
        {
            DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (useJson)
            {
                if (dcsob == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.JsonFormatRequiresDataContract, new object[] { od.Name, od.DeclaringContract.Name, od.DeclaringContract.Namespace })));
                }
                return this.CreateDataContractJsonSerializerOperationFormatter(od, dcsob, isWrapped);
            }
            ClientRuntime parent = new ClientRuntime("name", "");
            ClientOperation clientOperation = new ClientOperation(parent, "dummyClient", "urn:dummy") {
                Formatter = null
            };
            if (dcsob != null)
            {
                ((IOperationBehavior) dcsob).ApplyClientBehavior(od, clientOperation);
                return clientOperation.Formatter;
            }
            XmlSerializerOperationBehavior behavior2 = od.Behaviors.Find<XmlSerializerOperationBehavior>();
            if (behavior2 != null)
            {
                behavior2 = new XmlSerializerOperationBehavior(od, behavior2.XmlSerializerFormatAttribute, this.reflector);
                ((IOperationBehavior) behavior2).ApplyClientBehavior(od, clientOperation);
                return clientOperation.Formatter;
            }
            return null;
        }

        private string GetDefaultContentType(bool isStream, bool useJson, WebMessageEncodingBindingElement webEncoding)
        {
            if (isStream)
            {
                return defaultStreamContentType;
            }
            if (useJson)
            {
                return JsonMessageEncoderFactory.GetContentType(webEncoding);
            }
            return null;
        }

        private IDispatchMessageFormatter GetDefaultDispatchFormatter(OperationDescription od, bool useJson, bool isWrapped)
        {
            DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (useJson)
            {
                if (dcsob == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.JsonFormatRequiresDataContract, new object[] { od.Name, od.DeclaringContract.Name, od.DeclaringContract.Namespace })));
                }
                return this.CreateDataContractJsonSerializerOperationFormatter(od, dcsob, isWrapped);
            }
            EndpointDispatcher dispatcher = new EndpointDispatcher(new EndpointAddress("http://localhost/"), "name", "");
            DispatchOperation dispatchOperation = new DispatchOperation(dispatcher.DispatchRuntime, "dummyDispatch", "urn:dummy") {
                Formatter = null
            };
            if (dcsob != null)
            {
                ((IOperationBehavior) dcsob).ApplyDispatchBehavior(od, dispatchOperation);
                return dispatchOperation.Formatter;
            }
            XmlSerializerOperationBehavior behavior2 = od.Behaviors.Find<XmlSerializerOperationBehavior>();
            if (behavior2 != null)
            {
                behavior2 = new XmlSerializerOperationBehavior(od, behavior2.XmlSerializerFormatAttribute, this.reflector);
                ((IOperationBehavior) behavior2).ApplyDispatchBehavior(od, dispatchOperation);
                return dispatchOperation.Formatter;
            }
            return null;
        }

        private IClientMessageFormatter GetDefaultXmlAndJsonClientFormatter(OperationDescription od, bool isWrapped)
        {
            IClientMessageFormatter defaultFormatter = this.GetDefaultClientFormatter(od, false, isWrapped);
            if (!SupportsJsonFormat(od))
            {
                return defaultFormatter;
            }
            IClientMessageFormatter formatter2 = this.GetDefaultClientFormatter(od, true, isWrapped);
            return new DemultiplexingClientMessageFormatter(new Dictionary<WebContentFormat, IClientMessageFormatter> { 
                { 
                    WebContentFormat.Xml,
                    defaultFormatter
                },
                { 
                    WebContentFormat.Json,
                    formatter2
                }
            }, defaultFormatter);
        }

        private IDispatchMessageFormatter GetDefaultXmlAndJsonDispatchFormatter(OperationDescription od, bool isWrapped)
        {
            IDispatchMessageFormatter defaultFormatter = this.GetDefaultDispatchFormatter(od, false, isWrapped);
            if (!SupportsJsonFormat(od))
            {
                return defaultFormatter;
            }
            IDispatchMessageFormatter formatter2 = this.GetDefaultDispatchFormatter(od, true, isWrapped);
            return new DemultiplexingDispatchMessageFormatter(new Dictionary<WebContentFormat, IDispatchMessageFormatter> { 
                { 
                    WebContentFormat.Xml,
                    defaultFormatter
                },
                { 
                    WebContentFormat.Json,
                    formatter2
                }
            }, defaultFormatter);
        }

        protected virtual WebHttpDispatchOperationSelector GetOperationSelector(ServiceEndpoint endpoint) => 
            new WebHttpDispatchOperationSelector(endpoint);

        protected virtual QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription) => 
            new QueryStringConverter();

        protected virtual IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            Type type;
            Type type2;
            if (operationDescription.Messages.Count < 2)
            {
                return null;
            }
            this.ValidateBodyParameters(operationDescription, false);
            if (TryGetStreamParameterType(operationDescription.Messages[1], operationDescription, false, out type))
            {
                return new HttpStreamFormatter(operationDescription);
            }
            if (IsUntypedMessage(operationDescription.Messages[1]))
            {
                return new MessagePassthroughFormatter();
            }
            WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operationDescription);
            if (this.UseBareReplyFormatter(bodyStyle, operationDescription, this.GetResponseFormat(operationDescription), out type2))
            {
                return SingleBodyParameterMessageFormatter.CreateXmlAndJsonClientFormatter(operationDescription, type2, false, this.xmlSerializerManager);
            }
            MessageDescription description = operationDescription.Messages[0];
            operationDescription.Messages[0] = MakeDummyMessageDescription(MessageDirection.Input);
            IClientMessageFormatter defaultXmlAndJsonClientFormatter = this.GetDefaultXmlAndJsonClientFormatter(operationDescription, !IsBareResponse(bodyStyle));
            operationDescription.Messages[0] = description;
            return defaultXmlAndJsonClientFormatter;
        }

        protected virtual IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            IDispatchMessageFormatter formatter;
            Type type;
            if (operationDescription.Messages.Count < 2)
            {
                return null;
            }
            this.ValidateBodyParameters(operationDescription, false);
            WebMessageFormat responseFormat = this.GetResponseFormat(operationDescription);
            bool useJson = responseFormat == WebMessageFormat.Json;
            WebMessageEncodingBindingElement webEncoding = useJson ? endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() : null;
            bool isStream = false;
            if (TryGetStreamParameterType(operationDescription.Messages[1], operationDescription, false, out type))
            {
                isStream = true;
                formatter = new HttpStreamFormatter(operationDescription);
            }
            else if (IsUntypedMessage(operationDescription.Messages[1]))
            {
                formatter = new MessagePassthroughFormatter();
            }
            else
            {
                Type type2;
                WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operationDescription);
                if (this.UseBareReplyFormatter(bodyStyle, operationDescription, responseFormat, out type2))
                {
                    formatter = SingleBodyParameterMessageFormatter.CreateDispatchFormatter(operationDescription, type2, false, useJson, this.xmlSerializerManager);
                }
                else
                {
                    MessageDescription description = operationDescription.Messages[0];
                    operationDescription.Messages[0] = MakeDummyMessageDescription(MessageDirection.Input);
                    formatter = this.GetDefaultDispatchFormatter(operationDescription, useJson, !IsBareResponse(bodyStyle));
                    operationDescription.Messages[0] = description;
                }
            }
            string str = this.GetDefaultContentType(isStream, useJson, webEncoding);
            if (!string.IsNullOrEmpty(str))
            {
                return new ContentTypeSettingDispatchMessageFormatter(str, formatter);
            }
            return formatter;
        }

        protected virtual IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            WebMessageFormat requestFormat = this.GetRequestFormat(operationDescription);
            bool useJson = requestFormat == WebMessageFormat.Json;
            WebMessageEncodingBindingElement webEncoding = useJson ? endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() : null;
            IClientMessageFormatter innerFormatter = null;
            UriTemplateClientFormatter throwAway = new UriTemplateClientFormatter(operationDescription, null, this.GetQueryStringConverter(operationDescription), endpoint.Address.Uri, false, endpoint.Contract.Name);
            int numUriVariables = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
            bool isStream = false;
            HideReplyMessage(operationDescription, delegate {
                Effect effect = null;
                WebMessageBodyStyle style = this.GetBodyStyle(operationDescription);
                bool isUntypedWhenUriParamsNotConsidered = false;
                Effect doBodyFormatter = delegate {
                    IClientMessageFormatter formatter;
                    Type type;
                    if (numUriVariables != 0)
                    {
                        EnsureNotUntypedMessageNorMessageContract(operationDescription);
                    }
                    this.ValidateBodyParameters(operationDescription, true);
                    if (TryGetStreamParameterType(operationDescription.Messages[0], operationDescription, true, out type))
                    {
                        isStream = true;
                        formatter = new HttpStreamFormatter(operationDescription);
                    }
                    else if (this.UseBareRequestFormatter(style, operationDescription, out type))
                    {
                        formatter = SingleBodyParameterMessageFormatter.CreateClientFormatter(operationDescription, type, true, useJson, this.xmlSerializerManager);
                    }
                    else
                    {
                        formatter = this.GetDefaultClientFormatter(operationDescription, useJson, !IsBareRequest(style));
                    }
                    innerFormatter = formatter;
                    isUntypedWhenUriParamsNotConsidered = IsUntypedMessage(operationDescription.Messages[0]);
                };
                if (numUriVariables == 0)
                {
                    if (IsUntypedMessage(operationDescription.Messages[0]))
                    {
                        this.ValidateBodyParameters(operationDescription, true);
                        innerFormatter = new MessagePassthroughFormatter();
                        isUntypedWhenUriParamsNotConsidered = true;
                    }
                    else if (IsTypedMessage(operationDescription.Messages[0]))
                    {
                        this.ValidateBodyParameters(operationDescription, true);
                        innerFormatter = this.GetDefaultClientFormatter(operationDescription, useJson, !IsBareRequest(style));
                    }
                    else
                    {
                        doBodyFormatter();
                    }
                }
                else
                {
                    if (effect == null)
                    {
                        effect = () => CloneMessageDescriptionsBeforeActing(operationDescription, () => doBodyFormatter());
                    }
                    HideRequestUriTemplateParameters(operationDescription, throwAway, effect);
                }
                innerFormatter = new UriTemplateClientFormatter(operationDescription, innerFormatter, this.GetQueryStringConverter(operationDescription), endpoint.Address.Uri, isUntypedWhenUriParamsNotConsidered, endpoint.Contract.Name);
            });
            string str = this.GetDefaultContentType(isStream, useJson, webEncoding);
            if (!string.IsNullOrEmpty(str))
            {
                innerFormatter = new ContentTypeSettingClientMessageFormatter(str, innerFormatter);
            }
            return innerFormatter;
        }

        protected virtual IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            IDispatchMessageFormatter result = null;
            UriTemplateDispatchFormatter throwAway = new UriTemplateDispatchFormatter(operationDescription, null, this.GetQueryStringConverter(operationDescription), endpoint.Contract.Name, endpoint.Address.Uri);
            int numUriVariables = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
            HideReplyMessage(operationDescription, delegate {
                Effect effect = null;
                WebMessageBodyStyle style = this.GetBodyStyle(operationDescription);
                Effect doBodyFormatter = delegate {
                    Type type;
                    if (numUriVariables != 0)
                    {
                        EnsureNotUntypedMessageNorMessageContract(operationDescription);
                    }
                    this.ValidateBodyParameters(operationDescription, true);
                    if (TryGetStreamParameterType(operationDescription.Messages[0], operationDescription, true, out type))
                    {
                        result = new HttpStreamFormatter(operationDescription);
                    }
                    else
                    {
                        Type type2;
                        if (this.UseBareRequestFormatter(style, operationDescription, out type2))
                        {
                            result = SingleBodyParameterMessageFormatter.CreateXmlAndJsonDispatchFormatter(operationDescription, type2, true, this.xmlSerializerManager);
                        }
                        else
                        {
                            result = this.GetDefaultXmlAndJsonDispatchFormatter(operationDescription, !IsBareRequest(style));
                        }
                    }
                };
                if (numUriVariables == 0)
                {
                    if (IsUntypedMessage(operationDescription.Messages[0]))
                    {
                        this.ValidateBodyParameters(operationDescription, true);
                        result = new MessagePassthroughFormatter();
                    }
                    else if (IsTypedMessage(operationDescription.Messages[0]))
                    {
                        this.ValidateBodyParameters(operationDescription, true);
                        result = this.GetDefaultXmlAndJsonDispatchFormatter(operationDescription, !IsBareRequest(style));
                    }
                    else
                    {
                        doBodyFormatter();
                    }
                }
                else
                {
                    if (effect == null)
                    {
                        effect = () => CloneMessageDescriptionsBeforeActing(operationDescription, () => doBodyFormatter());
                    }
                    HideRequestUriTemplateParameters(operationDescription, throwAway, effect);
                }
                result = new UriTemplateDispatchFormatter(operationDescription, result, this.GetQueryStringConverter(operationDescription), endpoint.Contract.Name, endpoint.Address.Uri);
            });
            return result;
        }

        private WebMessageFormat GetRequestFormat(OperationDescription od)
        {
            WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
            WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                if (!wga.IsRequestFormatSetExplicitly)
                {
                    return this.DefaultOutgoingRequestFormat;
                }
                return wga.RequestFormat;
            }
            if (wia == null)
            {
                return this.DefaultOutgoingRequestFormat;
            }
            if (!wia.IsRequestFormatSetExplicitly)
            {
                return this.DefaultOutgoingRequestFormat;
            }
            return wia.RequestFormat;
        }

        private WebMessageFormat GetResponseFormat(OperationDescription od)
        {
            WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
            WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                if (!wga.IsResponseFormatSetExplicitly)
                {
                    return this.DefaultOutgoingResponseFormat;
                }
                return wga.ResponseFormat;
            }
            if (wia == null)
            {
                return this.DefaultOutgoingResponseFormat;
            }
            if (!wia.IsResponseFormatSetExplicitly)
            {
                return this.DefaultOutgoingResponseFormat;
            }
            return wia.ResponseFormat;
        }

        internal static string GetWebMethod(OperationDescription od)
        {
            WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
            WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return "GET";
            }
            if (wia == null)
            {
                return "POST";
            }
            return (wia.Method ?? "POST");
        }

        internal static string GetWebUriTemplate(OperationDescription od)
        {
            WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
            WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.UriTemplate;
            }
            if (wia != null)
            {
                return wia.UriTemplate;
            }
            return null;
        }

        internal virtual Dictionary<string, string> GetWmiProperties() => 
            new Dictionary<string, string> { 
                { 
                    "DefaultBodyStyle",
                    this.DefaultBodyStyle.ToString()
                },
                { 
                    "DefaultOutgoingRequestFormat",
                    this.DefaultOutgoingRequestFormat.ToString()
                },
                { 
                    "DefaultOutgoingResponseFormat",
                    this.DefaultOutgoingResponseFormat.ToString()
                }
            };

        internal virtual string GetWmiTypeName() => 
            "WebHttpBehavior";

        private static void HideReplyMessage(OperationDescription operationDescription, Effect effect)
        {
            MessageDescription description = null;
            if (operationDescription.Messages.Count > 1)
            {
                description = operationDescription.Messages[1];
                operationDescription.Messages[1] = MakeDummyMessageDescription(MessageDirection.Output);
            }
            effect();
            if (operationDescription.Messages.Count > 1)
            {
                operationDescription.Messages[1] = description;
            }
        }

        private static void HideRequestUriTemplateParameters(OperationDescription operationDescription, UriTemplateClientFormatter throwAway, Effect effect)
        {
            HideRequestUriTemplateParameters(operationDescription, throwAway.pathMapping, throwAway.queryMapping, effect);
        }

        internal static void HideRequestUriTemplateParameters(OperationDescription operationDescription, UriTemplateDispatchFormatter throwAway, Effect effect)
        {
            HideRequestUriTemplateParameters(operationDescription, throwAway.pathMapping, throwAway.queryMapping, effect);
        }

        private static void HideRequestUriTemplateParameters(OperationDescription operationDescription, Dictionary<int, string> pathMapping, Dictionary<int, KeyValuePair<string, Type>> queryMapping, Effect effect)
        {
            Collection<MessagePartDescription> collection = CloneParts(operationDescription.Messages[0]);
            Collection<MessagePartDescription> collection2 = CloneParts(operationDescription.Messages[0]);
            operationDescription.Messages[0].Body.Parts.Clear();
            int num = 0;
            for (int i = 0; i < collection2.Count; i++)
            {
                if (!pathMapping.ContainsKey(i) && !queryMapping.ContainsKey(i))
                {
                    operationDescription.Messages[0].Body.Parts.Add(collection2[i]);
                    collection2[i].Index = num++;
                }
            }
            effect();
            operationDescription.Messages[0].Body.Parts.Clear();
            for (int j = 0; j < collection.Count; j++)
            {
                operationDescription.Messages[0].Body.Parts.Add(collection[j]);
            }
        }

        private static bool IsBareRequest(WebMessageBodyStyle style)
        {
            if (style != WebMessageBodyStyle.Bare)
            {
                return (style == WebMessageBodyStyle.WrappedResponse);
            }
            return true;
        }

        private static bool IsBareResponse(WebMessageBodyStyle style)
        {
            if (style != WebMessageBodyStyle.Bare)
            {
                return (style == WebMessageBodyStyle.WrappedRequest);
            }
            return true;
        }

        internal static bool IsTypedMessage(MessageDescription message) => 
            ((message != null) && (message.MessageType != null));

        internal static bool IsUntypedMessage(MessageDescription message) => 
            ((((message?.Body.ReturnValue != null) && (message?.Body.Parts.Count == 0)) && (message?.Body.ReturnValue.Type == typeof(Message))) || (((message?.Body.ReturnValue == null) && (message?.Body.Parts.Count == 1)) && (message?.Body.Parts[0].Type == typeof(Message))));

        internal static MessageDescription MakeDummyMessageDescription(MessageDirection direction) => 
            new MessageDescription("urn:dummyAction", direction);

        internal static bool SupportsJsonFormat(OperationDescription od) => 
            (od.Behaviors.Find<DataContractSerializerOperationBehavior>() != null);

        void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
        {
            if (wmiInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("wmiInstance");
            }
            Dictionary<string, string> wmiProperties = this.GetWmiProperties();
            foreach (string str in wmiProperties.Keys)
            {
                wmiInstance.SetProperty(str, wmiProperties[str]);
            }
        }

        string IWmiInstanceProvider.GetInstanceType() => 
            this.GetWmiTypeName();

        private static bool TryGetNonMessageParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out Type type)
        {
            type = null;
            if (message != null)
            {
                if (IsTypedMessage(message) || IsUntypedMessage(message))
                {
                    return false;
                }
                if (isRequest)
                {
                    if (message.Body.Parts.Count > 1)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.AtMostOneRequestBodyParameterAllowedForUnwrappedMessages, new object[] { declaringOperation.Name, declaringOperation.DeclaringContract.Name })));
                    }
                    if ((message.Body.Parts.Count == 1) && (message.Body.Parts[0].Type != typeof(void)))
                    {
                        type = message.Body.Parts[0].Type;
                    }
                    return true;
                }
                if (message.Body.Parts.Count > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.OnlyReturnValueBodyParameterAllowedForUnwrappedMessages, new object[] { declaringOperation.Name, declaringOperation.DeclaringContract.Name })));
                }
                if ((message.Body.ReturnValue != null) && (message.Body.ReturnValue.Type != typeof(void)))
                {
                    type = message.Body.ReturnValue.Type;
                }
            }
            return true;
        }

        private static bool TryGetStreamParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out Type type)
        {
            type = null;
            if (((message == null) || IsTypedMessage(message)) || IsUntypedMessage(message))
            {
                return false;
            }
            if (!isRequest)
            {
                for (int j = 0; j < message.Body.Parts.Count; j++)
                {
                    if (typeof(Stream) == message.Body.Parts[j].Type)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.NoOutOrRefStreamParametersAllowed, new object[] { message.Body.Parts[j].Name, declaringOperation.Name, declaringOperation.DeclaringContract.Name })));
                    }
                }
                if ((message.Body.ReturnValue == null) || (typeof(Stream) != message.Body.ReturnValue.Type))
                {
                    return false;
                }
                if (message.Body.Parts.Count > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.NoOutOrRefParametersAllowedWithStreamResult, new object[] { declaringOperation.Name, declaringOperation.DeclaringContract.Name })));
                }
                type = message.Body.ReturnValue.Type;
                return true;
            }
            bool flag = false;
            for (int i = 0; i < message.Body.Parts.Count; i++)
            {
                if (typeof(Stream) == message.Body.Parts[i].Type)
                {
                    type = message.Body.Parts[i].Type;
                    flag = true;
                    break;
                }
            }
            if (flag && (message.Body.Parts.Count > 1))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.AtMostOneRequestBodyParameterAllowedForStream, new object[] { declaringOperation.Name, declaringOperation.DeclaringContract.Name })));
            }
            return flag;
        }

        internal virtual bool UseBareReplyFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, WebMessageFormat responseFormat, out Type parameterType)
        {
            parameterType = null;
            return (IsBareResponse(style) && TryGetNonMessageParameterType(operationDescription.Messages[1], operationDescription, false, out parameterType));
        }

        internal virtual bool UseBareRequestFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, out Type parameterType)
        {
            parameterType = null;
            return (IsBareRequest(style) && TryGetNonMessageParameterType(operationDescription.Messages[0], operationDescription, true, out parameterType));
        }

        public virtual void Validate(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
            }
            this.ValidateNoMessageHeadersPresent(endpoint);
            this.ValidateBinding(endpoint);
            this.ValidateContract(endpoint);
        }

        private static void ValidateAtMostOneStreamParameter(OperationDescription operation, bool request)
        {
            Type type;
            if (request)
            {
                TryGetStreamParameterType(operation.Messages[0], operation, true, out type);
            }
            else if (operation.Messages.Count > 1)
            {
                TryGetStreamParameterType(operation.Messages[1], operation, false, out type);
            }
        }

        protected virtual void ValidateBinding(ServiceEndpoint endpoint)
        {
            ValidateIsWebHttpBinding(endpoint, base.GetType().ToString());
        }

        private void ValidateBodyParameters(OperationDescription operation, bool request)
        {
            string webMethod = GetWebMethod(operation);
            if (request)
            {
                this.ValidateGETHasNoBody(operation, webMethod);
            }
            this.ValidateBodyStyle(operation, request);
            ValidateAtMostOneStreamParameter(operation, request);
        }

        private void ValidateBodyStyle(OperationDescription operation, bool request)
        {
            Type type;
            WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operation);
            if (request && IsBareRequest(bodyStyle))
            {
                TryGetNonMessageParameterType(operation.Messages[0], operation, true, out type);
            }
            if ((!request && (operation.Messages.Count > 1)) && IsBareResponse(bodyStyle))
            {
                TryGetNonMessageParameterType(operation.Messages[1], operation, false, out type);
            }
        }

        private void ValidateContract(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                this.ValidateNoOperationHasEncodedXmlSerializer(description);
                this.ValidateNoMessageContractHeaders(description.Messages[0], description.Name, endpoint.Contract.Name);
                this.ValidateNoBareMessageContractWithMultipleParts(description.Messages[0], description.Name, endpoint.Contract.Name);
                this.ValidateNoMessageContractWithStream(description.Messages[0], description.Name, endpoint.Contract.Name);
                if (description.Messages.Count > 1)
                {
                    this.ValidateNoMessageContractHeaders(description.Messages[1], description.Name, endpoint.Contract.Name);
                    this.ValidateNoBareMessageContractWithMultipleParts(description.Messages[1], description.Name, endpoint.Contract.Name);
                    this.ValidateNoMessageContractWithStream(description.Messages[1], description.Name, endpoint.Contract.Name);
                }
            }
        }

        private void ValidateGETHasNoBody(OperationDescription operation, string method)
        {
            if (((method == "GET") && !IsUntypedMessage(operation.Messages[0])) && (operation.Messages[0].Body.Parts.Count != 0))
            {
                if (!IsTypedMessage(operation.Messages[0]))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveBody, new object[] { operation.Name, operation.DeclaringContract.Name, operation.Messages[0].Body.Parts[0].Name })));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveMCParameter, new object[] { operation.Name, operation.DeclaringContract.Name, operation.Messages[0].MessageType.Name })));
            }
        }

        internal static void ValidateIsWebHttpBinding(ServiceEndpoint serviceEndpoint, string behaviorName)
        {
            Binding binding = serviceEndpoint.Binding;
            if ((binding.Scheme != "http") && (binding.Scheme != "https"))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadScheme, new object[] { serviceEndpoint.Contract.Name, behaviorName })));
            }
            if (binding.MessageVersion != MessageVersion.None)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadMessageVersion, new object[] { serviceEndpoint.Address.Uri.AbsoluteUri, behaviorName })));
            }
            TransportBindingElement element = binding.CreateBindingElements().Find<TransportBindingElement>();
            if ((element != null) && !element.ManualAddressing)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.ManualAddressingCannotBeFalseWithTransportBindingElement, new object[] { serviceEndpoint.Address.Uri.AbsoluteUri, behaviorName, element.GetType().Name })));
            }
        }

        private void ValidateNoBareMessageContractWithMultipleParts(MessageDescription md, string opName, string contractName)
        {
            if (IsTypedMessage(md) && (md.Body.WrapperName == null))
            {
                if (md.Body.Parts.Count > 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.InvalidMessageContractWithoutWrapperName, new object[] { opName, contractName, md.MessageType })));
                }
                if ((md.Body.Parts.Count == 1) && md.Body.Parts[0].Multiple)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.MCAtMostOneRequestBodyParameterAllowedForUnwrappedMessages, new object[] { opName, contractName, md.MessageType })));
                }
            }
        }

        private void ValidateNoMessageContractHeaders(MessageDescription md, string opName, string contractName)
        {
            if (md.Headers.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.InvalidMethodWithSOAPHeaders, new object[] { opName, contractName })));
            }
        }

        private void ValidateNoMessageContractWithStream(MessageDescription md, string opName, string contractName)
        {
            if (IsTypedMessage(md))
            {
                foreach (MessagePartDescription description in md.Body.Parts)
                {
                    if (description.Type == typeof(Stream))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.StreamBodyMemberNotSupported, new object[] { base.GetType().ToString(), contractName, opName, md.MessageType.ToString(), description.Name })));
                    }
                }
            }
        }

        private void ValidateNoMessageHeadersPresent(ServiceEndpoint endpoint)
        {
            if ((endpoint != null) && (endpoint.Address != null))
            {
                EndpointAddress address = endpoint.Address;
                if (address.Headers.Count > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.WebHttpServiceEndpointCannotHaveMessageHeaders, new object[] { address })));
                }
            }
        }

        private void ValidateNoOperationHasEncodedXmlSerializer(OperationDescription od)
        {
            XmlSerializerOperationBehavior behavior = od.Behaviors.Find<XmlSerializerOperationBehavior>();
            if ((behavior != null) && ((behavior.XmlSerializerFormatAttribute.Style == OperationFormatStyle.Rpc) || behavior.XmlSerializerFormatAttribute.IsEncoded))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.RpcEncodedNotSupportedForNoneMessageVersion, new object[] { od.Name, od.DeclaringContract.Name, od.DeclaringContract.Namespace })));
            }
        }

        public virtual WebMessageBodyStyle DefaultBodyStyle
        {
            get => 
                this.defaultBodyStyle;
            set
            {
                if (!WebMessageBodyStyleHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.defaultBodyStyle = value;
            }
        }

        public virtual WebMessageFormat DefaultOutgoingRequestFormat
        {
            get => 
                this.defaultOutgoingRequestFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.defaultOutgoingRequestFormat = value;
            }
        }

        public virtual WebMessageFormat DefaultOutgoingResponseFormat
        {
            get => 
                this.defaultOutgoingReplyFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.defaultOutgoingReplyFormat = value;
            }
        }

        internal delegate void Effect();

        internal class MessagePassthroughFormatter : IClientMessageFormatter, IDispatchMessageFormatter
        {
            public object DeserializeReply(Message message, object[] parameters) => 
                message;

            public void DeserializeRequest(Message message, object[] parameters)
            {
                parameters[0] = message;
            }

            public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result) => 
                (result as Message);

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters) => 
                (parameters[0] as Message);
        }
    }
}

